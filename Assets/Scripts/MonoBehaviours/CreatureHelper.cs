using UnityEngine;
using System.Collections;

public class CreatureHelper : MonoBehaviour {

    private Canvas c;

    public virtual void Start () {
        c = gameObject.GetComponentInChildren<Canvas>();
    }
	
	public virtual void Update () {
        // make sure the health bar faces the camera
        c.transform.LookAt(Camera.main.transform.position);
    }
}
