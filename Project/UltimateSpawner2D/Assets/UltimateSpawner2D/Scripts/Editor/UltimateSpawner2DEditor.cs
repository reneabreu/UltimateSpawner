using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UltimateSpawner2D {
	[CustomEditor(typeof(UltimateSpawner2D))]
	public class UltimateSpawner2DEditor : Editor {
		
		private UltimateSpawner2D ultimateSpawner2D;

		SerializedProperty spawnPointsList;

		
		public override void OnInspectorGUI() {
			serializedObject.Update();
			
			ultimateSpawner2D = (UltimateSpawner2D) target;
			
			GUILayout.Label("Basic Settings", EditorStyles.boldLabel);
						
//			EditorGUILayout.PropertyField(background, new GUIContent("Background Color"));
			
			ultimateSpawner2D.objectToSpawn = (GameObject) EditorGUILayout.ObjectField("Object to Spawn",
				ultimateSpawner2D.objectToSpawn, typeof(GameObject), true);
			
			ultimateSpawner2D.spawnMode =
				(SpawnMode) EditorGUILayout.EnumPopup("Spawn Mode", ultimateSpawner2D.spawnMode);

			
			ultimateSpawner2D.usePoolSystem = EditorGUILayout.Toggle("Use Pooling System?", ultimateSpawner2D.usePoolSystem);
			
			GUILayout.Space(10);

			ultimateSpawner2D.ShowDebugMessages = EditorGUILayout.Toggle("Show Debug Log?", ultimateSpawner2D.ShowDebugMessages);

			// Line Divider		
			GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
			
			if (ultimateSpawner2D.spawnMode == SpawnMode.Input) {
				
				GUILayout.Space(10);
			
				GUILayout.Label("Input Settings", EditorStyles.boldLabel);
				
				ultimateSpawner2D.inputKeyCode =
					(KeyCode) EditorGUILayout.EnumPopup("Input KeyCode", ultimateSpawner2D.inputKeyCode);
			} else if (ultimateSpawner2D.spawnMode == SpawnMode.External) {
				EditorGUILayout.HelpBox("Currently you can call the public method Spawn() from another script", MessageType.Info, true);
			} else {
				ShowTimerSettings();
			}
			if(ultimateSpawner2D.usePoolSystem)
				ShowPoolSettings();

			ShowPositionSettings();
			
			serializedObject.ApplyModifiedProperties();
		}

		void ShowPoolSettings() {
			GUILayout.Space(10);
			
			GUILayout.Label("Pool Settings", EditorStyles.boldLabel);

			ultimateSpawner2D.poolSize = EditorGUILayout.IntField("Pool Size", ultimateSpawner2D.poolSize);
			
			ultimateSpawner2D.canIncreasePoolSize = EditorGUILayout.Toggle("Can increase pool size?", ultimateSpawner2D.canIncreasePoolSize);

			if (ultimateSpawner2D.poolMaxSize < ultimateSpawner2D.poolSize)
				ultimateSpawner2D.poolMaxSize = ultimateSpawner2D.poolSize + 1;
			
			if(ultimateSpawner2D.canIncreasePoolSize)
				ultimateSpawner2D.poolMaxSize = EditorGUILayout.IntField("Pool Maximum Size", ultimateSpawner2D.poolMaxSize);

		}

		void ShowTimerSettings() {
			GUILayout.Space(10);
			
			GUILayout.Label("Timer Settings", EditorStyles.boldLabel);

			ultimateSpawner2D.firstSpawnTime = EditorGUILayout.FloatField("First Spawn Time", ultimateSpawner2D.firstSpawnTime);

			GUILayout.Space(5);

			
			if (ultimateSpawner2D.spawnMode == SpawnMode.FixedTime) {
				GUILayout.Label("Fixed Time Settings");
				ultimateSpawner2D.fixedDelayBetweenSpawns = EditorGUILayout.FloatField("Delay Between Spawns", ultimateSpawner2D.fixedDelayBetweenSpawns);
				
			} else if (ultimateSpawner2D.spawnMode == SpawnMode.RandomTime) {
				GUILayout.Label("Random Time Settings");
				ultimateSpawner2D.minDelayBetweenSpawns = EditorGUILayout.FloatField("Maximum Delay Between Spawns", ultimateSpawner2D.minDelayBetweenSpawns);
				ultimateSpawner2D.maxDelayBetweenSpawns = EditorGUILayout.FloatField("Minimum Delay Between Spawns", ultimateSpawner2D.maxDelayBetweenSpawns);
				
			} else if (ultimateSpawner2D.spawnMode == SpawnMode.ProgressiveTime) {
				GUILayout.Label("Progressive Time Settings");
				ultimateSpawner2D.startingDelayBetweenSpawns = EditorGUILayout.FloatField("Starting Delay Between Spawns", ultimateSpawner2D.startingDelayBetweenSpawns);
				ultimateSpawner2D.delayModifier = EditorGUILayout.FloatField("Delay Modifier", ultimateSpawner2D.delayModifier);
				ultimateSpawner2D.progressiveDelayLimit = EditorGUILayout.FloatField("Delay Limit", ultimateSpawner2D.progressiveDelayLimit);

				if (ultimateSpawner2D.progressiveDelayLimit <= 0)
					ultimateSpawner2D.progressiveDelayLimit = 0;
			}

		}

		void ShowPositionSettings() {
			
			spawnPointsList = serializedObject.FindProperty("spawnPoints");
		
			GUILayout.Space(10);
			
			GUILayout.Label("Position Settings", EditorStyles.boldLabel);
		
			ultimateSpawner2D.spawnAt =
				(SpawnAt) EditorGUILayout.EnumPopup("Spawn Mode", ultimateSpawner2D.spawnAt);
		
			
			if (ultimateSpawner2D.spawnAt == SpawnAt.Fixed) {
				GUILayout.Label("Fixed Spawn Point");
				
				ultimateSpawner2D.fixedPosition = (PositionToSpawn) EditorGUILayout.ObjectField("Spawn Point",
					ultimateSpawner2D.fixedPosition, typeof(PositionToSpawn), true);
				
			} else if (ultimateSpawner2D.spawnAt == SpawnAt.Random) {
				
				EditorGUILayout.PropertyField(spawnPointsList, new GUIContent("Random Spawn Points"));
				
				// List
				EditorGUI.indentLevel += 1;
				if (spawnPointsList.isExpanded) {
					EditorGUILayout.PropertyField(spawnPointsList.FindPropertyRelative("Array.size"));
					for (int i = 0; i < spawnPointsList.arraySize; i++) {
						EditorGUILayout.PropertyField(spawnPointsList.GetArrayElementAtIndex(i));
					}
				}
				EditorGUI.indentLevel -= 1;

			} else if (ultimateSpawner2D.spawnAt == SpawnAt.TargetTransform) {
				GUILayout.Label("Transform Spawn Point");
					
				ultimateSpawner2D.targetTransform = (Transform) EditorGUILayout.ObjectField("Spawn Point",
					ultimateSpawner2D.targetTransform, typeof(Transform), true);
			
			} else if (ultimateSpawner2D.spawnAt == SpawnAt.Spawner) {
				EditorGUILayout.HelpBox("The object will spawn at UltimateSpawner's position", MessageType.Info, true);
			}
			
		}
		
	}
}