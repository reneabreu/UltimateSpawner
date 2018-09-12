using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSpawnerSystem {

	public class ScriptableUSEnumList : ScriptableObject {
		public new string name;
		public List<ScriptableUSEnum> list;
	}
}