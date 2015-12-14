using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    GameState gs;
    SkillController sc;
    GameObject planet;   

    // Use this for initialization
    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        sc = GameObject.Find("SkillController").GetComponent<SkillController>();
        planet = GameObject.Find("Planet");
		GameValues.PlanetRadius = planet.GetComponent<MeshFilter>().mesh.bounds.size.x * 0.5f * planet.transform.localScale.x;
        gs.ActiveSkill = 0;
        // Create planet landscape
		planet.GetComponent<RandomObjectScattering> ().Setup ();
        // Init pathfinding
		if(GameObject.Find("PathFinding")) GameObject.Find("PathFinding").GetComponent<SphericalGrid>().BakeNodeProcess();

		SpawnAliens (gs.maxAliens);
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            sc.PerformActiveSkill();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            gs.ActiveSkill += 1;
            if (gs.ActiveSkill > 2) gs.ActiveSkill = 0;
			Debug.Log(gs.ActiveSkill);
        }
    }

	void SpawnAliens(int count) 
	{
		for (int i = 0; i < count; i++) {
			var rotation = Random.Range(0.03f, 0.06f);
			Vector3 sec_pos = Vector3.RotateTowards(gs.ShipPos, new Vector3(0,0,1) * gs.ShipPos.magnitude, rotation, gs.ShipPos.magnitude);
			float angle = i * 360.0f/count;
			
			var detail_pos = Quaternion.AngleAxis(angle, gs.ShipPos) * sec_pos;

			Alien a = new Alien (health: 100, speed: 0.15f, range: 7);
			a.GameObject = Creator.Create ("Alien", detail_pos, "Alien");
			gs.aliens.Add (a.GameObject, a);
			gs.creatures.Add(a.GameObject, a as Creature);
			a.GameObject.transform.up = -(transform.position - gs.ShipPos).normalized;
			a.Search();
		}
	}

      
	void RemoveReferences(Creature c) {
		// If the creature is an alien, tell the chasing monster that the target is dead
		foreach (DictionaryEntry d in gs.monsters) {
			Monster m = d.Value as Monster;
			if(m.alienTargets.Contains (c.GameObject)) {
				Debug.Log ("Removed alientarget");
				m.alienTargets.Remove(c.GameObject);
			}

		}
	}
}
