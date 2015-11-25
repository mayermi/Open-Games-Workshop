using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    GameState gs;

	// Use this for initialization
	void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
	}
	
	void FixedUpdate () {
	    foreach(DictionaryEntry monster in gs.monsters)
        {
            foreach(DictionaryEntry alien in gs.aliens)
            {
                Monster m = monster.Value as Monster;
                Alien a = alien.Value as Alien;
                if(m.IsInRange(a.GameObject)) {
                    m.Chase(a);
                } else if(m.state == Monster.MonsterState.CHASING) {
                    m.Idle();
                }
            }
        }
	}
}
