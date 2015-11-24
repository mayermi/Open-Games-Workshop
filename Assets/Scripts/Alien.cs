using UnityEngine;
using System.Collections;

public class Alien : Creature {

	public enum AlienState { SEARCHING, FLEEING }
	private GameObject Resource;
	private AlienState state;

	public Alien(int health, float speed, int range) : base(health, speed, range){
		this.state = AlienState.SEARCHING;
	}

	public void TakeResource(GameObject res) {}

	public void DropResource() {}

	public void Search() {}

	public void Flee() {}
}
