using UnityEngine;
using System.Collections;

public abstract class Monster : Creature {
	
	public enum MonsterState { IDLE, CHASING, ATTACKING }
	private MonsterState state;
	public int AttackDamage { get; set; }

	public Monster(int attack, int health, float speed, int range) : base(health, speed, range) {
		AttackDamage = attack;
		state = MonsterState.IDLE;
	}

	public void Attack(Creature c) {}
	
	public void Chase(Creature c) {}
	
	public abstract void Idle();
}
