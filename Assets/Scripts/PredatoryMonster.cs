using UnityEngine;
using System.Collections;

public class PredatoryMonster : Monster {

    private Vector3 target;

    public PredatoryMonster(int attack, int health, float speed, int range) : base(attack, health, speed, range)  {
		
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
            float distance = Random.Range(10, 60);
            target = GameObject.transform.position + ((rndDir) * distance);
            target = CoordinateHelper.GroundPosition(target);
            Debug.Log("Monster has new target: " + target);
            MoveTo(target);
        }
    }
	
}
