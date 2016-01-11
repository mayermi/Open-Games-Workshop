using UnityEngine;
using System.Collections;

public class LockRotation : MonoBehaviour {

	float lockPos = 0;
	// Use this for initialization
	void Start () {
		Vector3 pos = transform.eulerAngles;
		transform.eulerAngles = new Vector3(pos.x,pos.y,90);
		Debug.Log (transform.eulerAngles);
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler(lockPos, lockPos, lockPos);
	}
}
