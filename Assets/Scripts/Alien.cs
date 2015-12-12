using UnityEngine;
using System.Collections;

public class Alien : Creature {

	public enum AlienState { SEARCHING, FLEEING, CARRYING }
	public GameObject Resource { get; set;}
	public AlienState state;
	private Vector3 target;
	private float searchTime;

	public Alien(int health, float speed, int range) : base(health, speed, range){
		state = AlienState.SEARCHING;
	}

	public void TakeResource()
    {
        Resource.transform.SetParent(GameObject.transform);
        Resource.transform.position = GameObject.transform.position + new Vector3(0,1,0);
        Resource.GetComponent<Collider>().enabled = false;
        state = AlienState.CARRYING;
        Debug.Log("Going back to SpaceShip");
    }

	public void DropResource()
    {
        Resource = null;
    }

	public void Search()
	{
        //Debug.Log((GameObject.transform.position - target).magnitude);
        state = AlienState.SEARCHING;

        // for initialisation
        if (target == Vector3.zero || (Time.time - searchTime) > 15f)
			target = GameObject.transform.position;

		if ((GameObject.transform.position - target).magnitude <= 8f)
		{
			Vector3 rndDir = new Vector3(GameObject.transform.forward.x * Random.Range(-1, 1),
			                             GameObject.transform.forward.y * Random.Range(-1, 1),
			                             GameObject.transform.forward.z * Random.Range(-1, 1));
			float distance = Random.Range(10, 60);
			target = GameObject.transform.position + ((rndDir) * distance);
			target = CoordinateHelper.GroundPosition(target);
			Debug.Log ("Last Search: " + (Time.time - searchTime));
			searchTime = Time.time;
			MoveTo (target);
		}
	}

	public void Flee()
    {
        state = AlienState.FLEEING;
    }

    public void CarryResource(Vector3 ship_pos)
    {
        Debug.Log("carrying...");
        state = AlienState.CARRYING;

        if (target != ship_pos)
        {
            target = ship_pos;
            MoveTo(target);
        }
        // Alien is back at SpaceShip, reinit Search
        if ((GameObject.transform.position - target).magnitude <= 5f) target = Vector3.zero;      
        
    }
}
