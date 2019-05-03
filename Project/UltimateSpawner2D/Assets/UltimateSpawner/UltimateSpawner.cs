using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEditor;
using UnityEngine;

namespace UltimateSpawnerSystem {
	[HelpURL("https://github.com/reneabreu/UltimateSpawner")]
	public class UltimateSpawner : MonoBehaviour {

		#region UltimateSpawner Config
		
		private ScriptableUSEnum Fixed, RandomFixed, RandomRange;
		
		#endregion

		#region Spawned Object Settings

		private GameObject latestSpawnedObject;
		private int totalSpawns;
		
		#endregion
		
		#region Spawner Basic Settings

		[Tooltip("Choose a Object to spawn")] 
		public GameObject objectToSpawn;
		
		public SpawnMode spawnMode;

		// Show/Hide Debug Logs (I hate spammed logs)
		public bool ShowDebugMessages;

		#endregion

		#region Spawn Mode Settings
		
		// Input
		public KeyCode inputKeyCode;

		#endregion

		#region Pooling Setup
		
//		[Tooltip("Should we use object pooling?")] 
		public bool usePoolSystem;

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
		[Tooltip("Time between spawns. (after first spawn)")] 
		public float startingDelayBetweenSpawns;
		[Tooltip("")]
		public float delayModifier;
		[Tooltip("Since we use a countdown timer remember to use a value minor than the Starting Delay but greater than 0")] 
		public float progressiveDelayLimit;
			
		#endregion

		#region Position Setup
		
		// Basic Setup
		public SpawnAt spawnAt;

		public bool showGizmos;

		// Advanced setup
		
		/** SpawnPoint **/
		public ScriptableUSEnumList spawnPointEnum;
		public int selectedSpawnPointEnum = 0;
		
		// Fixed
		public SpawnPoint fixedSpawnPoint;
		
		// Random Fixed
		public List<SpawnPoint> randomSpawnPoints;
		
		/** Position **/
		public ScriptableUSEnumList positionEnum;
		public int selectedPositionEnum;
		
		// X
		public int selectedXEnum;
		public float fixedX;
		public List<float> randomFixedX;
		public float randomRangeMinX;
		public float randomRangeMaxX;
		
		// Y
		public int selectedYEnum;
		public float fixedY;
		public List<float> randomFixedY;
		public float randomRangeMinY;
		public float randomRangeMaxY;
		
		// Z
		public int selectedZEnum;
		public float fixedZ;
		public List<float> randomFixedZ;
		public float randomRangeMinZ;
		public float randomRangeMaxZ;
		
		// Target Transform
		public int selectedTransformEnum;
		public Transform targetTransform;
		public List<Transform> targetTransformList;
		
		// External
		public Vector3 externalPosition;
		
		#endregion

		#region Rotation Setup

		private Transform rotationTransform;
		
		public SpawnRotation spawnRotation;

		public float customRotationX;
		public float customRotationY;
		public float customRotationZ;

		#endregion

		#region Movement Setup
		
		public ObjectType objectType;

		public MovementType movementType;
		
		// 2D
		public ForceMode2D forceMode2D;
		public Vector2 force2D;
		public Vector2 velocity2D;
		
		// 3D
		public ForceMode forceMode;
		public Vector3 force3D;
		public Vector3 velocity3D;
		
		#endregion

		#region UnityCalls
		
		#if UNITY_EDITOR
		private void OnValidate() {
			SetEnum();
		}
		#endif

