using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UltimateSpawner {

	public class USSetup : MonoBehaviour {
		static List<string> defaultEnums = new List<string>{"Fixed", "RandomFixed", "RandomRange"};
		static List<string> defaultLists = new List<string>{"SpawnPointEnum", "PositionEnum"};
		
//		[MenuItem("Tools/UltimateSpawner/Create Enums File")]
		static void CreateBasicEnums() {

			string path = "Assets/UltimateSpawner/Core/Resources/ConfigFiles/EnumValues/";
			string extension = ".asset";

			foreach (var defaultEnum in defaultEnums) {
				if (AssetDatabase.LoadAssetAtPath(path + defaultEnum + extension, typeof(ScriptableUSEnum)) == null) {
					ScriptableUSEnum scriptableEnum = ScriptableObject.CreateInstance<ScriptableUSEnum>();
					scriptableEnum.name = defaultEnum;
					AssetDatabase.CreateAsset(scriptableEnum, path + defaultEnum + extension);
					
					// Print the path of the created asset
					Debug.Log(AssetDatabase.GetAssetPath(scriptableEnum));
				}
			}
			
		}
		
		static void CreateBasicLists() {

			string path = "Assets/UltimateSpawner/Core/Resources/ConfigFiles/EnumLists/";
			string extension = ".asset";

			foreach (var defaultList in defaultLists) {
				if (AssetDatabase.LoadAssetAtPath(path + defaultList + extension, typeof(ScriptableUSEnumList)) == null) {
					ScriptableUSEnumList defaultEnumList = ScriptableObject.CreateInstance<ScriptableUSEnumList>();
					defaultEnumList.name = defaultList;
					
					AssetDatabase.CreateAsset(defaultEnumList, path + defaultList + extension);

					// Print the path of the created asset
					Debug.Log(AssetDatabase.GetAssetPath(defaultEnumList));
				}
			}
			
		}
		
		[MenuItem("Tools/UltimateSpawner/Restore Default Config")]
		static void RestoreDefaultConfig() {
			
			CreateBasicLists();
			CreateBasicEnums();
			
			// Get Lists
			string listsPath = "Assets/UltimateSpawner/Core/Resources/ConfigFiles/EnumLists/";
			string extension = ".asset";
			
			List<ScriptableUSEnumList> lists = new List<ScriptableUSEnumList>();
			
			foreach (var defaultList in defaultLists) {
				
				lists.Add((ScriptableUSEnumList)AssetDatabase.LoadAssetAtPath(listsPath + defaultList + extension, typeof(ScriptableUSEnumList)));
			}
			
			// Get Enum
			string enumsPath = "Assets/UltimateSpawner/Core/Resources/ConfigFiles/EnumValues/";
			
			List<ScriptableUSEnum> enumValues = new List<ScriptableUSEnum>();
			
			foreach (var defaultEnum in defaultEnums) {
				enumValues.Add((ScriptableUSEnum)AssetDatabase.LoadAssetAtPath(enumsPath + defaultEnum + extension, typeof(ScriptableUSEnum)));
				
			}
			
			// Add each enum to each list
			for (int listIndex = 0; listIndex < lists.Count; listIndex++) {
				lists[listIndex].list = new List<ScriptableUSEnum>();
				for (int enumIndex = 0; enumIndex < enumValues.Count; enumIndex++) {

					if (lists[listIndex].name == "SpawnPointEnum") {
						if(enumValues[enumIndex].name == "Fixed" || enumValues[enumIndex].name == "RandomFixed")
							lists[listIndex].list.Add(enumValues[enumIndex]);
					}
					
					if (lists[listIndex].name == "PositionEnum") {
						if(enumValues[enumIndex].name == "Fixed" || enumValues[enumIndex].name == "RandomFixed" || enumValues[enumIndex].name == "RandomRange")
							lists[listIndex].list.Add(enumValues[enumIndex]);
					}
					
				}
				
			}

		}
	}
}