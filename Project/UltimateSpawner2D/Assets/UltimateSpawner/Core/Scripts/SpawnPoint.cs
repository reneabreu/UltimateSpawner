using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSpawnerSystem {

	[CreateAssetMenu(fileName = "Spawn Point", menuName = "UltimateSpawner/Spawn Point")]
	public class SpawnPoint : ScriptableObject {

		public SpawnPointPosition positionType;

		public enum ScreenBased {
			TopLeft,
			Top,
			TopRight,
			Left,
			Center,
			Right,
			BottomLeft,
			Bottom,
			BottomRight
		}

		public ScreenBased screenBasedPosition;
		public Vector3 offset;
		public Vector3 vectorPosition;
		public Vector3 vectorPositionScreenBased;
		private Camera c;


		public Vector3 VectorPosition() {
			
			c = Camera.main;
			
			if (positionType == SpawnPointPosition.Vector3)
				return vectorPosition;

			if (positionType == SpawnPointPosition.ScreenBased2D) {
				if (screenBasedPosition == ScreenBased.TopLeft) {
					vectorPositionScreenBased =
						c.ScreenToWorldPoint(new Vector3(offset.x, c.pixelHeight + offset.y, offset.z));
					vectorPositionScreenBased.z = 0 + offset.z;
					return vectorPositionScreenBased;
				}
				if (screenBasedPosition == ScreenBased.Top) {
					vectorPositionScreenBased =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth / 2 + offset.x, c.pixelHeight + offset.y, offset.z));
					vectorPositionScreenBased.z = 0 + offset.z;
					return vectorPositionScreenBased;
				}
				if (screenBasedPosition == ScreenBased.TopRight) {
					vectorPositionScreenBased =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth + offset.x, c.pixelHeight + offset.y, offset.z));
					vectorPositionScreenBased.z = 0 + offset.z;
					return vectorPositionScreenBased;
				}
				
				if (screenBasedPosition == ScreenBased.Left) {
					vectorPositionScreenBased =
						c.ScreenToWorldPoint(new Vector3(offset.x, c.pixelHeight/2 + offset.y, offset.z));
					vectorPositionScreenBased.z = 0 + offset.z;
					return vectorPositionScreenBased;
				}
				
				if (screenBasedPosition == ScreenBased.Center) {
					vectorPositionScreenBased =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth / 2 + offset.x, c.pixelHeight/2 + offset.y, offset.z));
					vectorPositionScreenBased.z = 0 + offset.z;
					return vectorPositionScreenBased;
				}
				
				if (screenBasedPosition == ScreenBased.Right) {
					vectorPositionScreenBased =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth + offset.x, c.pixelHeight/2 + offset.y, offset.z));
					vectorPositionScreenBased.z = 0 + offset.z;
					return vectorPositionScreenBased;
				}
				
				if (screenBasedPosition == ScreenBased.BottomLeft) {
					vectorPositionScreenBased =
						c.ScreenToWorldPoint(new Vector3(offset.x, offset.y, offset.z));
					vectorPositionScreenBased.z = 0 + offset.z;
					return vectorPositionScreenBased;
				}
				
				if (screenBasedPosition == ScreenBased.Bottom) {
					vectorPositionScreenBased =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth / 2 + offset.x, offset.y, offset.z));
					vectorPositionScreenBased.z = 0 + offset.z;
					return vectorPositionScreenBased;
				}
				
				if (screenBasedPosition == ScreenBased.BottomRight) {
					vectorPositionScreenBased =
						c.ScreenToWorldPoint(new Vector3(c.pixelWidth + offset.x, offset.y, offset.z));
					vectorPositionScreenBased.z = 0 + offset.z;
					return vectorPositionScreenBased;
				}
			}
			
			return vectorPosition;
		}
	}
	
	public enum SpawnPointPosition {
		Vector3,
		ScreenBased2D
	}

}