		void Awake() {
			
			SetEnum();
			
			#if UNITY_EDITOR
			ShowUltimateSpawnerSetUp();
			#endif
			
			// Setup Rotation Controller
			rotationTransform = new GameObject("USRotationController").transform;
			
			if(usePoolSystem)
				StartPool();
			
			if (spawnMode == SpawnMode.FixedTime || spawnMode == SpawnMode.ProgressiveTime ||
			    spawnMode == SpawnMode.RandomTime)
				SetupTimer();

			// A little fix to prevent errors when using the pooling system
			if (poolMaxSize < poolSize)
				poolMaxSize = poolSize + 1;
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
		
		#region SpawnedObject

		public GameObject LatestSpawnedObject {
			get { return latestSpawnedObject; }
		}

		public int TotalSpawns {
			get { return totalSpawns; }
		}

		#endregion

		#region Pooling

		void StartPool() {
			
			// Before start a new pool we should check if the pool exists
			if (objectsPool != null) {
				// Yup there is a pool, but is it in the correct size?
				if (objectsPool.Count <= poolMaxSize && objectsPool.Count >= poolSize) {
					// Everything is fine, no need to create the pool again 
					return;
				} else if (objectsPool.Count < poolSize) {
					// Something went wrong and a few objects were destroyed
					// Fixing the pool
					for (int i = objectsPool.Count; i < poolSize; i++){
						GameObject poolObject = Instantiate(objectToSpawn);
						poolObject.SetActive(false);
						objectsPool.Add(poolObject);
						poolObject.AddComponent<USPoolObject>().SetUltimateSpawner(this);
					}
					#if UNITY_EDITOR
					UltimateLog("Some objects in pool were destroyed, consider disabling them instead of destroying." +
					            "\nTo prevent game crashing UltimateSpawner fixed it by replacing them.", "WARNING");
					#endif
				}
			}
			else {
				// Nope, no pool created...
				// Let's create one
				// Initialize Pool Object
				currentPoolGameObject = new GameObject();

				// Initialize Pool
				objectsPool = new List<GameObject>();

				// Spawn Pool
				for (int i = 0; i < poolSize; i++) {
					GameObject poolObject = Instantiate(objectToSpawn);
					poolObject.SetActive(false);
					objectsPool.Add(poolObject);
					poolObject.AddComponent<USPoolObject>().SetUltimateSpawner(this);
				}
				#if UNITY_EDITOR
				UltimateLog("Pool created!");
				#endif
			}
		}
		
		GameObject GetNextObject() {
			
			if (objectsPool == null) {
				#if UNITY_EDITOR
				UltimateLog("Pool was destroyed and you are trying to access it, " +
				            "to prevent errors UltimateSpawner will create a new pool");
				#endif
				StartPool();
			}

			if (objectsPool.Count < poolSize) {
				StartPool();
			}
		
			// Check for the next available object in pool
			for (int i = 0; i < objectsPool.Count; i++) {
				if (!objectsPool[i].activeInHierarchy && objectsPool[i] != null) {
					#if UNITY_EDITOR
					UltimateLog(string.Format("Object {0} in pool was choosen", i));
					#endif
					
					return objectsPool[i];
				}

				if (objectsPool[i] == null) {
					#if UNITY_EDITOR
					UltimateLog("Object in pool has been destroyed but you are still trying to access it." +
					            "To prevent errors UltimateSpawner will create a new object!", "WARNING");
					#endif
					
					objectsPool[i] = Instantiate(objectToSpawn);
					objectsPool[i].SetActive(false);
					objectsPool[i].AddComponent<USPoolObject>().SetUltimateSpawner(this);

					return objectsPool[i];

				}
			}

			// If there is no object available in pool and we can icrease the pool
			// Spawn a new object
			if (canIncreasePoolSize && objectsPool.Count < poolMaxSize) {
				#if UNITY_EDITOR
				UltimateLog("Pool reached max size! Adding another object to the pool!");
				#endif
				
				GameObject newPoolObject = Instantiate(objectToSpawn);
				newPoolObject.SetActive(false);
				newPoolObject.AddComponent<USPoolObject>().SetUltimateSpawner(this);
				
				return newPoolObject;
			}
			else {
				#if UNITY_EDITOR
				UltimateLog("Pool reached max size!");
				UltimateLog("Unfortunately the pool reached it's maximum size. " +
				            "If you still need it to increase, try to setup a bigger max size", "WARNING");
				#endif
			}

			return null;
		}

		public void RemoveObjectFromPool(GameObject objectToRemove) {
			objectsPool.Remove(objectToRemove);
			#if UNITY_EDITOR
			UltimateLog(string.Format("Object {0} was removed from pool! And now the new pool size is: {1}", objectToRemove.name, objectsPool.Count ));
			#endif
		}

		#endregion
		
		#region Spawn

		public void Spawn() {
			
			// To prevent errors it's better check if pooling system is being used and if the pool exists
			// Check if pool exists
			if (usePoolSystem && objectsPool == null) {
				#if UNITY_EDITOR
				UltimateLog("You are trying to use the pooling system, but there is no pool created. " +
				            "UltimateSpawner will create a new pool");
				#endif
				StartPool();
			}

			// Activate Pool Object
			if (usePoolSystem) {
				// Get Object in Pool
				currentPoolGameObject = GetNextObject();

				if (currentPoolGameObject != null) {
					// Activate it
					currentPoolGameObject.SetActive(true);

					// Position it
					currentPoolGameObject.transform.position = GetSpawnPosition();

					// Rotate it
					if (spawnRotation != SpawnRotation.ObjectOwnRotation)
						currentPoolGameObject.transform.rotation = GetSpawnRotation();

					// Move it
					ApplyMovement(currentPoolGameObject);

					// Setting Latest Spawned Object
					latestSpawnedObject = currentPoolGameObject;
					
					#if UNITY_EDITOR
					UltimateLog(string.Format("Spawning object {0} at position {1} with a rotation of {2}",
						currentPoolGameObject.name, currentPoolGameObject.transform.position.ToString(), 
						currentPoolGameObject.transform.rotation.eulerAngles.ToString()));
					#endif
				}
			}
			// Instantiate New Object
			else {
				GameObject instantiatedObject = Instantiate(objectToSpawn);

				// Position it
				instantiatedObject.transform.position = GetSpawnPosition();

				// Rotate it
				if (spawnRotation != SpawnRotation.ObjectOwnRotation)
					instantiatedObject.transform.rotation = GetSpawnRotation();

				// Move it
				ApplyMovement(instantiatedObject);

				// Setting Latest Spawned Object
				latestSpawnedObject = currentPoolGameObject;
				
				#if UNITY_EDITOR
				UltimateLog(string.Format("Spawning object {0} at position {1} with a rotation of {2}",
					instantiatedObject.name, instantiatedObject.transform.position.ToString(), 
					instantiatedObject.transform.rotation.eulerAngles.ToString()));
				#endif
			}

			totalSpawns++;
			
			if(Spawned != null)
				Spawned.Invoke();
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
						#if UNITY_EDITOR
						UltimateLog("Elapsed was called but timer is not configured");
						#endif
						
						break;
			}
		}

