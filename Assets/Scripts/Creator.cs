using UnityEngine;
using System.Collections;

// this class only serves as a static wrapper for prefab instantiation
public static class Creator {
	
	public static GameObject Create(string resName, Vector3 pos, string newName, Quaternion rot)
	{
		GameObject g = Resources.Load (resName) as GameObject;
		GameObject created = MonoBehaviour.Instantiate (g, pos, rot) as GameObject;
        created.name = newName;
        return created;
	}

    public static GameObject Create(string resName, Vector3 pos, string newName)
    {
        GameObject g = Resources.Load(resName) as GameObject;
        Quaternion rotation = Quaternion.identity;
        GameObject created = MonoBehaviour.Instantiate(g, pos, rotation) as GameObject;
        created.name = newName;
        return created;
    }
}
