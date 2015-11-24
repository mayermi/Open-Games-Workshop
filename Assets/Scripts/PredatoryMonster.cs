using UnityEngine;
using System.Collections;

public class PredatoryMonster : Monster {
	
	public PredatoryMonster(int attack, int health, float speed, int range) : base(attack, health, speed, range)  {
		
	}
	
	public override void Idle() {}
	
}
