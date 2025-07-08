//--------------------------------------------------------------------------//
// Copyright 2025 Chocolate Dinosaur Ltd. All rights reserved.              //
// For full documentation visit https://www.chocolatedinosaur.com           //
//--------------------------------------------------------------------------//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ChocDino.PartyIO;

namespace ChocDino.PartyIO.Demos
{
	/// <remarks>
	/// Note everything is calcualted in screen-coordinates.
	/// The bottom-left of the screen or window is at (0, 0). The top-right of the screen or window is at (Screen.width, Screen.height).
	/// Positions are converted to GUI coordinate during rendering.
	/// </remarks>
	public class ShootingPartyDemo : MonoBehaviour
	{
		public class MouseCursor
		{
			public Mouse mouse;
			public Color color;
			public Vector3 screenPosition;
		}

		public class Bullet
		{
			public Color color;
			public Vector3 screenPosition;
			public Vector3 targetScreenPosition;
			public bool dead;
		}

		[SerializeField] Texture2D _cursorTexture = null;
		[SerializeField] Vector2 _cursorHotspot = Vector2.zero;
		[SerializeField] Texture2D _bulletTexture = null;

		private MouseManager _mouseMan;
		private bool _isPaused;
		private List<MouseCursor> _cursors = new List<MouseCursor>(4);
		private Color[] _cursorColors = { Color.red, Color.green, Color.blue, Color.magenta, Color.cyan, Color.yellow };
		private int _cursorSpawnCount;
		private List<Bullet> _bullets = new List<Bullet>(64);

		private List<Bullet> _badBullets = new List<Bullet>(64);
		private float _badBulletSpawnTimer;

		void OnEnable()
		{
			Cursor.lockState = CursorLockMode.Locked;
			MouseManager.ChangedConnectionState += OnChangedMouseConnectionState;
			_mouseMan = new MouseManager();
		}

		void OnDisable()
		{
			Cursor.lockState = CursorLockMode.None;
			MouseManager.ChangedConnectionState -= OnChangedMouseConnectionState;
			_mouseMan.Dispose();
			_mouseMan = null;
		}

		List<RaycastResult> m_RaycastResultCache = new List<RaycastResult>();