		float RandomTimer() {
			
			// Generate random number between gap
			float randomDelayBetweenSpawns = Random.Range(minDelayBetweenSpawns, maxDelayBetweenSpawns);
			
			#if UNITY_EDITOR
			UltimateLog(string.Format("The next spawn will happen in {0} seconds", randomDelayBetweenSpawns));
			#endif
			
			return randomDelayBetweenSpawns;
		}

		float ProgressiveTimer() {
			float progressiveDelay;

			// Just in case it's reaches a number less than 0
			if (progressiveDelayLimit < 0)
				progressiveDelayLimit = 0;
			
			if (interval <= progressiveDelayLimit) {
				progressiveDelay = progressiveDelayLimit;
				
				#if UNITY_EDITOR
				UltimateLog(string.Format("The spawn delay has reached it's limit of {0} seconds", progressiveDelay));
				#endif
				
				return progressiveDelay;
			}

			// Reduce the delay
			progressiveDelay = interval - delayModifier;
			
			#if UNITY_EDITOR
			UltimateLog(string.Format("The next spawn will happen in {0} seconds", progressiveDelay));
			#endif
			
			return progressiveDelay;
		}
		
		#endregion

		#region Position

		void OnDrawGizmos() {

			// TODO: Improve gizmos! For now it only works with spawn points
			// SpawnPoint
			if (spawnAt == SpawnAt.SpawnPoint && showGizmos) {

				if (spawnPointEnum.list[selectedSpawnPointEnum] == Fixed) {
					Gizmos.DrawIcon(fixedSpawnPoint.VectorPosition(), "UltimateSpawner/spawner_icon.png", true);

				}

				else if (spawnPointEnum.list[selectedSpawnPointEnum] == RandomFixed) {
					foreach (var spawnPoint in randomSpawnPoints) {
						if (spawnPoint != null)
							Gizmos.DrawIcon(spawnPoint.VectorPosition(), "UltimateSpawner/spawner_icon.png", true);
					}
				}

			} 
			
			else if (spawnAt == SpawnAt.Position && showGizmos) {
				// Keep track of possibilities
				int countX = 0, countY = 0, countZ = 0;
				
				// X 
				// fixed
				if (positionEnum.list[selectedXEnum] == Fixed) countX = 1;
				// Random Range
				else if (positionEnum.list[selectedXEnum] == RandomRange) countX = 2;
				// Random Fixed
				else if (positionEnum.list[selectedXEnum] == RandomFixed) countX = randomFixedX.Count;
				// Y
				// fixed
				if (positionEnum.list[selectedYEnum] == Fixed) countY = 1;
				// Random Range
				else if (positionEnum.list[selectedYEnum] == RandomRange) countY = 2;
				// Random Fixed
				else if (positionEnum.list[selectedYEnum] == RandomFixed) countY = randomFixedY.Count;
				// Z
				// fixed
				if (positionEnum.list[selectedZEnum] == Fixed) countZ = 1;
				// Random Range
				else if (positionEnum.list[selectedZEnum] == RandomRange) countZ = 2;
				// Random Fixed
				else if (positionEnum.list[selectedZEnum] == RandomFixed) countZ = randomFixedZ.Count;

				float x = 0, y = 0, z = 0;
				for (int i = 0; i < countX; i++) {
					if (positionEnum.list[selectedXEnum] == Fixed) x = fixedX;
					else if (positionEnum.list[selectedXEnum] == RandomRange) {
						x = i == 0 ? randomRangeMinX : randomRangeMaxX;
					}
					else if (positionEnum.list[selectedXEnum] == RandomFixed)
						x = randomFixedX[i];

					for (int j = 0; j < countY; j++) {
						if (positionEnum.list[selectedYEnum] == Fixed) y = fixedY;
						else if (positionEnum.list[selectedYEnum] == RandomRange) {
							y = j == 0 ? randomRangeMinY : randomRangeMaxY;
						}
						else if (positionEnum.list[selectedYEnum] == RandomFixed)
							y = randomFixedY[j];

						for (int k = 0; k < countZ; k++) {
							
							if (positionEnum.list[selectedZEnum] == Fixed) z = fixedZ;
							else if (positionEnum.list[selectedZEnum] == RandomRange) {
								z = k == 0 ? randomRangeMinZ : randomRangeMaxZ;
							}
							else if (positionEnum.list[selectedZEnum] == RandomFixed)
								z = randomFixedZ[k];
							
							Gizmos.DrawIcon(new Vector3(x,y,z), "UltimateSpawner/spawner_icon.png", true);
						} // End Z for
					} // End Y for
				} // End X for
			}
			
			else if (spawnAt == SpawnAt.ExternalPosition) {
				Gizmos.DrawIcon(externalPosition, "UltimateSpawner/spawner_icon.png", true);
			}

		}

