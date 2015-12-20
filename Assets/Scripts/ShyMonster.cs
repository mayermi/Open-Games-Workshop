using UnityEngine;
using System.Collections;

public class ShyMonster : Monster {

    Vector3 target;

	public ShyMonster(int attack, int health, float speed, int range) : base(attack, health, speed, range)  {
		
	}

	public override void Idle()
    {
        // ShyMonster does not move when idle
		state = MonsterState.IDLE;
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
