using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonsterHelper : CreatureHelper {

    GameState gs;
    Monster m;

	void Start () {
		base.Start ();
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        m = gs.monsters[gameObject] as Monster;
        gameObject.GetComponent<SphereCollider>().radius = m.VisionRange;
    }

	void Update() 
	{
		base.Update ();
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

	// this function is called by a monster after attacking
	public void StartCoolDown(float sec, Monster m)
	{
		StartCoroutine (AttackCooldown(sec, m));
	}

	IEnumerator AttackCooldown(float sec, Monster m)
	{
		yield return new WaitForSeconds (sec);
		m.AttackReady = true;
	}


}
