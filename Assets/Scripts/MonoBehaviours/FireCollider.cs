using UnityEngine;
using System.Collections;

public class FireCollider : MonoBehaviour {

    SkillController sc;

    void Start()
    {
        sc = GameObject.Find("SkillController").GetComponent<SkillController>();
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log(other);
        sc.CreatureInFire(other);
    }
}
