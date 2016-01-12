using UnityEngine;
using System.Collections;

public class LockRotation : MonoBehaviour {

	float lockPos = 0;
	void Start () {
		Vector3 pos = transform.eulerAngles;
		Vector3 localpos = transform.localEulerAngles;
		transform.eulerAngles = new Vector3(pos.x,pos.y,90);
	}
	
	// Update is called once per frame
	void Update () {
		transform.localRotation = Quaternion.Euler(lockPos, lockPos, lockPos);
	}
}
