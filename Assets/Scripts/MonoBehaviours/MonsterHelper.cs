using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonsterHelper : MonoBehaviour {

    GameState gs;
    Monster m;

	void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        m = gs.monsters[gameObject] as Monster;
        gameObject.GetComponent<SphereCollider>().radius = m.VisionRange;
    }

    // Always looking if an Alien enters the AggroRange
	void OnTriggerStay(Collider other)
    {     
        if(other.gameObject.tag == "Alien") // only interested in Aliens, not other monsters
        {
			//Debug.Log ((transform.position - other.transform.position).sqrMagnitude);
            if ((transform.position - other.transform.position).sqrMagnitude <= 12f)
            {
                m.Attack(gs.aliens[other.gameObject] as Alien);
            } else {
                m.Chase(gs.aliens[other.gameObject] as Alien);
            }             
        }
    }

    void OnTriggerLeave(Collider other)
    {
        m.Idle();
    }


}
