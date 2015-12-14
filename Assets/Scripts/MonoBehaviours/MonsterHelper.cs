using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

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
		if (m.alienTargets.Count == 0)
			m.Idle ();
		if (m.state == Monster.MonsterState.CHASING || m.state == Monster.MonsterState.ATTACKING)
			CheckDistance ();			
		
	}

    // Always looking if an Alien enters the AggroRange
	/*void OnTriggerStay(Collider other)
    {     
        if(other.gameObject.tag == "Alien") // only interested in Aliens, not other monsters
        {
            if ((transform.position - other.transform.position).magnitude <= 3f && m.state != Monster.MonsterState.IDLE)
            {
                m.Attack(gs.aliens[other.gameObject] as Alien);
            } else {
                m.Chase(gs.aliens[other.gameObject] as Alien);
            }             
        }
    }*/

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Alien") {
			m.alienTargets.Add (other.gameObject);
			m.Chase ();
		}
	}

    void OnTriggerLeave(Collider other)
    {
		if (m.alienTargets.Contains(other.gameObject)) {
			m.alienTargets.Remove(other.gameObject);
			if(m.alienTargets.Count == 0) m.Idle();
		}
    }

	void CheckDistance() {
		float dist = (transform.position - m.alienTargets.First().transform.position).magnitude;
		if (dist <= 4f)
			m.Attack (gs.creatures [m.alienTargets.First ()] as Creature);
		else if (dist > 4f)
			m.Chase ();
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
