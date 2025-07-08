//--------------------------------------------------------------------------//
// Copyright 2025 Chocolate Dinosaur Ltd. All rights reserved.              //
// For full documentation visit https://www.chocolatedinosaur.com           //
//--------------------------------------------------------------------------//

using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using ChocDino.PartyIO.Internal;

namespace ChocDino.PartyIO
{
	public class MouseDebugOverlay : MonoBehaviour
	{
		void OnGUI()
		{
			float scale = 1f;
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale * Screen.width / 1280f, scale * Screen.height / 720f, 1f));
			GUILayout.BeginVertical(GUI.skin.box);
			var mice = MouseManager.Instance.All;
			foreach (var mouse in mice)
			{
				string mouseButtonText = string.Empty;
				for (int i = 0; i < 5; i++)
				{
					mouseButtonText += mouse.IsPressed((MouseButton)i) ? "1" : "0";
				}
				string mouseButtonDownText = string.Empty;
				for (int i = 0; i < 5; i++)
				{
					mouseButtonDownText += mouse.WasPressedThisFrame((MouseButton)i) ? "1" : "0";
				}
				string mouseButtonUpText = string.Empty;
				for (int i = 0; i < 5; i++)
				{
					mouseButtonUpText += mouse.WasReleasedThisFrame((MouseButton)i) ? "1" : "0";
				}
				
				string connectionColor = "yellow";
				if (mouse.ConnectionState == MouseConnectionState.Connected)
				{
					connectionColor = "green";
				}
				if (mouse.ConnectionState == MouseConnectionState.Disconnected)
				{
					connectionColor = "red";
				}
				
				string text = string.Empty;
				text = string.Format("Mouse Id: #{0} Name: {2} Manufacturer: {3} <color={4}>Status:{1}</color>", mouse.DeviceId, mouse.ConnectionState.ToString(), mouse.FriendlyName, mouse.ManufacturerName, connectionColor);
				GUILayout.Label(text);
				if (mouse.ConnectionState == MouseConnectionState.Connected)
				{
					text = string.Format("     InstanceId: {0} IsUnique: {1} ", mouse.InstanceId, mouse.IsInstanceIdUnique);
					GUILayout.Label(text);
					if (mouse.IsPositionAbsolute())
					{
						text = string.Format("     Absolute: {0} VirtualDesktop: {1} ", true, mouse.IsVirtualDesktopPosition());
						GUILayout.Label(text);
					}
					text = string.Format("     PositionΔ: {0:+000.00;-000.00;+000.00},{1:+000.00;-000.00;+000.00} ScrollΔ: {2:+000.00;-000.00;+000.00},{3:+000.00;-000.00;+000.00}", mouse.PositionDelta.x, mouse.PositionDelta.y, mouse.ScrollDelta.x, mouse.ScrollDelta.y);
					GUILayout.Label(text);
					text = string.Format("     Buttons: {0} / Down: {1} Up: {2}", mouseButtonText, mouseButtonDownText, mouseButtonUpText);
					GUILayout.Label(text);
				}
			}
			GUILayout.EndVertical();
		}
	}
}