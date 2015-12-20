using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SkillController : MonoBehaviour {

    GameState gs;
    RecursiveLightning lightning;
	GameObject fire;
	bool fireBurning = false;
    Dictionary<Skills,bool> skillDisabled = new Dictionary<Skills,bool>();
    GrabController gc;

    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        gc = GameObject.Find("HandOfGod").GetComponent<GrabController>();
        lightning = GameObject.Find("Lightning").GetComponent<RecursiveLightning>();
		fire = GameObject.Find ("Fire");
        fire.GetComponent<ParticleSystem>().enableEmission = false;
        foreach (Skills s in Enum.GetValues(typeof(Skills)))
        {
            skillDisabled[s] = false;
        }
    }

	void FixedUpdate() {
        if (Input.GetMouseButtonDown(0) && !gc.IsGrabbing())
        {
            PerformActiveSkill();
        }

        UpdateFire();
	}

    public void PerformActiveSkill()
    {
        Skills active = (Skills)gs.ActiveSkill;
        if (skillDisabled[active])
            return;
        switch (active)
        {
            case Skills.Lightning:
                Debug.Log("Lightning");
                Lightning();
                break;
            case Skills.Fire:
                Debug.Log("Fire");
                Fire();
                break;
            case Skills.Skill3:
                Debug.Log("Nummer3");
                Nummer3();
                break;
            default:
                break;
        }
    }

    // Skill 1
    void Lightning()
    {
        skillDisabled[Skills.Fire] = true;
        // Lightning starts in Hand
        Vector3 from = GameObject.Find("HandOfGod").transform.position;
        
        // target is either a nearby monster or the ground 
        Vector3 to = CoordinateHelper.GroundPosition(from);
        Monster m = GetNearestMonster(to, 10f);
        if (m != null) to = m.GameObject.transform.position;

        lightning.firstVertexPosition = from;
        lightning.lastVertexPosition = to;
        lightning.StrikeLightning();
        if (m != null) m.TakeDamage(100);

        skillDisabled[Skills.Fire] = false;
    }

    // Skill 2
    void Fire()
    {
        skillDisabled[Skills.Fire] = true;
		GameObject hand = GameObject.Find ("HandOfGod");
		fire.transform.SetParent(hand.transform);
		//fire.transform.position = hand.transform.position;
		fireBurning = true;
		fire.GetComponent<ParticleSystem> ().enableEmission = true;
		StartCoroutine (StopFire(2f));
    }

    // Skill 3
    void Nummer3()
    {
        Debug.Log("Skill 3 triggered");
    }

    Monster GetNearestMonster(Vector3 pos, float radius)
    {
        foreach(DictionaryEntry d in gs.monsters)
        {
            Monster m = d.Value as Monster;
            float dist = (pos - m.GameObject.transform.position).magnitude;
            if(dist < radius)
            {
                return m;
            }
        }
        return null;
    }

    void UpdateFire()
    {
        GameObject planet = GameObject.Find("Planet");
        GameObject hand = GameObject.Find("HandOfGod");
        float radius = planet.GetComponent<MeshFilter>().mesh.bounds.size.x * planet.transform.localScale.x * 0.5f;
        fire.transform.position = hand.transform.position.normalized * (radius + 0.3f);
    }

    public void CreatureInFire(GameObject c)
    {
            Debug.Log("FIREEEEE");
            Creature creature = gs.creatures[c] as Creature;
            creature.TakeDamage(5);
    }

    IEnumerator StopFire(float sec)
	{
		yield return new WaitForSeconds(sec);
		fire.GetComponent<ParticleSystem> ().enableEmission = false;
		fireBurning = false;
        skillDisabled[Skills.Fire] = false;
	}

    enum Skills { Lightning, Fire, Skill3 };
}
