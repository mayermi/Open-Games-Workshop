using UnityEngine;
using System.Collections;

public class PlanetBody : MonoBehaviour
{
	public Transform planetTransform = null;
	public float planetRadius = 5f;



	#region Unity

	void Start()
	{
		transform.position = GroundPosition(transform.position);
	}

	void Update()
	{
		transform.rotation = RotateToPlanet();
	}

	#endregion



	// *************************
	//      PLANET ROTATION
	// *************************

	public Quaternion RotateToPlanet()
	{
		if(planetTransform != null)
		{
			// find what way is up based on the body's current position
			Vector3 gravityUp = (planetTransform.position - this.transform.position).normalized;
			// Rotation object to new rotation
			return Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
		}

		return new Quaternion();
	}

	public Quaternion LookAtTarget(Vector3 targetPos)
	{
		Vector3 dir = getRelativePosition(transform, targetPos);
		float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
		return Quaternion.AngleAxis(angle, transform.up) * transform.rotation; 
	}

	Vector3 getRelativePosition(Transform origin, Vector3 position)
	{
		Vector3 distance = position - origin.position;
		Vector3 relativePosition = Vector3.zero;
		relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
		relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
		relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);
		
		return relativePosition;
	}



	// *************************
	//        PLANET MOVE
	// *************************

	public Vector3 MoveForward(float _speed)
	{
		if(planetTransform != null)
		{
			// Apply forward amount
			Vector3 forward = transform.forward * (_speed * Time.deltaTime);

			// Make sure the new position is still on the planet
			Vector3 newPos = (transform.position + forward);
			newPos = (newPos - planetTransform.position).normalized * planetRadius;

			// return new position
			return newPos;
		}

		return Vector3.zero;
	}

	public Vector3 GroundPosition(Vector3 currentPosition)
	{
		Vector3 dir = (planetTransform.position - currentPosition).normalized;
		Vector3 startRayPos = -dir * (planetRadius * 1.1f);
		
		Ray ray = new Ray();
		ray.origin = startRayPos;
		ray.direction = dir;
		
		int groundTypeLayer = 1<<10;
		RaycastHit hit;
		
		//Debug.DrawRay(startRayPos,  dir * radius);
		//Debug.Break();
		if(Physics.Raycast(startRayPos, dir, out hit, (planetRadius * 1.1f), groundTypeLayer))
		{
			return hit.point;
		}
		
		return currentPosition;
	}

}
