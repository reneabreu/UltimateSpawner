using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSpawner2D {

	public class UltimateSpawner2D : MonoBehaviour {

		#region Spawner Basic Settings

		[Tooltip("Choose a Object to spawn")] 
		public GameObject objectToSpawn;
		[Tooltip("Should we use object pooling?")] 
		public bool usePoolSystem;
		
		public SpawnMode spawnMode;

		// Show/Hide Debug Logs (I hate spammed logs)
		public bool ShowDebugMessages;

		#endregion

		#region Spawner Advanced Settings

		public KeyCode inputKeyCode;

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

		#region Timer Setup

		// TODO: Configure first spawn to work with some trigger too
		[Tooltip("Time since script activation to spawn first object")] 
		public float firstSpawnTime;
		
		// Fixed Configuration
		[Tooltip("Time between spawns. (after first spawn)")] 
		public float fixedDelayBetweenSpawns;

		// Random Configuration
		public float minDelayBetweenSpawns;
		public float maxDelayBetweenSpawns;		

		// Progressive Configuration
		[Tooltip("Time since script activation to spawn first object")] 
		public float startingDelayBetweenSpawns;
		[Tooltip("")]
		public float delayModifier;
		[Tooltip("Since we use a countdown timer remember to use a value minor than the Starting Delay but greater than 0")] 
		public float progressiveDelayLimit;
			
		#endregion

		#region UnityCalls

		void Awake() {
			if(usePoolSystem)
				StartPool();
			
			if (spawnMode == SpawnMode.FixedTime || spawnMode == SpawnMode.ProgressiveTime ||
			    spawnMode == SpawnMode.RandomTime)
				SetupTimer();
		}

		void Update() {

			if (spawnMode == SpawnMode.Input) {
				if (Input.GetKeyDown(inputKeyCode))
					Spawn();
			}

			if (spawnMode == SpawnMode.FixedTime || spawnMode == SpawnMode.ProgressiveTime ||
			    spawnMode == SpawnMode.RandomTime) {
				
				timeLeft -= Time.deltaTime;
				if (timeLeft < 0) {
					timeLeft = interval;
					Elapsed.Invoke();
				}
			}
			
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
			
			if(Application.isEditor && ShowDebugMessages)
				Debug.Log("Pool created!");
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

			// Activate Pool Object
			if (usePoolSystem) {
				// Get Object in Pool
				currentPoolGameObject = GetNextObject();

				// Activate it
				if (currentPoolGameObject != null)
					currentPoolGameObject.SetActive(true);

			}
			// Instantiate New Object
			else {
				Instantiate(objectToSpawn, Vector2.zero, Quaternion.identity);
			}
		}
		
		#endregion

		#region Timer
		
		private float interval;

		private float timeLeft;

		private delegate void TimerElapsed();

		private event TimerElapsed Elapsed;

		void SetupTimer() {
			timeLeft = interval = firstSpawnTime;

			Elapsed += Spawn;
			Elapsed += ElapsedBehaviour;
		}

		void ElapsedBehaviour() {

			switch (spawnMode) {
					case SpawnMode.FixedTime:
						interval = fixedDelayBetweenSpawns;
						break;
					case SpawnMode.ProgressiveTime:
						interval = ProgressiveTimer();
						break;
					case SpawnMode.RandomTime:
						interval = RandomTimer();
						break;
					default:
						if(Application.isEditor && ShowDebugMessages)
							Debug.Log("Elapsed was called but timer is not configured");
						break;
			}
		}

		float RandomTimer() {
			
			// Generate random number between gap
			float randomDelayBetweenSpawns = Random.Range(minDelayBetweenSpawns, maxDelayBetweenSpawns);
			
			if(Application.isEditor && ShowDebugMessages)
				Debug.Log(string.Format("The next spawn will occur in {0} seconds", randomDelayBetweenSpawns));

			return randomDelayBetweenSpawns;
		}

		float ProgressiveTimer() {
			float progressiveDelay;

			// Just in case it's reaches a number less than 0
			if (progressiveDelayLimit < 0)
				progressiveDelayLimit = 0;
			
			if (interval <= progressiveDelayLimit) {
				progressiveDelay = progressiveDelayLimit;
				
				if(Application.isEditor && ShowDebugMessages)
					Debug.Log(string.Format("The next spawn will occur in {0} seconds", progressiveDelay));
				
				return progressiveDelay;
			}

			// Reduce the delay
			progressiveDelay = interval - delayModifier;
			
			if(Application.isEditor && ShowDebugMessages)
				Debug.Log(string.Format("The next spawn will occur in {0} seconds", progressiveDelay));
			
			return progressiveDelay;
		}
		
		#endregion
	}

	public enum SpawnMode {
		FixedTime,
		ProgressiveTime,
		RandomTime,
		External,
		Input
	}
	
	
}