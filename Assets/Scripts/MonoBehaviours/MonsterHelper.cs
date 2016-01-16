using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class MonsterHelper : CreatureHelper {

    Monster m;

	public override void Start () {
		base.Start ();
        m = gs.monsters[gameObject] as Monster;
        gameObject.GetComponent<SphereCollider>().radius = m.VisionRange;
    }

	public override void Update() 
	{
		base.Update ();

        if (m == null)
        {
            m = gs.monsters[gameObject] as Monster;
        } else if (m.alienTargets.Count == 0 && m.state != Monster.MonsterState.GRABBED)
        {
            m.Idle();
        } else if ( m.state != Monster.MonsterState.GRABBED &&
                    (   
                        m.state == Monster.MonsterState.CHASING ||
                        m.state == Monster.MonsterState.ATTACKING || 
                        (m.state == Monster.MonsterState.IDLE && m.alienTargets.Count > 0)
                    )
                  )
        {
            CheckDistance();
        }         

    }

    public override void NoPathFound()
    {
        base.NoPathFound();
        m.ResetTarget();
    }

    void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Alien" && !m.alienTargets.Contains(other.gameObject)) {
			m.alienTargets.Add (other.gameObject);
			if(m.state != Monster.MonsterState.GRABBED) m.Chase ();
		}
	}

    void OnTriggerExit(Collider other)
    {
		if (other.gameObject.tag == "Alien" && m.alienTargets.Contains(other.gameObject)) {
			m.alienTargets.Remove(other.gameObject);
			if(m.alienTargets.Count == 0 && m.state != Monster.MonsterState.GRABBED) m.Idle();
		}
    }

	void CheckDistance() {
		float dist = (transform.position - m.alienTargets.First ().transform.position).magnitude;
		if (dist <= 4f) {
			m.Attack (gs.creatures [m.alienTargets.First ()] as Alien);
			//source.Play();
		}
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
