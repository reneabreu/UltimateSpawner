using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UltimateSpawnerSystem {
	[CustomEditor(typeof(UltimateSpawner))]
	public class UltimateSpawnerEditor : Editor {
		
		private UltimateSpawner ultimateSpawner;

//		SerializedProperty spawnPointsList;
//		// Position Lists
//		SerializedProperty randomFixedX;
//		SerializedProperty randomFixedY;
//		SerializedProperty randomFixedZ;
		
		private ScriptableUSEnum Fixed, RandomFixed, RandomRange;

		void OnEnable() {
			Fixed = Resources.Load<ScriptableUSEnum>("ConfigFiles/EnumValues/Fixed");
			RandomFixed = Resources.Load<ScriptableUSEnum>("ConfigFiles/EnumValues/RandomFixed");
			RandomRange = Resources.Load<ScriptableUSEnum>("ConfigFiles/EnumValues/RandomRange");

		}
		
		public override void OnInspectorGUI() {
			
			ultimateSpawner = (UltimateSpawner) target;
			
			serializedObject.Update();

			GUILayout.Label("Basic Settings", EditorStyles.boldLabel);
						
//			EditorGUILayout.PropertyField(background, new GUIContent("Background Color"));
			
			ultimateSpawner.objectToSpawn = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Object to Spawn","Choose an object to spawn"),
				ultimateSpawner.objectToSpawn, typeof(GameObject), true);
						
			GUILayout.Space(10);

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Debug " + (ultimateSpawner.ShowDebugMessages ? "On" : "Off")))
			{
				ultimateSpawner.ShowDebugMessages = !ultimateSpawner.ShowDebugMessages;
			}
			if(GUILayout.Button("Pooling System " + (ultimateSpawner.usePoolSystem ? "On" : "Off")))
			{
				ultimateSpawner.usePoolSystem = !ultimateSpawner.usePoolSystem;
			}
			GUILayout.EndHorizontal();

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

			ShowMovementSettings();
			
			ShowPoolSettings();
			
//			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

//			EditorFix.SetObjectDirty(ultimateSpawner);

			serializedObject.ApplyModifiedProperties();

			// Little Fix to Set Scene Dirty if anything has changed
			if (GUI.changed && !Application.isPlaying) {
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());				
			}
		}

		void ShowPoolSettings() {
			if (ultimateSpawner.usePoolSystem) {

				// Line Divider		
				GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				GUILayout.Space(5);
				GUILayout.Label("Pool Settings", EditorStyles.boldLabel);

				ultimateSpawner.poolSize = EditorGUILayout.IntField("Pool Size", ultimateSpawner.poolSize);

				ultimateSpawner.canIncreasePoolSize =
					EditorGUILayout.Toggle("Can increase pool size?", ultimateSpawner.canIncreasePoolSize);

				if (ultimateSpawner.poolMaxSize < ultimateSpawner.poolSize)
					ultimateSpawner.poolMaxSize = ultimateSpawner.poolSize + 1;

				if (ultimateSpawner.canIncreasePoolSize)
					ultimateSpawner.poolMaxSize = EditorGUILayout.IntField("Pool Maximum Size", ultimateSpawner.poolMaxSize);

			}
		}

		void ShowTimerSettings() {
			GUILayout.Space(10);
			
			GUILayout.Label("Timer Settings", EditorStyles.boldLabel);

			ultimateSpawner.firstSpawnTime = EditorGUILayout.FloatField("First Spawn Time", ultimateSpawner.firstSpawnTime);

			GUILayout.Space(5);

			
			if (ultimateSpawner.spawnMode == SpawnMode.FixedTime) {
				GUILayout.Label("Fixed Time Settings");
				ultimateSpawner.fixedDelayBetweenSpawns = EditorGUILayout.FloatField("Seconds Between Spawns", ultimateSpawner.fixedDelayBetweenSpawns);
				
			} else if (ultimateSpawner.spawnMode == SpawnMode.RandomTime) {
				GUILayout.Label("Random Time Settings");
				ultimateSpawner.minDelayBetweenSpawns = EditorGUILayout.FloatField("Min Time Between Spawns", ultimateSpawner.minDelayBetweenSpawns);
				ultimateSpawner.maxDelayBetweenSpawns = EditorGUILayout.FloatField("Max Time Between Spawns", ultimateSpawner.maxDelayBetweenSpawns);
				
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
		
		string[] spawnPointStrings, positionStrings, transformStrings;
		
		bool showX, showY, showZ;

		void ShowPositionSettings() {
			
			var spawnPointsList = serializedObject.FindProperty("randomSpawnPoints");
			var targetTransformList = serializedObject.FindProperty("targetTransformList");
			var randomFixedX = serializedObject.FindProperty("randomFixedX");
			var randomFixedY = serializedObject.FindProperty("randomFixedY");
			var randomFixedZ = serializedObject.FindProperty("randomFixedZ");
		
			GUILayout.Space(5);
			
			GUILayout.Label("Position Settings", EditorStyles.boldLabel);
		
			ultimateSpawner.spawnAt =
				(SpawnAt) EditorGUILayout.EnumPopup("Spawn At", ultimateSpawner.spawnAt);

			if (ultimateSpawner.spawnAt == SpawnAt.SpawnPoint || ultimateSpawner.spawnAt == SpawnAt.Position)
				ultimateSpawner.showGizmos = EditorGUILayout.Toggle("Show Gizmos?", ultimateSpawner.showGizmos);
			
			if (ultimateSpawner.spawnAt == SpawnAt.SpawnPoint) {
				
				if (ultimateSpawner.spawnPointEnum != null) {

					spawnPointStrings = new string[ultimateSpawner.spawnPointEnum.list.Count];
					for (int i = 0; i < ultimateSpawner.spawnPointEnum.list.Count; i++) {
						spawnPointStrings[i] = ultimateSpawner.spawnPointEnum.list[i].name;
					}
					
					GUILayout.Space(10);
			
					GUILayout.Label("SpawnPoint Settings", EditorStyles.boldLabel);
					ultimateSpawner.selectedSpawnPointEnum = EditorGUILayout.Popup("SpawnPoint Type",ultimateSpawner.selectedSpawnPointEnum, spawnPointStrings);
				}
				else {
					ultimateSpawner.SetEnum();
				}
				
				

				if (ultimateSpawner.spawnPointEnum.list[ultimateSpawner.selectedSpawnPointEnum] == Fixed) {

					ultimateSpawner.fixedSpawnPoint = (SpawnPoint) EditorGUILayout.ObjectField("Fixed Spawn Point",
						ultimateSpawner.fixedSpawnPoint, typeof(SpawnPoint), true);
				} else if (ultimateSpawner.spawnPointEnum.list[ultimateSpawner.selectedSpawnPointEnum] == RandomFixed) {
					
					EditorGUILayout.PropertyField(spawnPointsList, new GUIContent("Random Spawn Points"));
				
					// List
					EditorGUI.indentLevel += 1;
					if (spawnPointsList.isExpanded) {
						EditorGUILayout.PropertyField(spawnPointsList.FindPropertyRelative("Array.size"));
						for (int i = 0; i < spawnPointsList.arraySize; i++) {
							EditorGUILayout.PropertyField(spawnPointsList.GetArrayElementAtIndex(i));
						}
					}
					spawnPointsList.serializedObject.ApplyModifiedProperties();
					EditorGUI.indentLevel -= 1;
					
				}

			} else if (ultimateSpawner.spawnAt == SpawnAt.Position) {
				
				GUILayout.Space(10);
			
				GUILayout.Label("Position Settings", EditorStyles.boldLabel);
	
				// X
				showX = EditorGUILayout.Foldout(showX, "X Setup");
				if (showX) {

					if (ultimateSpawner.positionEnum != null) {

						positionStrings = new string[ultimateSpawner.positionEnum.list.Count];
						for (int i = 0; i < ultimateSpawner.positionEnum.list.Count; i++) {
							positionStrings[i] = ultimateSpawner.positionEnum.list[i].name;
						}

						ultimateSpawner.selectedXEnum = EditorGUILayout.Popup("X Type", ultimateSpawner.selectedXEnum, positionStrings);
					}
					else {
						ultimateSpawner.SetEnum();
					}

					if (ultimateSpawner.positionEnum.list[ultimateSpawner.selectedXEnum] == Fixed)
						ultimateSpawner.fixedX = EditorGUILayout.FloatField("Fixed X Position", ultimateSpawner.fixedX);
					else if (ultimateSpawner.positionEnum.list[ultimateSpawner.selectedXEnum] == RandomFixed) {
						EditorGUI.indentLevel += 1;
						EditorGUILayout.PropertyField(randomFixedX, new GUIContent("Random Fixed X"));
						// List
						EditorGUI.indentLevel += 1;
						if (randomFixedX.isExpanded) {
							EditorGUILayout.PropertyField(randomFixedX.FindPropertyRelative("Array.size"));
							for (int i = 0; i < randomFixedX.arraySize; i++) {
								EditorGUILayout.PropertyField(randomFixedX.GetArrayElementAtIndex(i));
							}
						}
						randomFixedX.serializedObject.ApplyModifiedProperties();

						EditorGUI.indentLevel -= 1;
						EditorGUI.indentLevel -= 1;

					}
					else if (ultimateSpawner.positionEnum.list[ultimateSpawner.selectedXEnum] == RandomRange) {

						EditorGUILayout.BeginHorizontal();
						GUILayout.Label("X Random Range");
						EditorGUIUtility.labelWidth = 30f;
						ultimateSpawner.randomRangeMinX = EditorGUILayout.FloatField("Min", ultimateSpawner.randomRangeMinX);
						ultimateSpawner.randomRangeMaxX = EditorGUILayout.FloatField("Max", ultimateSpawner.randomRangeMaxX);
						EditorGUIUtility.labelWidth = 0f;
						EditorGUILayout.EndHorizontal();

					}
				}

				// Y
				showY = EditorGUILayout.Foldout(showY, "Y Setup");
				if (showY) {

					if (ultimateSpawner.positionEnum != null) {

						positionStrings = new string[ultimateSpawner.positionEnum.list.Count];
						for (int i = 0; i < ultimateSpawner.positionEnum.list.Count; i++) {
							positionStrings[i] = ultimateSpawner.positionEnum.list[i].name;
						}

						ultimateSpawner.selectedYEnum = EditorGUILayout.Popup("Y Type", ultimateSpawner.selectedYEnum, positionStrings);
					}
					else {
						ultimateSpawner.SetEnum();
					}

					if (ultimateSpawner.positionEnum.list[ultimateSpawner.selectedYEnum] == Fixed)
						ultimateSpawner.fixedY = EditorGUILayout.FloatField("Fixed Y Position", ultimateSpawner.fixedY);
					else if (ultimateSpawner.positionEnum.list[ultimateSpawner.selectedYEnum] == RandomFixed) {
						EditorGUI.indentLevel += 1;
						EditorGUILayout.PropertyField(randomFixedY, new GUIContent("Random Fixed Y"));
						// List
						EditorGUI.indentLevel += 1;
						if (randomFixedY.isExpanded) {
							EditorGUILayout.PropertyField(randomFixedY.FindPropertyRelative("Array.size"));
							for (int i = 0; i < randomFixedY.arraySize; i++) {
								EditorGUILayout.PropertyField(randomFixedY.GetArrayElementAtIndex(i));
							}
						}
						randomFixedY.serializedObject.ApplyModifiedProperties();

						EditorGUI.indentLevel -= 1;
						EditorGUI.indentLevel -= 1;

					}
					else if (ultimateSpawner.positionEnum.list[ultimateSpawner.selectedYEnum] == RandomRange) {

						EditorGUILayout.BeginHorizontal();
						GUILayout.Label("Y Random Range");
						EditorGUIUtility.labelWidth = 30f;
						ultimateSpawner.randomRangeMinY = EditorGUILayout.FloatField("Min", ultimateSpawner.randomRangeMinY);
						ultimateSpawner.randomRangeMaxY = EditorGUILayout.FloatField("Max", ultimateSpawner.randomRangeMaxY);
						EditorGUIUtility.labelWidth = 0f;
						EditorGUILayout.EndHorizontal();

					}

				}

				// Z
				showZ = EditorGUILayout.Foldout(showZ, "Z Setup");
				if (showZ) {
			
					if (ultimateSpawner.positionEnum != null) {

						positionStrings = new string[ultimateSpawner.positionEnum.list.Count];
						for (int i = 0; i < ultimateSpawner.positionEnum.list.Count; i++) {
							positionStrings[i] = ultimateSpawner.positionEnum.list[i].name;
						}
			
						ultimateSpawner.selectedZEnum = EditorGUILayout.Popup("Z Type",ultimateSpawner.selectedZEnum, positionStrings);
					}
					else {
						ultimateSpawner.SetEnum();
					}
			
					if(ultimateSpawner.positionEnum.list[ultimateSpawner.selectedZEnum] == Fixed)
						ultimateSpawner.fixedZ = EditorGUILayout.FloatField("Fixed Z Position", ultimateSpawner.fixedZ);
					else if (ultimateSpawner.positionEnum.list[ultimateSpawner.selectedXEnum] == RandomFixed) {
						EditorGUI.indentLevel += 1;
						EditorGUILayout.PropertyField(randomFixedZ, new GUIContent("Random Fixed Z"));
						// List
						EditorGUI.indentLevel += 1;
						if (randomFixedZ.isExpanded) {
							EditorGUILayout.PropertyField(randomFixedZ.FindPropertyRelative("Array.size"));
							for (int i = 0; i < randomFixedZ.arraySize; i++) {
								EditorGUILayout.PropertyField(randomFixedZ.GetArrayElementAtIndex(i));
							}
						}
						randomFixedY.serializedObject.ApplyModifiedProperties();

						EditorGUI.indentLevel -= 1;
						EditorGUI.indentLevel -= 1;

					} else if (ultimateSpawner.positionEnum.list[ultimateSpawner.selectedZEnum] == RandomRange) {

						EditorGUILayout.BeginHorizontal();
						GUILayout.Label("Z Random Range");
						EditorGUIUtility.labelWidth = 30f;
						ultimateSpawner.randomRangeMinZ = EditorGUILayout.FloatField("Min", ultimateSpawner.randomRangeMinZ);
						ultimateSpawner.randomRangeMaxZ = EditorGUILayout.FloatField("Max", ultimateSpawner.randomRangeMaxZ);
						EditorGUIUtility.labelWidth = 0f;
						EditorGUILayout.EndHorizontal();

					}

				}

			} else if (ultimateSpawner.spawnAt == SpawnAt.TargetTransform) {

				if (ultimateSpawner.spawnPointEnum != null) {

					transformStrings = new string[ultimateSpawner.spawnPointEnum.list.Count];
					for (int i = 0; i < ultimateSpawner.spawnPointEnum.list.Count; i++) {
						transformStrings[i] = ultimateSpawner.spawnPointEnum.list[i].name;
					}
					
					GUILayout.Space(10);
			
					GUILayout.Label("Transform Settings", EditorStyles.boldLabel);
					ultimateSpawner.selectedTransformEnum = EditorGUILayout.Popup("SpawnPoint Type",ultimateSpawner.selectedTransformEnum, transformStrings);
				}
				else {
					ultimateSpawner.SetEnum();
				}

				if (ultimateSpawner.spawnPointEnum.list[ultimateSpawner.selectedTransformEnum] == Fixed) {

					ultimateSpawner.targetTransform = (Transform) EditorGUILayout.ObjectField("Spawn Point",
					ultimateSpawner.targetTransform, typeof(Transform), true);
				} else if (ultimateSpawner.spawnPointEnum.list[ultimateSpawner.selectedTransformEnum] == RandomFixed) {
				
				EditorGUILayout.PropertyField(targetTransformList, new GUIContent("List of Target's Transforms"));
				
					// List
					EditorGUI.indentLevel += 1;
					if (targetTransformList.isExpanded) {
						EditorGUILayout.PropertyField(targetTransformList.FindPropertyRelative("Array.size"));
						for (int i = 0; i < targetTransformList.arraySize; i++) {
							EditorGUILayout.PropertyField(targetTransformList.GetArrayElementAtIndex(i));
						}
					}
					targetTransformList.serializedObject.ApplyModifiedProperties();
					EditorGUI.indentLevel -= 1;
					
				}
			
			} else if (ultimateSpawner.spawnAt == SpawnAt.Spawner) {
				EditorGUILayout.HelpBox("The object will spawn at UltimateSpawner's position", MessageType.Info, true);
			}
			
			
			// Line Divider		
			GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
			
//			spawnPointsList.serializedObject.Update();
//			randomFixedX.serializedObject.Update();
//			randomFixedY.serializedObject.Update();
//			randomFixedZ.serializedObject.Update();
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

		void ShowMovementSettings() {

			GUILayout.Space(5);

			GUILayout.Label("Movement Settings", EditorStyles.boldLabel);

			ultimateSpawner.movementType =
				(MovementType) EditorGUILayout.EnumPopup("Movement Type", ultimateSpawner.movementType);

			if (ultimateSpawner.movementType != MovementType.None){
				GUILayout.Space(10);
	
				ultimateSpawner.objectType =
					(ObjectType) EditorGUILayout.EnumPopup("Object is", ultimateSpawner.objectType);
			}
		if (ultimateSpawner.movementType == MovementType.Force) {

				if (ultimateSpawner.objectType == ObjectType._2D) {
					ultimateSpawner.forceMode2D =
						(ForceMode2D) EditorGUILayout.EnumPopup("Force Mode", ultimateSpawner.forceMode2D);

					ultimateSpawner.force2D = EditorGUILayout.Vector2Field("Force", ultimateSpawner.force2D);
				}
				else {
					ultimateSpawner.forceMode =
						(ForceMode) EditorGUILayout.EnumPopup("Force Mode", ultimateSpawner.forceMode);
					
					ultimateSpawner.force3D = EditorGUILayout.Vector3Field("Force", ultimateSpawner.force3D);
				}


			} else if (ultimateSpawner.movementType == MovementType.Velocity) {
				if (ultimateSpawner.objectType == ObjectType._2D) {

					ultimateSpawner.velocity2D = EditorGUILayout.Vector2Field("Velocity", ultimateSpawner.velocity2D);
				}
				else {
					
					ultimateSpawner.velocity3D = EditorGUILayout.Vector3Field("Velocity", ultimateSpawner.velocity3D);

				}
			}
			
		}
	}
}