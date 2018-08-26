using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSpawner {

	public class US_MovementExtension : MonoBehaviour {

		private Rigidbody rigidbody;
		private Rigidbody2D rigidbody2D;

		private Vector3 force;
		private Vector2 force2D;

		private ForceMode forceMode;
		private ForceMode2D forceMode2D;

		private bool applyForce;
		
		private void OnEnable() {
			if (GetComponent<Rigidbody>() != null)
				rigidbody = GetComponent<Rigidbody>();
			
			if (GetComponent<Rigidbody2D>() != null)
				rigidbody2D = GetComponent<Rigidbody2D>();
			
		}

		void FixedUpdate() {

			if (applyForce) {
				if (rigidbody != null)
					rigidbody.AddForce(force, forceMode);
				else if (rigidbody2D != null)
					rigidbody2D.AddForce(force2D, forceMode2D);
				
			}
		}

		public void Movement(Vector2 force, ForceMode2D forceMode2D) {
			force2D = force;
			this.forceMode2D = forceMode2D;

			applyForce = true;
		}
		
		public void Movement(Vector3 force, ForceMode forceMode) {
			this.force = force;
			this.forceMode = forceMode;
			
			applyForce = true;
		}
		
		public void Movement(Vector2 speed) {
			applyForce = false;

			rigidbody2D.velocity = speed;
		}
		
		public void Movement(Vector3 speed) {
			applyForce = false;

			rigidbody.velocity = speed;
		}

		public void StopMovement() {
			if (rigidbody != null)
				rigidbody.velocity = Vector3.zero;
			
			if (rigidbody2D != null)
				rigidbody2D.velocity = Vector2.zero; 
			
			applyForce = false;
		}
	}
}