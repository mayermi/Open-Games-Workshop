using UnityEngine;
using System.Collections;

public class Alien : Creature {

	public enum AlienState { SEARCHING, FLEEING }
	public GameObject Resource { get; set;}
	private AlienState state;

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

	public void Search() {}

	public void Flee() {}
}
