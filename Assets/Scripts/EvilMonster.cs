using UnityEngine;
using System.Collections;

public class EvilMonster : Monster {

    Vector3 target;

	public EvilMonster(int attack, int health, float speed, int range, bool contagious) : base(attack, health, speed, range, contagious)  {
		
	}
	
	public override void Idle() {
        state = MonsterState.IDLE;

        if(target != GameValues.ShipPos)
        {
            target = GameValues.ShipPos;
            MoveTo(target);
        }
    }

    public override void GetGrabbed()
    {
        base.GetGrabbed();
        StopMoving();
        ResetTarget();
    }

    public override void ResetTarget()
    {
        target = Vector3.zero;
    }

}
