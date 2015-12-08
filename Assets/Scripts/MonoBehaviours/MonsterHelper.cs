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
            if (CoordinateHelper.calcDistance(transform.position, other.transform.position) <= 1.5f)
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
