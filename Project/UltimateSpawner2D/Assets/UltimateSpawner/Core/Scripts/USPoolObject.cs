using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSpawner {


	public class USPoolObject : MonoBehaviour {

		// Keep traking to which UltimateSpawner's pool this object belongs to
		private UltimateSpawner ultimateSpawner;

		// Set UltimateSpawner
		public void SetUltimateSpawner(UltimateSpawner us) {
			this.ultimateSpawner = us;
		}

		private void OnApplicationQuit() {
			// Prevent Errors on leaving the game
			ultimateSpawner = null;
		}

		void OnDestroy() {

			if (ultimateSpawner != null) {
				// Warn player about destroing a object
				ultimateSpawner.UltimateLog(string.Format("Object {0} was destroyed but it was in a pool. " +
				                                          "It's highly recommended to disable it instead of destroying",
						gameObject.name),
					"WARNING");

				// Remove Object From pool
				ultimateSpawner.RemoveObjectFromPool(this.gameObject);
			}
		
		}
	}

}