		Vector3 GetSpawnPosition() {
			
			// Spawner
			if (spawnAt == SpawnAt.Spawner) {
				return transform.position;
			} 
			// Transform
			else if (spawnAt == SpawnAt.TargetTransform) {
				if (spawnPointEnum.list[selectedTransformEnum] == Fixed) {
					return targetTransform.position;
				}

				else if (spawnPointEnum.list[selectedTransformEnum] == RandomFixed) {
					int randomTransform = Random.Range(0, targetTransformList.Count);
					return targetTransformList[randomTransform].position;
				}
			} 
			// SpawnPoint
			else if (spawnAt == SpawnAt.SpawnPoint) {

				if (spawnPointEnum.list[selectedSpawnPointEnum] == Fixed) {
					return fixedSpawnPoint.VectorPosition();
				}

				else if (spawnPointEnum.list[selectedSpawnPointEnum] == RandomFixed) {
					int randomSpawnpoint = Random.Range(0, randomSpawnPoints.Count);
					return randomSpawnPoints[randomSpawnpoint].VectorPosition();
				}

			}
			// Position
			else if (spawnAt == SpawnAt.Position) {
				
				// Create vector to return
				Vector3 spawnPosition = new Vector3();
				
				// X 
				// fixed
				if (positionEnum.list[selectedXEnum] == Fixed) {
					spawnPosition.x = fixedX;
				}
				// Random Range
				else if (positionEnum.list[selectedXEnum] == RandomRange) {
					spawnPosition.x = Random.Range(randomRangeMinX, randomRangeMaxX);
				}
				// Random Fixed
				else if (positionEnum.list[selectedXEnum] == RandomFixed) {
					spawnPosition.x = randomFixedX[Random.Range(0, randomFixedX.Count)];
				}
				
				// Y
				// fixed
				if (positionEnum.list[selectedYEnum] == Fixed) {
					spawnPosition.y = fixedY;
				}
				// Random Range
				else if (positionEnum.list[selectedYEnum] == RandomRange) {
					spawnPosition.y = Random.Range(randomRangeMinY, randomRangeMaxY);
				}
				// Random Fixed
				else if (positionEnum.list[selectedYEnum] == RandomFixed) {
					spawnPosition.y = randomFixedY[Random.Range(0, randomFixedY.Count)];
				}
				
				// Z
				// fixed
				if (positionEnum.list[selectedZEnum] == Fixed) {
					spawnPosition.z = fixedZ;
				}
				// Random Range
				else if (positionEnum.list[selectedZEnum] == RandomRange) {
					spawnPosition.z = Random.Range(randomRangeMinZ, randomRangeMaxZ);
				}
				// Random Fixed
				else if (positionEnum.list[selectedZEnum] == RandomFixed) {
					spawnPosition.z = randomFixedZ[Random.Range(0, randomFixedZ.Count)];
				}

				return spawnPosition;
			} 
			
			else if (spawnAt == SpawnAt.ExternalPosition)
				return externalPosition;
			
			#if UNITY_EDITOR
			UltimateLog("Something went wrong and the position is null", "ERROR");
			#endif
			
			// In case of null return at (0,0,0)
			return Vector3.zero;
		}
		

