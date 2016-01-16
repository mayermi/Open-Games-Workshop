using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SkillController : MonoBehaviour {

    GameState gs;
	GrabController gc;
	UIManager ui;
    GameObject hand;
    RecursiveLightning lightning;
	GameObject fire;
	GameObject heal;
	Dictionary<Skills,bool> skillDisabled = new Dictionary<Skills,bool> (); 

	private int skillPerformed = 0;

	private const float HEALRADIUS = 10f;
	private const int HEALPOINTS = 50;
	private const float HEAL_TIMEOUT = 10f;

	private const float LIGHTNINGRADIUS = 10f;
	private const int LIGHTNINGDAMAGE = 100;
	private const float LIGHTNING_TIMEOUT = 10f;

	private const float FIRE_TIMEOUT = 7f;

	public AudioClip lightningSound;
	public AudioClip fireSound;
	public AudioClip healSound;
	private AudioSource source;
	private float volLowRange = .5f;
	private float volHighRange = 1.0f;
	private float vol;


    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        gc = GameObject.Find("HandOfGod").GetComponent<GrabController>();
		ui = GameObject.Find ("UI").GetComponent<UIManager> ();

		lightning = GameObject.Find("Lightning").GetComponent<RecursiveLightning>();
		fire = GameObject.Find ("Fire");
        fire.GetComponent<ParticleSystem>().enableEmission = false;
		heal = GameObject.Find ("Heal");

        foreach (Skills s in Enum.GetValues(typeof(Skills)))
        {
            skillDisabled[s] = false;
        }

		source = GetComponent<AudioSource>();
		vol = UnityEngine.Random.Range (volLowRange, volHighRange);
    }

	void FixedUpdate() {
        if (Input.GetMouseButtonDown(0) && !gc.IsGrabbing() && gs.gameReady)
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

        GameObject.Find("hand").GetComponent<Animation>().Play("Skill");

        switch (active)
        {
            case Skills.Lightning:
                Debug.Log("Lightning");
                Lightning();
				source.PlayOneShot(lightningSound,vol);
                break;
            case Skills.Fire:
                Debug.Log("Fire");
                Fire();
				source.PlayOneShot(fireSound,vol);
                break;
            case Skills.Heal:
                Debug.Log("Heal");
                Heal();
				source.PlayOneShot(healSound,vol);
                break;
            default:
                break;
        }
    }

	public void PerformActiveSkillWithRealsense()
	{
		skillPerformed++;
		if (skillPerformed % 2 != 0)
			PerformActiveSkill ();
	}

    // Skill 1
    void Lightning()
    {
        skillDisabled[Skills.Lightning] = true;
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

		StartCoroutine (SkillTimeout (Skills.Lightning, LIGHTNING_TIMEOUT));

    }

    // Skill 2
    void Fire()
    {
        skillDisabled[Skills.Fire] = true;
		GameObject hand = GameObject.Find ("HandOfGod");
		fire.transform.SetParent(hand.transform);
		fire.GetComponent<ParticleSystem> ().enableEmission = true;
		StartCoroutine (StopFire(1.25f));
		StartCoroutine (SkillTimeout (Skills.Fire, FIRE_TIMEOUT));
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
		StartCoroutine (SkillTimeout(Skills.Heal, HEAL_TIMEOUT));
    }

    Monster GetNearestMonster(Vector3 pos, float radius)
    {
        Monster m = null;
        foreach(DictionaryEntry d in gs.monsters)
        {
            m = d.Value as Monster;
            float dist = (pos - m.GameObject.transform.position).magnitude;
            if(dist < radius)
            {
                break;
            }
        }
        return m;
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
            Creature creature = gs.creatures[c] as Creature;
            creature.TakeDamage(5);
    }

    IEnumerator StopFire(float sec)
	{
		yield return new WaitForSeconds(sec);
		fire.GetComponent<ParticleSystem> ().enableEmission = false;;
	}

	IEnumerator SkillTimeout(Skills s, float timeout) 
	{
		float start = Time.time;

		while ((Time.time - start) < timeout)
		{
			ui.UpdateTimeout((int)s, (timeout - (Time.time - start)) / timeout );
			yield return false;
		}

		skillDisabled [s] = false;
	}
	

    enum Skills { Lightning, Fire, Heal };
}
