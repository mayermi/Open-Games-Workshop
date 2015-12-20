using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class Monster : Creature {
	
	public enum MonsterState { IDLE, CHASING, ATTACKING, GRABBED }
	public MonsterState state;
	public int AttackDamage { get; set; }
	public bool AttackReady { get; set; }
	public List<GameObject> alienTargets;

	public Monster(int attack, int health, float speed, int range) : base(health, speed, range) {
		AttackDamage = attack;
		state = MonsterState.IDLE;
		AttackReady = true;
		alienTargets = new List<GameObject> ();
	}

	public void Attack(Creature c)
    {   
		if (AttackReady) {
			c.TakeDamage(AttackDamage);
			state = MonsterState.ATTACKING;
            MoveTo(c.GameObject.transform.position);
            Debug.Log(this + "is attacking " + c);

			// Start cooldown of attack
			AttackReady = false;
			GameObject.GetComponent<MonsterHelper>().StartCoolDown(1, this);
		}
    }
	
	public void Chase()
    {
		//Debug.Log(this + "is chasing ");
        MoveTo(alienTargets.First().transform.position);
        state = MonsterState.CHASING;
    }

    public virtual void GetGrabbed()
    {
        state = MonsterState.GRABBED;
        StopMoving();
        Debug.Log(this + " is grabbed");
    }

    public abstract void ResetTarget();
	
	public abstract void Idle();


}
