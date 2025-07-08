//--------------------------------------------------------------------------//
// Copyright 2025 Chocolate Dinosaur Ltd. All rights reserved.              //
// For full documentation visit https://www.chocolatedinosaur.com           //
//--------------------------------------------------------------------------//

#if UNITY_EDITOR_WIN || (!UNITY_EDITOR && UNITY_STANDALONE_WIN)
#define MOUSEPARTY_PLATFORM
#endif

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ChocDino.PartyIO
{
    [AddComponentMenu("Event/Mouse Party Input Module")]
    /// <summary>
    /// Input module for working with multiple mice
    /// </summary>
    public class MousePartyInputModule : StandaloneInputModule
    {
        private PointerEventData m_InputPointerEvent;
		private int _lastFrameUpdated = -1;
		private readonly List<MouseState> _mouseStates = new List<MouseState>();
		private List<Vector3> _mousePositions = new List<Vector3>();

		public bool _enableMouseParty = true;

        private bool ShouldIgnoreEventsOnNoFocus()
        {
#if UNITY_EDITOR
            return !UnityEditor.EditorApplication.isRemoteConnected;
#else
            return true;
#endif
        }

		private bool IsMouseParty()
		{
			#if MOUSEPARTY_PLATFORM
			return _enableMouseParty;
			#else
			return false;
			#endif
		}

        public override void UpdateModule()
        {
			if (!IsMouseParty())
			{
				base.UpdateModule();
				return;
			}

			// If the window is not in focus then release any existing mouse state
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
            {
				// TODO: need to make this support multiple mice?
                if (m_InputPointerEvent != null && m_InputPointerEvent.pointerDrag != null && m_InputPointerEvent.dragging)
                {
                    ReleaseMouse(m_InputPointerEvent, m_InputPointerEvent.pointerCurrentRaycast.gameObject);
                }

                m_InputPointerEvent = null;

                return;
            }
        }

        private void ReleaseMouse(PointerEventData pointerEvent, GameObject currentOverGo)
        {
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // PointerClick and Drop events
            if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            // redo pointer enter / exit to refresh state
            // so that if we moused over something that ignored it before
            // due to having pressed on something else
            // it now gets it.
            if (currentOverGo != pointerEvent.pointerEnter)
            {
                HandlePointerExitAndEnter(pointerEvent, null);
                HandlePointerExitAndEnter(pointerEvent, currentOverGo);
            }

            m_InputPointerEvent = pointerEvent;
        }

        public override bool ShouldActivateModule()
        {
            var shouldActivate = false;

            // TODO: detect any changes in input since the last frame and return true
			if (IsMouseParty())
			{
				if (!shouldActivate && _lastFrameUpdated != MouseManager.Instance.LastFrameUpdated)
				{
					shouldActivate = true;
				}
			}

            if (!shouldActivate && !base.ShouldActivateModule())
            {
				shouldActivate = false;
			}

            return shouldActivate;
        }

        /// <summary>
        /// See BaseInputModule.
        /// </summary>
        public override void ActivateModule()
        {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
                return;

            base.ActivateModule();

			if (IsMouseParty())
			{
				// TODO: reset mouse positions?
				_lastFrameUpdated = MouseManager.Instance.LastFrameUpdated;
			}
        }

        public override void Process()
        {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
                return;

            bool usedEvent = SendUpdateEventToSelectedObject();

            // case 1004066 - touch / mouse events should be processed before navigation events in case
            // they change the current selected gameobject and the submit button is a touch / mouse button.

            // touch needs to take precedence because of the mouse emulation layer
            if (!ProcessTouchEvents() && input.mousePresent)
			{
				if (IsMouseParty())
				{
                	ProcessMouseEvent();
				}
				else
				{
					base.ProcessMouseEvent();
				}
			}
        }

        private bool ProcessTouchEvents()
        {
            for (int i = 0; i < input.touchCount; ++i)
            {
                Touch touch = input.GetTouch(i);

                if (touch.type == TouchType.Indirect)
                    continue;

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(touch, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if (!released)
                {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
                else
                    RemovePointerData(pointer);
            }
            return input.touchCount > 0;
        }

        protected new void ProcessMouseEvent()
        {
			var mice = MouseManager.Instance.All;
			foreach (var mouse in mice)
			{
				if (mouse.ConnectionState == MouseConnectionState.Connected)
				{
            		ProcessMouseEvent(mouse);
				}
				else
				{
					// Remove the mouse if it isn't connected
					int baseId = mouse.DeviceId * 200;
					m_PointerData.Remove(baseId + kMouseMiddleId);
					m_PointerData.Remove(baseId + kMouseRightId);
					m_PointerData.Remove(baseId + kMouseLeftId);
				}
			}
        }

        /// <summary>
        /// Process all mouse events.
        /// </summary>
        protected void ProcessMouseEvent(Mouse mouse)
        {
            var mouseData = GetMousePointerEventData(mouse);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            // Now process right / middle clicks
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

            if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            }
        }

        /// <summary>
        /// Return the current MouseState.
        /// </summary>
        protected MouseState GetMousePointerEventData(Mouse mouse)
        {
			while (_mouseStates.Count <= mouse.DeviceId)
			{
				_mouseStates.Add(null);
				_mousePositions.Add(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
			}
			if (_mouseStates[mouse.DeviceId] == null)
			{
				_mouseStates[mouse.DeviceId] = new MouseState();
			}

			Vector3 screenMinimum = Vector3.zero; // bottom-left of the screen
			Vector3 screenMaximum = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0f); // top-right of the screen
			if (mouse.IsPositionAbsolute())
			{
				Vector3 screenPosition = mouse.PositionDelta;
				screenPosition = Vector3.Max(screenPosition, screenMinimum);
				screenPosition = Vector3.Min(screenPosition, screenMaximum);
				_mousePositions[mouse.DeviceId] = screenPosition;
			}
			else if (mouse.PositionDelta != Vector3.zero)
			{
				Vector3 screenPosition = _mousePositions[mouse.DeviceId] + new Vector3(mouse.PositionDelta.x, -mouse.PositionDelta.y, 0f);
				screenPosition = Vector3.Max(screenPosition, screenMinimum);
				screenPosition = Vector3.Min(screenPosition, screenMaximum);
				_mousePositions[mouse.DeviceId] = screenPosition;
			}

			int baseId = mouse.DeviceId * 200;

            // Populate the left button...
            PointerEventData leftData;
            var created = GetPointerData(baseId + kMouseLeftId, out leftData, true);

            leftData.Reset();

            if (created)
                leftData.position = _mousePositions[mouse.DeviceId];

            Vector2 pos = _mousePositions[mouse.DeviceId];
            /*if (Cursor.lockState != CursorLockMode.Locked)
            {
                // We don't want to do ANY cursor-based interaction when the mouse is locked
                leftData.position = new Vector2(-1.0f, -1.0f);
                leftData.delta = Vector2.zero;
            }
            else*/
            {
                leftData.delta = pos - leftData.position;
                leftData.position = pos;
            }
            leftData.scrollDelta = mouse.ScrollDelta;
            leftData.button = PointerEventData.InputButton.Left;
            eventSystem.RaycastAll(leftData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            leftData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();

            // copy the apropriate data into right and middle slots
            PointerEventData rightData;
            GetPointerData(baseId + kMouseRightId, out rightData, true);
            CopyFromTo(leftData, rightData);
            rightData.button = PointerEventData.InputButton.Right;

            PointerEventData middleData;
            GetPointerData(baseId + kMouseMiddleId, out middleData, true);
            CopyFromTo(leftData, middleData);
            middleData.button = PointerEventData.InputButton.Middle;

            _mouseStates[mouse.DeviceId].SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(mouse, MouseButton.Left), leftData);
            _mouseStates[mouse.DeviceId].SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(mouse, MouseButton.Right), rightData);
            _mouseStates[mouse.DeviceId].SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(mouse, MouseButton.Middle), middleData);

            return _mouseStates[mouse.DeviceId];
        }

        protected PointerEventData.FramePressState StateForMouseButton(Mouse mouse, MouseButton buttonId)
        {
            var pressed = mouse.WasPressedThisFrame(buttonId);
            var released = mouse.WasReleasedThisFrame(buttonId);
            if (pressed && released)
                return PointerEventData.FramePressState.PressedAndReleased;
            if (pressed)
                return PointerEventData.FramePressState.Pressed;
            if (released)
                return PointerEventData.FramePressState.Released;
            return PointerEventData.FramePressState.NotChanged;
        }

        /// <summary>
        /// Process movement for the current frame with the given pointer event.
        /// </summary>
        protected override void ProcessMove(PointerEventData pointerEvent)
        {
			if (!IsMouseParty())
			{
				base.ProcessMove(pointerEvent);
				return;
			}

            var targetGO = pointerEvent.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointerEvent, targetGO);
        }

        private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
        {
            if (!useDragThreshold)
                return true;

            return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
        }

        /// <summary>
        /// Process the drag for the current frame with the given pointer event.
        /// </summary>
        protected override void ProcessDrag(PointerEventData pointerEvent)
        {
			if (!IsMouseParty())
			{
				base.ProcessDrag(pointerEvent);
				return;
			}

            if (!pointerEvent.IsPointerMoving() ||
                pointerEvent.pointerDrag == null)
                return;

            if (!pointerEvent.dragging
                && ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
                pointerEvent.dragging = true;
            }

            // Drag notification
            if (pointerEvent.dragging)
            {
                // Before doing drag we should cancel any pointer down state
                // And clear selection!
                if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                    pointerEvent.eligibleForClick = false;
                    pointerEvent.pointerPress = null;
                    pointerEvent.rawPointerPress = null;
                }
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
            }
        }

		#if PACKAGE_UITOOLKIT
        /// <summary>
        /// Returns Id of the pointer following <see cref="UnityEngine.UIElements.PointerId"/> convention.
        /// </summary>
        /// <param name="sourcePointerData">PointerEventData whose pointerId will be converted to UI Toolkit pointer convention.</param>
        /// <seealso cref="UnityEngine.UIElements.IPointerEvent" />
        public override int ConvertUIToolkitPointerId(PointerEventData sourcePointerData)
        {
			return 0 + ((sourcePointerData.pointerId + 1) / 200);
        }

		#endif
    }
}