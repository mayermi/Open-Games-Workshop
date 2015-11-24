using UnityEngine;
using System.Collections;

public class EvilMonster : Monster {
	
	public EvilMonster(int attack, int health, float speed, int range) : base(health, speed, range)  {
		
	}
	
	public void Idle() {}
	
}
