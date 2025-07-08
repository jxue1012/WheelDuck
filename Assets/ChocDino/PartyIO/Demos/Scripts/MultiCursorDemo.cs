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
	/// <summary>
	/// Simple demo that shows how to use the MouseManager to update multiple mice, handle device connection/disconnection.
	/// IMGUI is used to display the cursors.
	/// </summary>
	/// <remarks>
	/// Note everything is calcualted in screen-coordinates.
	/// The bottom-left of the screen or window is at (0, 0). The top-right of the screen or window is at (Screen.width, Screen.height).
	/// Positions are converted to GUI coordinate during rendering.
	/// </remarks>
	public class MultiCursorDemo : MonoBehaviour
	{
		public class MouseCursor
		{
			public Mouse mouse;
			public Color color;
			public Vector3 screenPosition;
		}

		public class Particle
		{
			public Color color;
			public Vector3 screenPosition;
		}

		[SerializeField] Texture2D _cursorTexture = null;
		[SerializeField] Vector2 _cursorHotspot = Vector2.zero;
		[SerializeField] Texture2D _particleTexture = null;

		private MouseManager _mouseMan;
		private bool _isPaused;
		private List<MouseCursor> _cursors = new List<MouseCursor>(4);
		private Color[] _cursorColors = { Color.white, Color.red, Color.green, Color.blue };
		private int _cursorSpawnCount;
		private List<Particle> _particles = new List<Particle>(64);

		void OnEnable()
		{
			// Hide the system cursor
			Cursor.lockState = CursorLockMode.Locked;

			// Create the MouseManager
			MouseManager.ChangedConnectionState += OnChangedMouseConnectionState;
			_mouseMan = new MouseManager();
		}

		void OnDisable()
		{
			// Unhide the system cursor
			Cursor.lockState = CursorLockMode.None;

			// Destroy the MouseManager
			MouseManager.ChangedConnectionState -= OnChangedMouseConnectionState;
			_mouseMan.Dispose();
			_mouseMan = null;
		}

		void OnChangedMouseConnectionState(Mouse mouse)
		{
			if (mouse.ConnectionState == MouseConnectionState.Connected)
			{
				SpawnCursor(mouse);
			}
			else if (mouse.ConnectionState == MouseConnectionState.Disconnected)
			{
				DestroyCursor(mouse);
			}
		}

		void OnApplicationFocus(bool hasFocus)
		{
			_isPaused = !hasFocus;
			//Debug.Log("OnApplicationFocus: " + hasFocus);
		}

		void OnApplicationPause(bool pauseStatus)
		{
			_isPaused = pauseStatus;
			//Debug.Log("OnApplicationPause: " + pauseStatus);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
				return;
			}

			UpdateParticles();
 
			if (!_isPaused)
			{
				// Update the MouseManager
				_mouseMan.Update();
				
				// Update the cursors
				Vector3 screenMinimum = Vector3.zero; // bottom-left of the screen
				Vector3 screenMaximum = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0f); // top-right of the screen
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
					if (_particleTexture)
					{
						if (cursor.mouse.IsPressed(MouseButton.Left))
						{
							SpawnParticle(cursor.screenPosition, cursor.color);
						}
					}
				}
			}
		}

		void SpawnCursor(Mouse mouse)
		{
			Debug.Log(string.Format("Creating mouse #{0}: {1}/{2}\n({3})", mouse.DeviceId, mouse.FriendlyName, mouse.ManufacturerName, mouse.InstanceId));
			// If there is already a cursor assocated with this mouse then don't spawn a new one
			MouseCursor cursor = null;
			foreach (var c in _cursors)
			{
				if (mouse == c.mouse)
				{
					cursor = c;
					break;
				}
			}

			if (cursor == null)
			{
				cursor = new MouseCursor();
				cursor.mouse = mouse;
				cursor.screenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
				if (cursor.mouse.IsPositionAbsolute())
				{
					cursor.screenPosition = cursor.mouse.PositionDelta;
				}
				cursor.color = _cursorColors[_cursorSpawnCount % _cursorColors.Length];
				_cursors.Add(cursor);
				_cursorSpawnCount++;
			}
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

		void SpawnParticle(Vector3 screenPosition, Color color)
		{
			var particle = new Particle();
			particle.screenPosition = screenPosition;
			particle.color = color;
			_particles.Add(particle);
		}

		void UpdateParticles()
		{
			// Update particles
			for (int i = 0; i < _particles.Count; i++)
			{
				_particles[i].screenPosition += Vector3.down * Time.deltaTime * 1000f;
			}

			// Remove dead particles that have fallen off the bottom of the screen
			for (int i = 0; i < _particles.Count; i++)
			{
				if (_particles[i].screenPosition.y < (-_particleTexture.height / 2f))
				{
					_particles.RemoveAt(i);
					i = 0;
				}
			}
		}

		void OnGUI()
		{
			// Draw particles
			foreach (var particle in _particles)
			{
				// Offset the rectangle so the particle is drawn centered on its position
				Vector2 offset = new Vector2(_particleTexture.width / 2f, -_particleTexture.height / 2f);
				var rect = new Rect(particle.screenPosition.x, particle.screenPosition.y, _particleTexture.width, _particleTexture.height);
				rect.position -= offset;
				
				// Convert from screen-space to GUI space
				rect.y = Screen.height - rect.y;
				
				GUI.color = particle.color;
				GUI.DrawTexture(rect, _particleTexture, ScaleMode.StretchToFill, true);
			}
			
			// Draw cursors
			foreach (var cursor in _cursors)
			{
				// Offset the rectangle so the cursor hotspot is drawn at the correct position
				Vector2 cursorOffset = new Vector2(_cursorTexture.width * _cursorHotspot.x, -_cursorTexture.height * _cursorHotspot.y);
				var rect = new Rect(cursor.screenPosition.x, cursor.screenPosition.y, _cursorTexture.width, _cursorTexture.height);
				rect.position -= cursorOffset;

				// Convert from screen-space to GUI space
				rect.y = Screen.height - rect.y;

				GUI.color = cursor.color;
				GUI.DrawTexture(rect, _cursorTexture, ScaleMode.StretchToFill, true);
			}
		}
	}
}