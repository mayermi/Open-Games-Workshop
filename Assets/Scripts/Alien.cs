using UnityEngine;
using System.Collections;

public class Alien : Creature {

	public enum AlienState { SEARCHING, FLEEING }
	public GameObject Resource { get; set;}
	private AlienState state;
	private Vector3 target;

	public Alien(int health, float speed, int range) : base(health, speed, range){
		this.state = AlienState.SEARCHING;
	}

	public void TakeResource(GameObject res)
    {
		Resource = res;
	}

	public void DropResource()
    {
        Resource = null;
    }

	public void Search()
	{
		// for initialisation
		if (target == Vector3.zero)
			target = GameObject.transform.position;

		//Debug.Log ((GameObject.transform.position - target).sqrMagnitude);

		if ((GameObject.transform.position - target).sqrMagnitude <= 50f)
		{
			Vector3 rndDir = new Vector3(GameObject.transform.forward.x * Random.Range(-1, 1),
			                             GameObject.transform.forward.y * Random.Range(-1, 1),
			                             GameObject.transform.forward.z * Random.Range(-1, 1));
			float distance = Random.Range(1, 60);
			target = GameObject.transform.position + ((rndDir) * distance);
			target = CoordinateHelper.GroundPosition(target);
			MoveTo (target);
			Debug.Log ("new target: " + target);
		}
	}

	public void Flee() {}
}
