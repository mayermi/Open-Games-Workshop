using UnityEngine;
using System.Collections;

public class LockRotationGod : MonoBehaviour {

	private Quaternion spawnRot;

	void Start () {
		spawnRot = transform.localRotation;
	}

	void Update () {
		transform.localRotation = spawnRot;
	}
}
