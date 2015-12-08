using UnityEngine;
using System.Collections;

public abstract class Monster : Creature {
	
	public enum MonsterState { IDLE, CHASING, ATTACKING, GRABBED }
	public MonsterState state;
	public int AttackDamage { get; set; }
	public bool AttackReady { get; set; }

	public Monster(int attack, int health, float speed, int range) : base(health, speed, range) {
		AttackDamage = attack;
		state = MonsterState.IDLE;
		AttackReady = true;
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
	
	public void Chase(Creature c)
    {
		Debug.Log(this + "is chasing " + c + " to " + c.GameObject.transform.position);
        MoveTo(c.GameObject.transform.position);
        state = MonsterState.CHASING;
    }

    public void GetGrabbed()
    {
        state = MonsterState.GRABBED;
        Debug.Log(this + " is grabbed");
    }
	
	public abstract void Idle();


}
