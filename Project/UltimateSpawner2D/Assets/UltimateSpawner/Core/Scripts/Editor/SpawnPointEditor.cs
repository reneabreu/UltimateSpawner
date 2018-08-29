using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UltimateSpawner {
	
	[CustomEditor(typeof(SpawnPoint))]
	public class SpawnPointEditor : Editor {

		public override void OnInspectorGUI() {
			
			var spawnPoint = (SpawnPoint) target;
			
			spawnPoint.positionType =
				(SpawnPointPosition) EditorGUILayout.EnumPopup("Position Type", spawnPoint.positionType);
			
			GUILayout.Space(10);

			if(spawnPoint.positionType == SpawnPointPosition.Vector3)
				spawnPoint.vectorPosition = EditorGUILayout.Vector3Field("Force", spawnPoint.vectorPosition);
			
			else if (spawnPoint.positionType == SpawnPointPosition.ScreenBased) {
				spawnPoint.screenBasedPosition =
					(SpawnPoint.ScreenBased) EditorGUILayout.EnumPopup("Screen Point", spawnPoint.screenBasedPosition);
				
				GUILayout.Space(5);
				
				spawnPoint.offset = EditorGUILayout.Vector3Field("Offset", spawnPoint.offset);


			}

			EditorUtility.SetDirty(spawnPoint);
		}
	}
}