using UnityEngine;
using System.Collections;

public class AlienHelper : CreatureHelper {

	Alien alien;

	void Start () {
		base.Start ();
		alien = GameObject.Find ("GameState").GetComponent<GameState>().aliens[gameObject] as Alien;
	}

	void Update () {
		base.Update ();
		alien.Search ();
	}
}
