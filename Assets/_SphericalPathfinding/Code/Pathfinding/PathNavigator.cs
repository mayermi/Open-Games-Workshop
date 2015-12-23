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
		if (travellingStraight)
		{
			MoveTowards(target.position);
		}

        if (!locked)
        {
            // if the target position has moved
            if (target != null)
            {
                float targetPosDiff = Vector3.Distance(prevTargetPos, target.position);

                if (targetPosDiff > 0.01f)
                {
                    travelling = true;

					DecideMethod(target.position);
                    
					timer = Time.time;
                }

                prevTargetPos = target.position;
            }

            if (!travelling) // if the navigator has finished travelling
            {
                Vector3 targetPos = Vector3.zero;
                if (target != null) targetPos = target.position;
                //else targetPos = RandomTargetPos();

                // check the distance to its target position, if it's far away start navigating again
                /*float dist = (transform.position - targetPos).sqrMagnitude;
			    if(dist > 0.15f)
			    {
				    travelling = true;
				    PathRequestManager.RequestPath(transform.position, targetPos, OnPathFound);
			    }*/
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

		RaycastHit hit;
		Ray ray = new Ray (transform.position, direction);
		Debug.DrawLine (transform.position, transform.position + 20*direction, Color.cyan, 5f);
		if (Physics.Raycast (ray, out hit, 20, mask, QueryTriggerInteraction.Ignore))
		{
			if (hit.transform.tag == "NotWalkable")
			{	
				Debug.Log ("Requesting Pathfinding");
				travellingStraight = false;
				PathRequestManager.RequestPath(transform.position, t, OnPathFound);
				return;
			} 
		} 
		travellingStraight = true;
	}

	public void SetTarget(Vector3 t) 
	{ 
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
				Debug.Log ("Path found in: " + (Time.time-timer));
                path = newPath;
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
	
	IEnumerator FollowPath()
	{
		targetIndex = 0;
		Vector3 currentWaypoint = path[targetIndex];
		
		while (true && !locked) 
		{
			float dist = (transform.position - currentWaypoint).sqrMagnitude;

			if (dist <= 0.25f) 
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

	public void MoveTowards(Vector3 targetPos)
	{
		Quaternion newRot = planetBody.LookAtTarget(targetPos);
		transform.rotation = Quaternion.Slerp(transform.rotation, newRot, lookSpeed * Time.deltaTime);

		//transform.position 	= planetBody.MoveForward(moveSpeed);
		GetComponent<MoveOnSphere> ().moveTowards(targetPos);
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
