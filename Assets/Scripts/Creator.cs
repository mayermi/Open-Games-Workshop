using UnityEngine;
using System.Collections;

// this class only serves as a static wrapper for prefab instantiation
public static class Creator {
	
	public static GameObject Create(string name, Vector3 pos)
	{
		GameObject g = Resources.Load (name) as GameObject;
		return MonoBehaviour.Instantiate (g, pos, Quaternion.identity) as GameObject;
	}
}
