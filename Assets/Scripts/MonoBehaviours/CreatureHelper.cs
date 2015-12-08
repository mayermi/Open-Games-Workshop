using UnityEngine;
using System.Collections;

public class CreatureHelper : MonoBehaviour {

    private Canvas c;

    void Start () {
        c = gameObject.GetComponentInChildren<Canvas>();
    }
	
	void Update () {
        // make sure the health bar faces the camera
        c.transform.LookAt(Camera.main.transform.position);
    }
}