		#endregion

		#region Rotation
		
		Quaternion GetSpawnRotation() {

			
			switch (spawnRotation) {
					case SpawnRotation.Custom:
						rotationTransform.Rotate(customRotationX, customRotationY, customRotationZ);
						break;
					case SpawnRotation.Identity:
						rotationTransform.rotation = Quaternion.identity;
						break;
					case SpawnRotation.Spawner:
						rotationTransform.rotation = transform.rotation;
						break;
					case SpawnRotation.ObjectOwnRotation:
						break;
			}
			
			return rotationTransform.rotation;

		}

		#endregion

		#region Movement

		void ApplyMovement(GameObject spawnedObject) {
			if (spawnedObject.GetComponent<USExtension_Movement>() == null && movementType != MovementType.None) {
				spawnedObject.AddComponent<USExtension_Movement>();
			}
			
			if (spawnedObject.GetComponent<USExtension_Movement>() != null && movementType != MovementType.None) {
				// 2D
				if(objectType == ObjectType._2D && movementType == MovementType.Force)
					spawnedObject.GetComponent<USExtension_Movement>().Movement(force2D, forceMode2D);
				else if(objectType == ObjectType._2D && movementType == MovementType.Velocity)
					spawnedObject.GetComponent<USExtension_Movement>().Movement(velocity2D);
				// 3D
				else if(objectType == ObjectType._3D && movementType == MovementType.Force)
					spawnedObject.GetComponent<USExtension_Movement>().Movement(force3D, forceMode);
				else if(objectType == ObjectType._3D && movementType == MovementType.Velocity)
					spawnedObject.GetComponent<USExtension_Movement>().Movement(velocity3D);
				// None
				else if (movementType == MovementType.None)
					spawnedObject.GetComponent<USExtension_Movement>().StopMovement();

			}
			
			
		}

		#endregion

		#region UltimateSpawner Configuration

