using UnityEngine;
using System.Collections;

public class PredatoryMonster : Monster {

    private Vector3 target;

	public PredatoryMonster(int attack, int health, float speed, int range, bool contagious) : base(attack, health, speed, range, contagious) {

    }

    public override void Idle() {
        state = MonsterState.IDLE;

        // for initialisation
        if (target == Vector3.zero)
            target = GameObject.transform.position;

        if ((GameObject.transform.position - target).magnitude <= 8f)
        {
            Vector3 rndDir = new Vector3(GameObject.transform.forward.x * Random.Range(-1, 1),
                                         GameObject.transform.forward.y * Random.Range(-1, 1),
                                         GameObject.transform.forward.z * Random.Range(-1, 1));
            float distance = Random.Range(5, 40);
            target = GameObject.transform.position + ((rndDir) * distance);
            target = CoordinateHelper.GroundPosition(target);
            MoveTo(target);
        }
    }

    public override void GetGrabbed() {
        base.GetGrabbed();
        StopMoving();
        ResetTarget();
    }

    public override void ResetTarget()
    {
        target = Vector3.zero;
    }

}
