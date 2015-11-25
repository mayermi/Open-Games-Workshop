using UnityEngine;
using System.Collections;

public abstract class Monster : Creature {
	
	public enum MonsterState { IDLE, CHASING, ATTACKING }
	public MonsterState state;
	public int AttackDamage { get; set; }

	public Monster(int attack, int health, float speed, int range) : base(health, speed, range) {
		AttackDamage = attack;
		state = MonsterState.IDLE;
	}

	public void Attack(Creature c)
    {
        c.TakeDamage(AttackDamage);
        state = MonsterState.ATTACKING;
    }
	
	public void Chase(Creature c)
    {
        MoveTo(c.GameObject.transform.position);
        state = MonsterState.CHASING;
        Debug.Log(this + "is chasing " + c + " to " + c.GameObject.transform.position);
        this.GameObject.GetComponent<MoveOnSphere>().SetTarget( c.GameObject.transform.position );
    }
	
	public abstract void Idle();
}