		public void SetEnum() {
			spawnPointEnum = Resources.Load<ScriptableUSEnumList>("ConfigFiles/EnumLists/SpawnPointEnum");
			positionEnum = Resources.Load<ScriptableUSEnumList>("ConfigFiles/EnumLists/PositionEnum");
			
			Fixed = Resources.Load<ScriptableUSEnum>("ConfigFiles/EnumValues/Fixed");
			RandomFixed = Resources.Load<ScriptableUSEnum>("ConfigFiles/EnumValues/RandomFixed");
			RandomRange = Resources.Load<ScriptableUSEnum>("ConfigFiles/EnumValues/RandomRange");
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Used to send a debug message
		/// </summary>
		/// <param name="message">The log you want to send</param>
		/// <param name="logType">You can use "NORMAL", "WARNING", "ERROR" </param>
		public void UltimateLog(string message, string logType = "NORMAL") {

			// If is not editor or we don't want to show logs
			if (!Application.isEditor || !ShowDebugMessages)
				return;

			// Create color
			Color ultimateSpawnerTagColor;
			
			// Setup a color variation for each skin
			ultimateSpawnerTagColor = EditorGUIUtility.isProSkin ? new Color(8,129,221) : new Color(5,81,139);
			
			// Header to indicate that the log came from UltimateSpawner
			string header = "UltimateSpawner: ";
			
			if (logType == "NORMAL")
				Debug.Log (string.Format("<color=#{0:X2}{1:X2}{2:X2}><b>{3}</b></color>{4}", (byte)(ultimateSpawnerTagColor.r), (byte)(ultimateSpawnerTagColor.g), (byte)(ultimateSpawnerTagColor.b), header, message), gameObject);
			
			if (logType == "WARNING")
				Debug.LogWarning (string.Format("<color=#{0:X2}{1:X2}{2:X2}><b>{3}</b></color>{4}", (byte)(ultimateSpawnerTagColor.r), (byte)(ultimateSpawnerTagColor.g), (byte)(ultimateSpawnerTagColor.b), header, message), gameObject);
			
			if (logType == "ERROR")
				Debug.LogError (string.Format("<color=#{0:X2}{1:X2}{2:X2}><b>{3}</b></color>{4}", (byte)(ultimateSpawnerTagColor.r), (byte)(ultimateSpawnerTagColor.g), (byte)(ultimateSpawnerTagColor.b), header, message), gameObject);
		}

		void ShowUltimateSpawnerSetUp() {
			
			// Log UltimateSpawner configuration
			string setup = "UltimateSpawner Setup\n";
			setup += "Object to Spawn: " + objectToSpawn.name + "\n";
			setup += "Spawn Mode: " + spawnMode.ToString() + "\n";

			if (spawnMode == SpawnMode.FixedTime)
				setup += "Fixed time: " + fixedDelayBetweenSpawns + "\n";
			if (spawnMode == SpawnMode.RandomTime)
				setup += string.Format("Random Time Between {0} and {1}\n", minDelayBetweenSpawns, maxDelayBetweenSpawns);
			if (spawnMode == SpawnMode.ProgressiveTime)
				setup += string.Format("Progressive Time starts at: {0}" +
				                       "\nand will reduce: {1} after every spawn" +
				                       "\nbut will stop at: {2}\n",
					startingDelayBetweenSpawns, delayModifier, progressiveDelayLimit);
			if (spawnMode == SpawnMode.Input)
				setup += "Spawn when pressing key: " + inputKeyCode.ToString() + "\n";

			if (spawnAt == SpawnAt.Spawner)
				setup += "Spawn At: UltimateSpawner Position\n";
			if (spawnAt == SpawnAt.SpawnPoint) {
				if (spawnPointEnum == Fixed)
					setup += "Fixed Spawn Point: " + fixedSpawnPoint.name + "\n";

				if (spawnPointEnum == RandomFixed)
					setup += string.Format("Randomizing between {0} spawn points\n", randomSpawnPoints.Count);
			}

			if (spawnAt == SpawnAt.Position) {
				setup += "Spawn at: Custom Position\n";
				
				if (positionEnum.list[selectedXEnum] == Fixed)
					setup += "X is fixed at: " + fixedX + "\n";
				if (positionEnum.list[selectedXEnum] == RandomFixed)
					setup += "X will randomize between " + randomFixedX.Count + "positions \n";
				if (positionEnum.list[selectedXEnum] == RandomRange)
					setup += string.Format("X will be randomized between {0} and {1}\n", randomRangeMinX, randomRangeMaxX);
				
				if (positionEnum.list[selectedYEnum] == Fixed)
					setup += "Y is fixed at: " + fixedY + "\n";
				if (positionEnum.list[selectedYEnum] == RandomFixed)
					setup += "Y will randomize between " + randomFixedY.Count + "positions \n";
				if (positionEnum.list[selectedYEnum] == RandomRange)
					setup += string.Format("Y will be randomized between {0} and {1}\n", randomRangeMinY, randomRangeMaxY);
				
				if (positionEnum.list[selectedZEnum] == Fixed)
					setup += "Z is fixed at: " + fixedZ + "\n";
				if (positionEnum.list[selectedZEnum] == RandomFixed)
					setup += "Z will randomize between " + randomFixedZ.Count + "positions \n";
				if (positionEnum.list[selectedZEnum] == RandomRange)
					setup += string.Format("Z will be randomized between {0} and {1}\n", randomRangeMinZ, randomRangeMaxZ);

			}

			if (spawnAt == SpawnAt.TargetTransform)
				if(spawnPointEnum.list[selectedTransformEnum] == Fixed)
					setup += string.Format("Spawn at {0}'s transform position\n", targetTransform.name);
				else if(spawnPointEnum.list[selectedTransformEnum] == RandomFixed)
					setup += string.Format("Randomizing between {0} transforms\n", targetTransformList.Count);
					
			
			if (spawnRotation == SpawnRotation.Identity)
				setup += "Object's rotation will be: Quaternion.Identity (0,0,0)\n";
			if (spawnRotation == SpawnRotation.Spawner)
				setup += "Object's rotation will be: UltimateSpawner Rotation\n";
			if (spawnRotation == SpawnRotation.ObjectOwnRotation)
				setup += "Object's rotation will be: Prefab's Current Rotation\n";
			if (spawnRotation == SpawnRotation.Custom)
				setup += string.Format("Object's rotation will be: ({0},{1},{2})\n",customRotationX, customRotationY, customRotationZ);

			if (movementType == MovementType.None)
				setup += "Won't apply any movement\n";
			if (movementType == MovementType.Force)
				setup += "UltimateSpawner will apply force to Object\n";
			if (movementType == MovementType.Velocity)
				setup += "UltimateSpawner will apply a speed to objects's velocity\n";

			setup += usePoolSystem ? "Use Pool System: Yes\n" : "Use Pool System: No\n";

			if (usePoolSystem)
				setup += "Pool Size: " + poolSize + "\n";

			setup += canIncreasePoolSize ? "Can Increase Pool Size: Yes\n" : "Can Increase Pool Size: No\n";

			if (canIncreasePoolSize)
				setup += "Pool Max Size: " + poolMaxSize + "\n";
			
			UltimateLog(setup);
		}
		#endif

		public void SetObjectToSpawn(GameObject newObjectToSpawn) {
			objectToSpawn = newObjectToSpawn;
		}

		public void SetExternalPosition(Vector3 extPos) {
			externalPosition = extPos;
		}

		#endregion
		
		#region UltimateSpawner Events
		
		public delegate void US_Event();

		public event US_Event Spawned;

		#endregion
	}

	public enum SpawnMode {
		FixedTime,
		ProgressiveTime,
		RandomTime,
		External,
		Input
	}

	public enum SpawnAt {
		Spawner,
		SpawnPoint,
		Position,
		TargetTransform,
		ExternalPosition
	}

	public enum SpawnRotation {
		Identity,
		Spawner,
		Custom,
		ObjectOwnRotation
	}

	public enum ObjectType {
		_2D,
		_3D
	}

	public enum MovementType {
		None,
		Force,
		Velocity
	}
	
}