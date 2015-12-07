using UnityEngine;
using System.Collections;

public class ShyMonster : Monster {

	public ShyMonster(int attack, int health, float speed, int range) : base(attack, health, speed, range)  {
		
	}

	public override void Idle()
    {
        // ShyMonster does not move when idle
        //GameObject.GetComponent<MoveOnSphere>().RunningLocked = true;

        // delete these two lines after wayfinding is implemented
        GameObject.GetComponent<MoveOnSphere>().RunningLocked = false;
        GameObject.GetComponent<MoveOnSphere>().SetTarget(GameObject.transform.position);
    }

}
