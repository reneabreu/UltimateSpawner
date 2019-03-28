v0.2.6-bugfix
- Fixed US not adding USPoolObject script to new spawned object when increases the pool size

v0.2.6
- Created Spawned Event
- Created latestSpawnedObject
- Improved Pooling System!
  - Before spawn US checks if the pool is being used and if it was created (prevent errors if a pool is created during gameplay)
  - Before get next object in pool, check if the pool still exists
  - Fixed errors at the pooling system if a pool object was destroyed
  - If for some reason US is using pooling and poolMaxSize < poolSize we set poolMaxSize to poolSize + 1 to prevent errors
  - PoolObject script (for now it just alert the player that the object was destroyed)
- Added Center as a screen based SpawnPoint
- Changed namespace from UltimateSpawner to UltimateSpawnerSystem
- Added help link
- US now count spawns
- Fixed errors caused by UltimateLog and USSetup.cs on build process
- Improved gizmos to work with custom position
- Created another vector3 to use with screenbased cases
- Fixed error caused by not setting an enum outside editor

v0.2.4
- Fixed: A few spelling in editor
- Fixed: Debug Logs
- Added: A new Log System

v0.2.3
- Fixed: scene folders being imported at root folder
- Fixed: spawn method creating a empty gameobject
- Fixed: changing spawner to use pool in-game not creating pool
- Fixed: movement first spawn not moving
- Implemented: SpawnPoints can now be based on screen size 
- Added: Script icon
- Changed: Gizmo icon

v0.2.2
- Fixed Custom Editor Errors
- Now you can edit spawn position X, Y, Z 
- Renamed Folders
- Created ScriptableObjects to use as enum

v0.2.1
- Minor bug fixes
- Renamed some folders

v0.2
- Implemented the option to apply movement to spawned object
  - Created a movement extension that is attached automatically to spawned object
- Implemented the option to apply rotation to spawned object
- Changed the custom Editor to look more intuitive
- Fixed a few bugs i've found