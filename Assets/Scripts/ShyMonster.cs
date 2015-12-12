using UnityEngine;
using System.Collections;

public class ShyMonster : Monster {

	public ShyMonster(int attack, int health, float speed, int range) : base(attack, health, speed, range)  {
		
	}

	public override void Idle()
    {
        // ShyMonster does not move when idle
		state = MonsterState.IDLE;
    }

}
