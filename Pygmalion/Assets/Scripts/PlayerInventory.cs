using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

	public GameObject bigObj;
	public Dictionary<string, SmallObject> smallObjs = new Dictionary<string, SmallObject>();

	public void addObject(string id){
		SmallObject obj;
		if (smallObjs.TryGetValue(id, out obj)){
			obj.count++;
		}
		else {
			smallObjs[id] = new SmallObject(id);
		}
	}

	public bool containsSmallObject(string id){
		return smallObjs.ContainsKey(id);
	}
	public bool containsSmallObject(string id, uint n){
		SmallObject obj;
		if (smallObjs.TryGetValue(id, out obj)){
			return obj.count >= n;
		}
		return false;
	}

	public void removeSmallObject(string id){
		smallObjs.Remove(id);
	}
	public void removeSmallObject(string id, uint n){
		SmallObject obj;
		if (smallObjs.TryGetValue(id, out obj)){
			if (n >= obj.count)
				smallObjs.Remove(id);
			else
				obj.count -= n;
		}
	}

	[System.Serializable]
	public class SmallObject {
		public string id;
		public uint count;
		public SmallObject(string id){
			this.id = id;
			count = 1;
		}
	}
}
