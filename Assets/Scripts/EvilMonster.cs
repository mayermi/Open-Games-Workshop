using UnityEngine;
using System.Collections;

public class EvilMonster : Monster {

    Vector3 target;

	public EvilMonster(int attack, int health, float speed, int range) : base(attack, health, speed, range)  {
		
	}
	
	public override void Idle() {
        state = MonsterState.IDLE;

        if(target != GameValues.ShipPos)
        {
            target = GameValues.ShipPos;
            MoveTo(target);
        }
    }
	
}