		void Update()
		{
			UpdateBullets();

			if (!_isPaused)
			{
				_mouseMan.Update();
				
				//Vector3 cursorOffset = new Vector3(_cursorTexture.width * _cursorHotspot.x, _cursorTexture.height * _cursorHotspot.y, 0f);
				Vector3 screenMinimum = Vector3.zero;// - cursorOffset;  //bottom-left
				Vector3 screenMaximum = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0f);// - cursorOffset; // top-right
				foreach (var cursor in _cursors)
				{
					// Update cursor position clamping to game view area
					{
						Vector3 newPosition = cursor.screenPosition;
						if (cursor.mouse.IsPositionAbsolute())
						{
							newPosition = cursor.mouse.PositionDelta;
						}
						else if (cursor.mouse.PositionDelta != Vector3.zero)
						{
							// NOTE: mouse delta Y is negated to match Unity's screen-space convention
							newPosition += new Vector3(cursor.mouse.PositionDelta.x, -cursor.mouse.PositionDelta.y, 0f);
						}
						if (newPosition != cursor.screenPosition)
						{
							newPosition = Vector3.Max(newPosition, screenMinimum);
							newPosition = Vector3.Min(newPosition, screenMaximum);
							cursor.screenPosition = newPosition;
						}
					}

					// Spawn particles on mouse down
					if (cursor.mouse.WasPressedThisFrame(MouseButton.Left))
					{
						SpawnPlayerBullet(cursor.screenPosition, cursor.color);
					}
					//Debug.Log(cursor.mouse.WasPressedThisFrame(MouseButton.Left) + " " + cursor.mouse.WasReleasedThisFrame(MouseButton.Left));
	
					/*if (cursor.mouse.WasPressedThisFrame(MouseButton.Left))
					{
						PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
						pointerEvent.position = new Vector2(cursor.screenPosition.x, cursor.screenPosition.y);
						pointerEvent.pressPosition = pointerEvent.position;
						pointerEvent.delta = new Vector2(cursor.mouse.PositionDelta.x, cursor.mouse.PositionDelta.y);
						pointerEvent.pointerPress = _buttonGo;

						EventSystem.current.RaycastAll(pointerEvent, m_RaycastResultCache);
						var raycast = FindFirstRaycast(m_RaycastResultCache);
						pointerEvent.pointerCurrentRaycast = raycast;
						m_RaycastResultCache.Clear();

						if (pointerEvent.pointerCurrentRaycast.gameObject)
						{
							//Debug.Log("hit " + pointerEvent.pointerCurrentRaycast.gameObject.name);
							ExecuteEvents.Execute(pointerEvent.pointerCurrentRaycast.gameObject, pointerEvent, ExecuteEvents.pointerDownHandler);
						}
						else
						{
							//Debug.Log("miss");
						}
					}
					else if (cursor.mouse.WasReleasedThisFrame(MouseButton.Left))
					{
						PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
						pointerEvent.position = new Vector2(cursor.screenPosition.x, cursor.screenPosition.y);
						pointerEvent.pressPosition = pointerEvent.position;
						pointerEvent.delta = new Vector2(cursor.mouse.PositionDelta.x, cursor.mouse.PositionDelta.y);
						pointerEvent.pointerPress = _buttonGo;
						pointerEvent.pointerCurrentRaycast = new RaycastResult();

						EventSystem.current.RaycastAll(pointerEvent, m_RaycastResultCache);
						var raycast = FindFirstRaycast(m_RaycastResultCache);
						pointerEvent.pointerCurrentRaycast = raycast;
						m_RaycastResultCache.Clear();

						if (pointerEvent.pointerCurrentRaycast.gameObject)
						{
							Debug.Log("RELEASE hit " + pointerEvent.pointerCurrentRaycast.gameObject.name);
							ExecuteEvents.Execute(pointerEvent.pointerCurrentRaycast.gameObject, pointerEvent, ExecuteEvents.pointerUpHandler);
						}
						else
						{
							Debug.Log("RELEASE miss");
						}
					}*/
				}
			}
		}

        protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            for (var i = 0; i < candidates.Count; ++i)
            {
                if (candidates[i].gameObject == null)
                    continue;

                return candidates[i];
            }
            return new RaycastResult();
        }

		void OnApplicationFocus(bool hasFocus)
		{
			_isPaused = !hasFocus;
		}

		void OnApplicationPause(bool pauseStatus)
		{
			_isPaused = pauseStatus;
		}

		void OnChangedMouseConnectionState(Mouse mouse)
		{
			if (mouse.ConnectionState == MouseConnectionState.Connected)
			{
				SpawnCursor(mouse);
			}
			else
			{
				DestroyCursor(mouse);
			}
		}

		void SpawnCursor(Mouse mouse)
		{
			var cursor = new MouseCursor();
			cursor.mouse = mouse;
			cursor.screenPosition = new Vector3(Camera.main.pixelWidth / 2f, Camera.main.pixelHeight / 2f, 0f);
			cursor.color = _cursorColors[_cursorSpawnCount % _cursorColors.Length];
			_cursors.Add(cursor);
			_cursorSpawnCount++;
		}

		void DestroyCursor(Mouse mouse)
		{
			for (int i = 0; i < _cursors.Count; i++)
			{
				if (_cursors[i].mouse == mouse)
				{
					_cursors.RemoveAt(i);
					break;
				}
			}
		}

		void SpawnPlayerBullet(Vector3 screenPosition, Color color)
		{
			var bullet = new Bullet();
			bullet.targetScreenPosition = screenPosition;
			bullet.screenPosition = new Vector3(Camera.main.pixelWidth * 0.5f, 0f, 0f);
			bullet.color = color;
			_bullets.Add(bullet);
		}

		void SpawnBadBullet(Vector3 screenPosition, Color color)
		{
			var bullet = new Bullet();
			bullet.targetScreenPosition = new Vector3(Camera.main.pixelWidth * 0.5f, 0f, 0f);
			bullet.screenPosition = screenPosition;
			bullet.color = color;
			_badBullets.Add(bullet);
		}

		void UpdateBullets()
		{
			// Update bullets
			for (int i = 0; i < _bullets.Count; i++)
			{
				_bullets[i].screenPosition = Vector3.MoveTowards(_bullets[i].screenPosition, _bullets[i].targetScreenPosition, Time.deltaTime * 1200f);
			}
			for (int i = 0; i < _badBullets.Count; i++)
			{
				_badBullets[i].screenPosition = Vector3.MoveTowards(_badBullets[i].screenPosition, _badBullets[i].targetScreenPosition, Time.deltaTime * 500f);
			}

			// Detect collisions
			List<Bullet> _removeBullets = new List<Bullet>(16);
			for (int i = 0; i < _bullets.Count; i++)
			{
				var bullet = _bullets[i];
				for (int j = 0; j < _badBullets.Count; j++)
				{
					var badBullet = _badBullets[j];
					float d = (bullet.screenPosition - badBullet.screenPosition).magnitude;
					if (d < 40f)
					{
						badBullet.dead = true;
						bullet.dead = true;
					}
				}
			}
			_bullets.RemoveAll((x)=> x.dead);
			_badBullets.RemoveAll((x)=> x.dead);


			// Remove dead bullets
			for (int i = 0; i < _bullets.Count; i++)
			{
				float d = (_bullets[i].screenPosition - _bullets[i].targetScreenPosition).magnitude;

				if (d < 10f)
				{
					_bullets.RemoveAt(i);
					i = 0;
				}
			}
			for (int i = 0; i < _badBullets.Count; i++)
			{
				float d = (_badBullets[i].screenPosition - _badBullets[i].targetScreenPosition).magnitude;

				if (d < 10f)
				{
					_badBullets.RemoveAt(i);
					i = 0;
				}
			}

			// Spawn bad bullets
			_badBulletSpawnTimer += Time.deltaTime;
			const float SpawnDuration = 0.33f;
			if (_badBulletSpawnTimer > SpawnDuration)
			{
				SpawnBadBullet(new Vector3(Camera.main.pixelWidth * Random.value, Camera.main.pixelHeight, 0f), Color.white);
				_badBulletSpawnTimer -= SpawnDuration;
			}
		}

		void OnGUI()
		{
			// Draw bullet particles
			foreach (var bullet in _bullets)
			{
				Vector2 offset = new Vector2(_bulletTexture.width / 2f, -_bulletTexture.height / 2f);
				var rect = new Rect(bullet.screenPosition.x, bullet.screenPosition.y, _bulletTexture.width, _bulletTexture.height);
				rect.position -= offset;
				rect.y = Screen.height - rect.y;
				GUI.color = bullet.color;
				GUI.DrawTexture(rect, _bulletTexture, ScaleMode.StretchToFill, true);
			}
			foreach (var bullet in _badBullets)
			{
				Vector2 offset = new Vector2(_bulletTexture.width / 2f, -_bulletTexture.height / 2f);
				var rect = new Rect(bullet.screenPosition.x, bullet.screenPosition.y, _bulletTexture.width, _bulletTexture.height);
				rect.position -= offset;
				rect.y = Screen.height - rect.y;
				GUI.color = bullet.color;
				GUI.DrawTexture(rect, _bulletTexture, ScaleMode.StretchToFill, true);
			}
			
			// Draw cursors
			foreach (var cursor in _cursors)
			{
				Vector2 cursorOffset = new Vector2(_cursorTexture.width * _cursorHotspot.x, -_cursorTexture.height * _cursorHotspot.y);
				var rect = new Rect(cursor.screenPosition.x, cursor.screenPosition.y, _cursorTexture.width, _cursorTexture.height);
				rect.position -= cursorOffset;
				rect.y = Screen.height - rect.y;
				GUI.color = cursor.color;
				GUI.DrawTexture(rect, _cursorTexture, ScaleMode.StretchToFill, true);
			}
		}
	}
}