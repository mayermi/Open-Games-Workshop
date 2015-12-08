using UnityEngine;
using System.Collections;

public class MyRotateScript : MonoBehaviour {

	public void RotateMyCube()
    {
        Vector3 vec3 = new Vector3(30, 30, 30);
        this.transform.Rotate(vec3);
    }
}
