using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSpawner {

	[CreateAssetMenu(fileName = "Spawn Point", menuName = "UltimateSpawner/Spawn Point")]
	public class SpawnPoint : ScriptableObject {

		public SpawnPointPosition positionType;

		public enum ScreenBased {
			TopLeft,
			Top,
			TopRight,
			Left,
			Right,
			BottomLeft,
			Bottom,
			BottomRight
		}

		public ScreenBased screenBasedPosition;
		public Vector3 offset;
		public Vector3 vectorPosition;
		private Camera c;


		public Vector3 VectorPosition() {
			
			c = Camera.main;
			
			if (positionType == SpawnPointPosition.Vector3)
				return vectorPosition;

			if (positionType == SpawnPointPosition.ScreenBased) {
				if (screenBasedPosition == ScreenBased.TopLeft) {
					vectorPosition =
						c.ScreenToWorldPoint(new Vector3(offset.x, c.pixelHeight + offset.y, offset.z));
					return vectorPosition;
				}
				if (screenBasedPosition == ScreenBased.Top) {
					vectorPosition =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth / 2 + offset.x, c.pixelHeight + offset.y, offset.z));
					return vectorPosition;
				}
				if (screenBasedPosition == ScreenBased.TopRight) {
					vectorPosition =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth + offset.x, c.pixelHeight + offset.y, offset.z));
					return vectorPosition;
				}
				
				if (screenBasedPosition == ScreenBased.Left) {
					vectorPosition =
						c.ScreenToWorldPoint(new Vector3(offset.x, c.pixelHeight/2 + offset.y, offset.z));
					return vectorPosition;
				}
				
				if (screenBasedPosition == ScreenBased.Right) {
					vectorPosition =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth + offset.x, c.pixelHeight/2 + offset.y, offset.z));
					return vectorPosition;
				}
				
				if (screenBasedPosition == ScreenBased.BottomLeft) {
					vectorPosition =
						c.ScreenToWorldPoint(new Vector3(offset.x, offset.y, offset.z));
					return vectorPosition;
				}
				
				if (screenBasedPosition == ScreenBased.Bottom) {
					vectorPosition =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth / 2 + offset.x, offset.y, offset.z));
					return vectorPosition;
				}
				
				if (screenBasedPosition == ScreenBased.BottomRight) {
					vectorPosition =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth + offset.x, offset.y, offset.z));
					return vectorPosition;
				}
			}
			
			return vectorPosition;
		}
	}
	
	public enum SpawnPointPosition {
		Vector3,
		ScreenBased
	}

}