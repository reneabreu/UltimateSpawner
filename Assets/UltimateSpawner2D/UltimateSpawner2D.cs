using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSpawner2D {

	public class UltimateSpawner2D : MonoBehaviour {

		#region Spawner Basic Settings

		[Tooltip("Choose a Object to spawn")] public GameObject objectToSpawn;

		#endregion

		#region Spawner Advanced Settings

		public bool ShowDebugMessages;

		#endregion

		#region Pooling Setup

		// Pool Basic Settings
		public int poolSize = 20;

		private List<GameObject> objectsPool; // This is the pool
		private GameObject currentPoolGameObject; // We keep this to control our pool

		// Pool Advanced Settings
		[Tooltip("If there is no object deactivated and we need to spawn. " +
		         "Can we increase the pool size?")]
		public bool canIncreasePoolSize;

		[Tooltip("What's the maximum size we can expand?")]
		public int poolMaxSize;

		#endregion

		#region UnityCalls

		void Awake() {
			StartPool();
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Space))
				Spawn();
		}

		#endregion

		#region Pooling

		void StartPool() {

			// Initialize Pool Object
			currentPoolGameObject = new GameObject();
			
			// Initialize Pool
			objectsPool = new List<GameObject>();
			
			// Spawn Pool
			for (int i = 0; i < poolSize; i++) {
				GameObject poolObject = Instantiate(objectToSpawn);
				poolObject.SetActive(false);
				objectsPool.Add(poolObject);
			}
		}
		
		GameObject GetNextObject() {
		
			// Check for the next available object in pool
			for (int i = 0; i < objectsPool.Count; i++) {
				if (!objectsPool[i].activeInHierarchy) {
					if(Application.isEditor && ShowDebugMessages)
						Debug.Log(string.Format("Object {0} in pool was choosen", i));
					
					return objectsPool[i];
				}
			}

			// If there is no object available in pool and we can icrease the pool
			// Spawn a new object
			if (canIncreasePoolSize && objectsPool.Count < poolMaxSize) {
				if(Application.isEditor && ShowDebugMessages)
					Debug.Log("Pool reached max size! Adding another object to the pool!");
				
				GameObject newPoolObject = Instantiate(objectToSpawn);
				objectToSpawn.SetActive(false);
				return newPoolObject;
			}
			else {
				if(Application.isEditor && ShowDebugMessages)
					Debug.Log("Pool reached max size! Unfortunately it reached it's maximum size.");
			}

			return null;
		}

		#endregion
		
		#region Spawn

		public void Spawn() {
			
			// Get Object in Pool
			currentPoolGameObject = GetNextObject();
			
			// Activate it
			currentPoolGameObject.SetActive(true);
		}
		
		#endregion
	}

	public enum SpawnMode {
		FixedTime,
		ProgressiveTime,
		RandomTime
	}
	
	
}