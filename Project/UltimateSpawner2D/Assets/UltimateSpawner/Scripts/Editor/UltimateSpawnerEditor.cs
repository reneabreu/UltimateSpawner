using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UltimateSpawner {
	[CustomEditor(typeof(UltimateSpawner))]
	public class UltimateSpawnerEditor : Editor {
		
		private UltimateSpawner ultimateSpawner;

		SerializedProperty spawnPointsList;
		
		SerializedProperty customTransform;
		
		public override void OnInspectorGUI() {
			serializedObject.Update();
			
			ultimateSpawner = (UltimateSpawner) target;
			
			GUILayout.Label("Basic Settings", EditorStyles.boldLabel);
						
//			EditorGUILayout.PropertyField(background, new GUIContent("Background Color"));
			
			ultimateSpawner.objectToSpawn = (GameObject) EditorGUILayout.ObjectField("Object to Spawn",
				ultimateSpawner.objectToSpawn, typeof(GameObject), true);
			
			ultimateSpawner.objectType =
				(ObjectType) EditorGUILayout.EnumPopup("Object is", ultimateSpawner.objectType);
						
			GUILayout.Space(10);

			ultimateSpawner.ShowDebugMessages = EditorGUILayout.Toggle("Show Debug Log?", ultimateSpawner.ShowDebugMessages);

			// Line Divider		
			GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
			GUILayout.Space(5);

			GUILayout.Label("Spawn Settings", EditorStyles.boldLabel);
			ultimateSpawner.spawnMode =
				(SpawnMode) EditorGUILayout.EnumPopup("Spawn Mode", ultimateSpawner.spawnMode);
			
			if (ultimateSpawner.spawnMode == SpawnMode.Input) {
				
				GUILayout.Space(10);
			
				GUILayout.Label("Input Settings", EditorStyles.boldLabel);
				
				ultimateSpawner.inputKeyCode =
					(KeyCode) EditorGUILayout.EnumPopup("Input KeyCode", ultimateSpawner.inputKeyCode);
				
				// Line Divider		
				GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
			} else if (ultimateSpawner.spawnMode == SpawnMode.External) {
				
				EditorGUILayout.HelpBox("Currently you can call the public method Spawn() from another script", MessageType.Info, true);
				
				// Line Divider		
				GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				
			} else {
				ShowTimerSettings();
			}
			
			ShowPositionSettings();

			ShowRotationSettings();
			
			GUILayout.Space(5);
			GUILayout.Label("Pool Settings", EditorStyles.boldLabel);
			
			ultimateSpawner.usePoolSystem = EditorGUILayout.Toggle("Use Pooling System?", ultimateSpawner.usePoolSystem);
			if(ultimateSpawner.usePoolSystem)
				ShowPoolSettings();
			
			serializedObject.ApplyModifiedProperties();
		}

		void ShowPoolSettings() {

			GUILayout.Space(10);

			ultimateSpawner.poolSize = EditorGUILayout.IntField("Pool Size", ultimateSpawner.poolSize);
			
			ultimateSpawner.canIncreasePoolSize = EditorGUILayout.Toggle("Can increase pool size?", ultimateSpawner.canIncreasePoolSize);

			if (ultimateSpawner.poolMaxSize < ultimateSpawner.poolSize)
				ultimateSpawner.poolMaxSize = ultimateSpawner.poolSize + 1;
			
			if(ultimateSpawner.canIncreasePoolSize)
				ultimateSpawner.poolMaxSize = EditorGUILayout.IntField("Pool Maximum Size", ultimateSpawner.poolMaxSize);

		}

		void ShowTimerSettings() {
			GUILayout.Space(10);
			
			GUILayout.Label("Timer Settings", EditorStyles.boldLabel);

			ultimateSpawner.firstSpawnTime = EditorGUILayout.FloatField("First Spawn Time", ultimateSpawner.firstSpawnTime);

			GUILayout.Space(5);

			
			if (ultimateSpawner.spawnMode == SpawnMode.FixedTime) {
				GUILayout.Label("Fixed Time Settings");
				ultimateSpawner.fixedDelayBetweenSpawns = EditorGUILayout.FloatField("Delay Between Spawns", ultimateSpawner.fixedDelayBetweenSpawns);
				
			} else if (ultimateSpawner.spawnMode == SpawnMode.RandomTime) {
				GUILayout.Label("Random Time Settings");
				ultimateSpawner.minDelayBetweenSpawns = EditorGUILayout.FloatField("Maximum Delay Between Spawns", ultimateSpawner.minDelayBetweenSpawns);
				ultimateSpawner.maxDelayBetweenSpawns = EditorGUILayout.FloatField("Minimum Delay Between Spawns", ultimateSpawner.maxDelayBetweenSpawns);
				
			} else if (ultimateSpawner.spawnMode == SpawnMode.ProgressiveTime) {
				GUILayout.Label("Progressive Time Settings");
				ultimateSpawner.startingDelayBetweenSpawns = EditorGUILayout.FloatField("Starting Delay Between Spawns", ultimateSpawner.startingDelayBetweenSpawns);
				ultimateSpawner.delayModifier = EditorGUILayout.FloatField("Delay Modifier", ultimateSpawner.delayModifier);
				ultimateSpawner.progressiveDelayLimit = EditorGUILayout.FloatField("Delay Limit", ultimateSpawner.progressiveDelayLimit);

				if (ultimateSpawner.progressiveDelayLimit <= 0)
					ultimateSpawner.progressiveDelayLimit = 0;
			}

			// Line Divider		
			GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		}

		void ShowPositionSettings() {
			
			spawnPointsList = serializedObject.FindProperty("spawnPoints");
		
			GUILayout.Space(5);
			
			GUILayout.Label("Position Settings", EditorStyles.boldLabel);
		
			ultimateSpawner.spawnAt =
				(SpawnAt) EditorGUILayout.EnumPopup("Spawn At", ultimateSpawner.spawnAt);
		
			
			if (ultimateSpawner.spawnAt == SpawnAt.Fixed) {
				GUILayout.Label("Fixed Spawn Point");
				
				ultimateSpawner.fixedPosition = (PositionToSpawn) EditorGUILayout.ObjectField("Spawn Point",
					ultimateSpawner.fixedPosition, typeof(PositionToSpawn), true);
				
			} else if (ultimateSpawner.spawnAt == SpawnAt.Random) {
				
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

			} else if (ultimateSpawner.spawnAt == SpawnAt.TargetTransform) {
				GUILayout.Label("Transform Spawn Point");
					
				ultimateSpawner.targetTransform = (Transform) EditorGUILayout.ObjectField("Spawn Point",
					ultimateSpawner.targetTransform, typeof(Transform), true);
			
			} else if (ultimateSpawner.spawnAt == SpawnAt.Spawner) {
				EditorGUILayout.HelpBox("The object will spawn at UltimateSpawner's position", MessageType.Info, true);
			}
			
			// Line Divider		
			GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		}

		void ShowRotationSettings() {
						
			GUILayout.Space(5);
			
			GUILayout.Label("Rotation Settings", EditorStyles.boldLabel);
		
			ultimateSpawner.spawnRotation =
				(SpawnRotation) EditorGUILayout.EnumPopup("Spawn Rotation", ultimateSpawner.spawnRotation);
			
			if (ultimateSpawner.spawnRotation == SpawnRotation.Identity) {
				EditorGUILayout.HelpBox("The object will spawn with a rotation equals Quaternion.identity", MessageType.Info, true);
			} else if (ultimateSpawner.spawnRotation == SpawnRotation.ObjectOwnRotation) {
				EditorGUILayout.HelpBox("The object will spawn with it's current rotation", MessageType.Info, true);
			} else if (ultimateSpawner.spawnRotation == SpawnRotation.Spawner) {
				EditorGUILayout.HelpBox("The object will spawn with the spawner's rotation ", MessageType.Info, true);
			} else if (ultimateSpawner.spawnRotation == SpawnRotation.Custom) {
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Custom Rotation");

				EditorGUIUtility.labelWidth = 15f;
				ultimateSpawner.customRotationX = EditorGUILayout.FloatField("X", ultimateSpawner.customRotationX);
				ultimateSpawner.customRotationY = EditorGUILayout.FloatField("Y", ultimateSpawner.customRotationY);
				ultimateSpawner.customRotationZ = EditorGUILayout.FloatField("Z", ultimateSpawner.customRotationZ);
				EditorGUIUtility.labelWidth = 0f;
				
				EditorGUILayout.EndHorizontal();
			}
			
			// Line Divider		
			GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		}
		
	}
}