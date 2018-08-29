using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSpawner {

	public class ScriptableUSEnumList : ScriptableObject {
		public new string name;
		public List<ScriptableUSEnum> list;
	}
}