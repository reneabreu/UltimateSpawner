using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSpawner {

	public class Square : MonoBehaviour {

		void OnEnable() {
			StartCoroutine(AutoDeactivate());
			
			if(GetComponent<Rigidbody2D>() != null )
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			if (GetComponent<Rigidbody>() != null)
				GetComponent<Rigidbody>().velocity = Vector3.zero;
		}

		IEnumerator AutoDeactivate() {
			yield return new WaitForSeconds(1f);
			gameObject.SetActive(false);
		}
	}
}