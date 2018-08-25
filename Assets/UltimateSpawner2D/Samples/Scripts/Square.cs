using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(AutoDeactivate());
	}
	
	IEnumerator AutoDeactivate() {
		yield return new WaitForSeconds(.5f);
		gameObject.SetActive(false);
	}
}
