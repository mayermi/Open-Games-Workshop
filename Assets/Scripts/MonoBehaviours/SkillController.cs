using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SkillController : MonoBehaviour {

    GameState gs;
    RecursiveLightning lightning;
	GameObject fire;
	bool fireBurning = false;
	GameObject heal;
    Dictionary<Skills,bool> skillDisabled = new Dictionary<Skills,bool>();
    GrabController gc;

	private const float HEALRADIUS = 10f;
	private const int HEALPOINTS = 50;
	private const float LIGHTNINGRADIUS = 10f;
	private const int LIGHTNINGDAMAGE = 100;

    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        gc = GameObject.Find("HandOfGod").GetComponent<GrabController>();
        lightning = GameObject.Find("Lightning").GetComponent<RecursiveLightning>();
		fire = GameObject.Find ("Fire");
        fire.GetComponent<ParticleSystem>().enableEmission = false;
		heal = GameObject.Find ("Heal");
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
            case Skills.Heal:
                Debug.Log("Heal");
                Heal();
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
		Monster m = GetNearestMonster(to, LIGHTNINGRADIUS);
        if (m != null) to = m.GameObject.transform.position;

        lightning.firstVertexPosition = from;
        lightning.lastVertexPosition = to;
        lightning.StrikeLightning();
        if (m != null) m.TakeDamage(LIGHTNINGDAMAGE);

        skillDisabled[Skills.Fire] = false;
    }

    // Skill 2
    void Fire()
    {
        skillDisabled[Skills.Fire] = true;
		GameObject hand = GameObject.Find ("HandOfGod");
		fire.transform.SetParent(hand.transform);
		fireBurning = true;
		fire.GetComponent<ParticleSystem> ().enableEmission = true;
		StartCoroutine (StopFire(2f));
    }

    // Skill 3
    void Heal()
    {
		skillDisabled[Skills.Heal] = true;
		Vector3 pos = GameObject.Find("HandOfGod").transform.position;
		pos = CoordinateHelper.GroundPosition (pos);
		heal.transform.position = pos;
		heal.transform.forward = pos.normalized;
		heal.GetComponent<ParticleSystem> ().Play ();
		foreach(DictionaryEntry d in gs.aliens)
		{
			Alien a = d.Value as Alien;
			float dist = (pos - a.GameObject.transform.position).magnitude;
			if(dist < HEALRADIUS)
			{
				a.GetHealed(HEALPOINTS);
			}
		}
		StartCoroutine (StopHeal(4f));
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

	Alien[] GetNearestAlien(Vector3 pos, float radius)
	{


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

	IEnumerator StopHeal(float sec)
	{
		yield return new WaitForSeconds(sec);
		skillDisabled[Skills.Heal] = false;
	}

    enum Skills { Lightning, Fire, Heal };
}
