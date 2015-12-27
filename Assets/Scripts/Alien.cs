using UnityEngine;
using System.Collections;

public class Alien : Creature {

	public enum AlienState { SEARCHING, FLEEING, CARRYING }
	public GameObject Resource { get; set;}
	public bool Infected { get; set;}
	public AlienState state;
	private Vector3 target;
	public bool movingToResource = false;
	private float waitTimer = -1f;
	private const float WAITTIME = 10f;

	public Alien(int health, float speed, int range) : base(health, speed, range){
		state = AlienState.SEARCHING;
		Infected = false;
	}

	public override void TakeDamage(int d, object source=null)
	{
		base.TakeDamage (d, source);
		if (source != null && source is Monster) 
		{
			state = AlienState.FLEEING;
		}
	}

	public override void GetHealed(int d, object source=null) 
	{
		if (Infected)
			Infected = false;
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
		Resource.transform.SetParent (null);
		Resource.transform.position -= new Vector3(0,1,0);
		Resource.GetComponent<Collider> ().enabled = true;
		Resource = null;
    }

	public override void Die()
	{
		base.Die ();
		if(Resource) DropResource ();
	} 

	public void Search()
	{
        state = AlienState.SEARCHING;

        // for initialisation
        if (target == Vector3.zero)
			target = GameObject.transform.position;

		if ((GameObject.transform.position - target).magnitude <= 5f)
		{
			Vector3 rndDir = new Vector3(GameObject.transform.forward.x * Random.Range(-1, 1),
			                             GameObject.transform.forward.y * Random.Range(-1, 1),
			                             GameObject.transform.forward.z * Random.Range(-1, 1));
			float distance = Random.Range(10, 60);
			target = GameObject.transform.position + ((rndDir) * distance);
			target = CoordinateHelper.GroundPosition(target);
			waitTimer = Time.time;
			MoveTo (target);
		}
	}

	public void Flee()
    {
		//Debug.Log ("Fleeing");
        state = AlienState.FLEEING;
		if(Resource) DropResource ();
		movingToResource = false;
		ReturnToShip ();

		// Alien is back at SpaceShip, waits for WAITTIME seconds
		if ((GameObject.transform.position - target).magnitude <= 5f && waitTimer == -1f)
			waitTimer = Time.time;

		if (waitTimer != -1f && Time.time - waitTimer > WAITTIME) 
		{
			state = AlienState.SEARCHING;
			waitTimer = -1f;
		}
			
    }

    public void CarryResource()
    {
        state = AlienState.CARRYING;

		ReturnToShip ();

        // Alien is back at SpaceShip, reinit Search
		if ((GameObject.transform.position - target).magnitude <= 5f)
			ResetTarget ();
        
    }

    public void ResetTarget()
    {
        target = Vector3.zero;
    }

	private void ReturnToShip() 
	{
		if (target != GameValues.ShipPos)
		{
			target = GameValues.ShipPos;
			MoveTo(target);
		}
	}
}
