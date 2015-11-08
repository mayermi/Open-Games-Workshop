using UnityEngine;
using System.Collections;

public class PlanetGravity : MonoBehaviour {

	public float gravity = -9.81f;
	
	void OnTriggerStay(Collider other)
    {
        Vector3 direction = (other.transform.position - transform.position).normalized;
        other.attachedRigidbody.AddForce(direction * gravity);

        Quaternion q = Quaternion.FromToRotation(other.transform.up, direction);
        q = q * other.transform.rotation;
        if (other.transform.rotation != q) {
            other.transform.rotation = Quaternion.Slerp(other.transform.rotation, q, 0.1f * Time.deltaTime);
        }
    }
}
