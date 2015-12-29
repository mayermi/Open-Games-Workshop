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
	public const float COOLDOWN = 1f;
	public bool isContagious;

	public Monster(int attack, int health, float speed, int range, bool contagious) : base(health, speed, range) {
		AttackDamage = attack;
		state = MonsterState.IDLE;
		AttackReady = true;
		alienTargets = new List<GameObject> ();
		isContagious = contagious;
	}

	public void Attack(Alien a)
    {   
		if (AttackReady) {
			state = MonsterState.ATTACKING;
			a.TakeDamage(AttackDamage, this);
			if(isContagious && !a.Infected) a.Infected = true;

            MoveTo(a.GameObject.transform.position);

			// Start cooldown of attack
			AttackReady = false;
			GameObject.GetComponent<MonsterHelper>().StartCoolDown(COOLDOWN, this);
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
        Debug.Log(this + " is grabbed");
    }

    public abstract void ResetTarget();
	
	public abstract void Idle();


}
