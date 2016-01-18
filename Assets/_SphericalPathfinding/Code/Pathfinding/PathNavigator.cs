using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlanetBody))]
public class PathNavigator : MonoBehaviour
{
	public SphericalGrid sphericalGrid;
	public Transform target;
    GameObject PathFinding;

	Vector3 prevTargetPos;
	
	public float lookSpeed = 1;

	Vector3[] path;
	int targetIndex;

	bool travelling = false;
	bool travellingStraight = false;

	PlanetBody planetBody;

	public bool drawPath;

    public bool locked;

	public LayerMask mask;

	private float timer;

	#region Unity

	void Awake()
	{      
        PathFinding = GameObject.FindWithTag("PathFinding");
        if(PathFinding) sphericalGrid = PathFinding.GetComponent<SphericalGrid>();
        planetBody = GetComponent<PlanetBody>();
		target = new GameObject ().transform;
        target.name = "PathfindingTarget";
	}

	void Update()
	{	
		if (!locked)
		{
			if (target != null && !travelling)
			{
					travelling = true;
					
					if((transform.position-target.position).magnitude <= 20) {
						DecideMethod(target.position);
					} else {
						travellingStraight = false;
						PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
					}
						
					timer = Time.time;
			}
		}  
		
	}


	#endregion


	// *************************
	//          NAVIGATE
	// *************************

	public void DecideMethod(Vector3 t) 
	{
		Quaternion q = planetBody.LookAtTarget (t);
		GameObject trans = new GameObject();
		trans.transform.rotation = q;
		Vector3 direction = trans.transform.forward;
		Destroy (trans);

		Vector3 tempTo = transform.position + 20 * direction;
		Vector3 castTo = tempTo.normalized * (GameValues.PlanetRadius*1.05f);
		Vector3 castFrom = transform.position.normalized * (GameValues.PlanetRadius*1.05f);
		direction = (castTo - castFrom).normalized;

		RaycastHit hit;
		Ray ray = new Ray (castFrom, direction);
		Debug.DrawLine (castFrom, castTo, Color.cyan, 2f);
		if (Physics.Raycast (ray, out hit, 20)) {
			if(hit.transform.tag == "NotWalkable") {
				//Debug.Log ("Hit obstacle. Requesting Pathfinding.");
				travellingStraight = false;
                travelling = true;
				PathRequestManager.RequestPath (transform.position, t, OnPathFound);
				return;
			}
		} 
		travellingStraight = true;
		//Debug.Log ("Way is clear.");
		StopCoroutine("FollowPathStraight");
		StopCoroutine("FollowPath");
		path = new Vector3[0];
		StartCoroutine ("FollowPathStraight");
	}

	public void SetTarget(Vector3 t) 
	{ 
		//Debug.Log ("changed target");
		travelling = false;
		target.position = t;
	}

    public void StopMoving()
    {
        StopCoroutine("FollowPath");
        locked = true;
        travelling = false;
    }
	
	public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	{
        if (gameObject.activeSelf)
        {
            if (pathSuccessful && newPath.Length > 0)
            {
				//Debug.Log ("Path found in: " + (Time.time-timer));
				path = newPath;
				StopCoroutine("FollowPathStraight");
				StopCoroutine("FollowPath");
				StartCoroutine("FollowPath");
			}
			else
			{
				gameObject.SendMessage("NoPathFound");
				travelling = false;
			}
		}	
	}

	IEnumerator FollowPathStraight()
	{
		while (!locked) 
		{
			float dist = (transform.position - target.position).magnitude;
			if (dist <= 0.5f) 
			{
				travelling = false;
				travellingStraight = false;
				yield break;
			}
			MoveTowards(target.position);
			yield return null;		
		}
	}
	
	IEnumerator FollowPath()
	{
		targetIndex = 0;
		Vector3 currentWaypoint = path[targetIndex];
		
		while (true && !locked) 
		{
			float dist = (transform.position - currentWaypoint).magnitude;

			if (dist <= 0.75f) 
			{
				targetIndex ++;
				if (targetIndex >= path.Length) 
				{
					travelling = false;
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
			MoveTowards(currentWaypoint);
			yield return null;
			
		}
	}

	public void MoveTowards(Vector3 tPos)
	{
		Quaternion newRot = planetBody.LookAtTarget(tPos);
		transform.rotation = Quaternion.Slerp(transform.rotation, newRot, lookSpeed * Time.deltaTime);

		//transform.position 	= planetBody.MoveForward(moveSpeed);
		GetComponent<MoveOnSphere> ().moveTowards(tPos);
	}


	// *************************
	//         UTILITY
	// *************************


	Vector3 RandomTargetPos()
	{
		Vector3 rndDir = new Vector3(transform.forward.x * Random.Range(-1, 1), transform.forward.y * Random.Range(-1, 1), transform.forward.z * Random.Range(-1, 1));
		float distance = Random.Range(10, 60);
		Vector3 point = transform.position + ((rndDir) * distance);
		
		return planetBody.GroundPosition(point);
	}



	// *************************
	//          DEBUG
	// *************************
	
	public void OnDrawGizmos()
	{
		if (path != null && drawPath) 
		{
			for (int i = targetIndex; i < path.Length; i ++) 
			{
				Gizmos.color = Color.green;
				Gizmos.DrawCube(path[i], Vector3.one*0.02f);
				
				if (i == targetIndex) 
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else 
				{
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}


}
