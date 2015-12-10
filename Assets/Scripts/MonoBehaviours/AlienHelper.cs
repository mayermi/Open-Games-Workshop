using UnityEngine;
using System.Collections;

public class AlienHelper : CreatureHelper {

	Alien alien;

	void Start () {
		base.Start ();
		alien = GameObject.Find ("GameState").GetComponent<GameState>().aliens[gameObject] as Alien;
        gameObject.GetComponent<SphereCollider>().radius = alien.VisionRange;
    }

	void Update () {
		base.Update ();
		alien.Search ();
	}

    void OnTriggerEnter()
    {
        // register if a resource is near...
    }
}
