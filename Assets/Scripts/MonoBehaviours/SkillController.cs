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

    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        lightning = GameObject.Find("Lightning").GetComponent<RecursiveLightning>();
		fire = GameObject.Find ("Fire");
        fire.GetComponent<ParticleSystem>().enableEmission = false;
        foreach (Skills s in Enum.GetValues(typeof(Skills)))
        {
            skillDisabled[s] = false;
        }
    }

	void FixedUpdate() {
        UpdateFire();
	}

    public void PerformActiveSkill()
    {
        Skills active = (Skills) gs.ActiveSkill;
        if (skillDisabled[active])
            return;
        switch(active)
        {
            case Skills.Lightning:
                Lightning();
                break;
            case Skills.Fire:
                Fire();
                break;
            case Skills.Skill3:
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
        // Start and end positions are placeholders at the moment
        Vector3 from = GameObject.Find("HandOfGod").transform.position + new Vector3(0, 1, 0);
        Vector3 to = GameObject.Find("monster(Clone)").transform.position;
        lightning.firstVertexPosition = from;
        lightning.lastVertexPosition = to;
        lightning.StrikeLightning();
        CauseDamage(to, 5f, 25);
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
		//CauseDamage(hand.transform.position, 5f, 25);
		StartCoroutine (StopFire(2f));
    }

    // Skill 3
    void Nummer3()
    {
        Debug.Log("Skill 3 triggered");
    }

    void CauseDamage(Vector3 pos, float radius, int damage)
    {
        foreach(DictionaryEntry d in gs.monsters)
        {
            Monster m = d.Value as Monster;
            float dist = (pos - m.GameObject.transform.position).magnitude;
            if(dist < radius)
            {
                m.TakeDamage(damage);
            }
        }
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
            Debug.Log(c);
            Creature creature = gs.creatures[c] as Creature;
            creature.TakeDamage(1);
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
