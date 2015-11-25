using UnityEngine;
using System.Collections;

public class ShyMonster : Monster {

	public ShyMonster(int attack, int health, float speed, int range) : base(attack, health, speed, range)  {
		
	}

	public override void Idle()
    {
        this.GameObject.GetComponent<MoveOnSphere>().target = this.GameObject.transform.position;
    }

}
