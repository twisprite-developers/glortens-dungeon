using UnityEngine;
using System.Collections;
using UnityEditor;
using TwinSpriteSDK;
using TwinSpriteSDKPrivate;
using System.Collections.Generic;

[CustomEditor(typeof(ToyxManager))]
public class ToyxInspectorManager : Editor {

	bool showConsole = true;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		
		ToyxManager toyxManager = (ToyxManager)target;

		// Create session button
		if(GUILayout.Button("Create Session")) {
			toyxManager.CreateSession(null);
		}

		// Fetch if needed button
		if(GUILayout.Button("Fetch If Needed")) {
			toyxManager.FetchIfNeeded(null);
		}

		// Fetch button
		if(GUILayout.Button("Fetch")) {
			toyxManager.Fetch(null);
		}

		// Save button
		if(GUILayout.Button("Save")) {
			toyxManager.Save(null);
		}

		// SaveEventually button
		if(GUILayout.Button("Save Eventually")) {
			toyxManager.SaveEventually();
		}

		// Show Console button
		if (showConsole) {
			if(GUILayout.Button("Hide Console")) {
				showConsole = false;
			}
		} else {
			if(GUILayout.Button("Show Console")) {
				showConsole = true;
			}
		}

		// Clear console
		if(GUILayout.Button("Clear Console")) {
			toyxManager.infoMessage = "";
		}

		// Console
		if (showConsole && toyxManager.infoMessage != null) {
			GUILayout.Label(toyxManager.infoMessage);
		}


	}


}
