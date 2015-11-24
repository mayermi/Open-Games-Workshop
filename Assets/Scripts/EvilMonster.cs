using UnityEngine;
using System.Collections;

public class EvilMonster : Monster {
	
	public EvilMonster(int attack, int health, float speed, int range) : base(attack, health, speed, range)  {
		
	}
	
	public override void Idle() {}
	
}
