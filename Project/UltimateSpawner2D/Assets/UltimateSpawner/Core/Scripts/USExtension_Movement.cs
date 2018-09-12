using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSpawnerSystem {

	public class USExtension_Movement : MonoBehaviour {

		private Rigidbody rigidBody;
		private Rigidbody2D rigidBody2D;

		private Vector3 force;
		private Vector2 force2D;

		private ForceMode forceMode;
		private ForceMode2D forceMode2D;

		private bool applyForce;
		
		private void OnEnable() {
			if (GetComponent<Rigidbody>() != null)
				rigidBody = GetComponent<Rigidbody>();
			
			if (GetComponent<Rigidbody2D>() != null)
				rigidBody2D = GetComponent<Rigidbody2D>();
			
		}

		void FixedUpdate() {

			if (applyForce) {
				if (rigidBody != null)
					rigidBody.AddForce(force, forceMode);
				else if (rigidBody2D != null)
					rigidBody2D.AddForce(force2D, forceMode2D);
				
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

			rigidBody2D.velocity = speed;
		}
		
		public void Movement(Vector3 speed) {
			applyForce = false;

			rigidBody.velocity = speed;
		}

		public void StopMovement() {
			if (rigidBody != null)
				rigidBody.velocity = Vector3.zero;
			
			if (rigidBody2D != null)
				rigidBody2D.velocity = Vector2.zero; 
			
			applyForce = false;
		}
	}
}