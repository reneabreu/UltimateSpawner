using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {

	void OnEnable () {
		StartCoroutine(AutoDeactivate());
	}
	
	IEnumerator AutoDeactivate() {
		yield return new WaitForSeconds(1f);
		gameObject.SetActive(false);
	}
}
