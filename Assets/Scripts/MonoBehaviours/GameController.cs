using UnityEngine;
using System.Collections;
using Pantheon.Utils;

public class GameController : MonoBehaviour {

    GameState gs;
    SkillController sc;
    GameObject planet;
    float lastSpawn;
    float spawnTimer;

    // Use this for initialization
    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        sc = GameObject.Find("SkillController").GetComponent<SkillController>();
        planet = GameObject.Find("Planet");
        GameValues.PlanetRadius = planet.GetComponent<MeshFilter>().mesh.bounds.size.x * 0.5f * planet.transform.localScale.x;
        gs.ActiveSkill = 0;
        lastSpawn = Time.time;
        spawnTimer = Time.time;
        // Create planet landscape
		planet.GetComponent<RandomObjectScattering> ().Setup ();
        // Init pathfinding
		if(GameObject.Find("PathFinding")) GameObject.Find("PathFinding").GetComponent<SphericalGrid>().BakeNodeProcess();

		SpawnAliens (gs.maxAliens);
	}

    void Update()
    {    
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            gs.ActiveSkill += 1;
            if (gs.ActiveSkill > 2) gs.ActiveSkill = 0;
			Debug.Log(gs.ActiveSkill);
        }

        // every two seconds there is a chance for a monster spawn
        if (Time.time - spawnTimer > 2)
        {
            //DecideMonsterSpawning();
            spawnTimer = Time.time;
        }
    }

    void SpawnAliens(int count) 
	{
		for (int i = 0; i < count; i++) {
			var rotation = Random.Range(0.03f, 0.06f);
			Vector3 sec_pos = Vector3.RotateTowards(GameValues.ShipPos, new Vector3(0,0,1) * GameValues.ShipPos.magnitude, rotation, GameValues.ShipPos.magnitude);
			float angle = i * 360.0f/count;
			
			var detail_pos = Quaternion.AngleAxis(angle, GameValues.ShipPos) * sec_pos;

			Alien a = new Alien (health: 100, speed: 0.15f, range: 7);
			a.GameObject = Creator.Create ("Alien", detail_pos, "Alien");
			gs.aliens.Add (a.GameObject, a);
			gs.creatures.Add(a.GameObject, a as Creature);
			a.GameObject.transform.up = -(transform.position - GameValues.ShipPos).normalized;
			a.Search();
		}
	}

    void DecideMonsterSpawning()
    {
        float rand = Random.Range(0f, 1f);
        float value = Mathf.Pow(1.0116f, Time.time-lastSpawn)- 1;
        if(rand < value)
        {
            DecideMonsterFamily();
            lastSpawn = Time.time;
        }
    }

    void DecideMonsterFamily()
    {
        var r = Random.Range(0.0f, 1.0f);
        if (r > 0.66f)
            SpawnShyMonsters();
        else if (r > 0.33f)
            SpawnPredators();
        else
            SpawnEvilMonsters();
    }

    void SpawnShyMonsters()
    {
        int count = Random.Range(1, 2);
        Debug.Log("Spawning " + count + " ShyMonsters");
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = gs.MonsterSpawnPoints.Any();
            ShyMonster m = new ShyMonster(15, 100, 0.2f, 10);
            m.GameObject = Creator.Create("monster", pos, "ShyMonster");
            gs.monsters.Add(m.GameObject, m);
            gs.creatures.Add(m.GameObject, m as Creature);

            GameObject effect = Creator.Create("Spawner", pos, "Spawner");
            effect.transform.forward = -(planet.transform.position - pos).normalized;
        }
    }

    void SpawnPredators()
    {
        int count = Random.Range(1,5);
        Vector3 pos = gs.MonsterSpawnPoints.Any();
        Debug.Log("Spawning " + count + " Predators");
        for (int i = 0; i < count; i++)
        {        
            Vector3 spawnPos = pos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            spawnPos = CoordinateHelper.GroundPosition(spawnPos);
            PredatoryMonster m = new PredatoryMonster(15, 100, 0.2f, 10);
            m.GameObject = Creator.Create("monster_small", spawnPos, "PredatoryMonster");
            gs.monsters.Add(m.GameObject, m);
            gs.creatures.Add(m.GameObject, m as Creature);

            GameObject effect = Creator.Create("Spawner", pos, "Spawner");
            effect.transform.forward = -(planet.transform.position -pos).normalized;
        }
    }

    void SpawnEvilMonsters()
    {
        int count = Random.Range(1, 10);
        Debug.Log("Spawning " + count + " EvilMonsters");
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = gs.MonsterSpawnPoints.Any();
            EvilMonster m = new EvilMonster(15, 100, 0.2f, 10);
            m.GameObject = Creator.Create("monster", pos, "ShyMonster");
            gs.monsters.Add(m.GameObject, m);
            gs.creatures.Add(m.GameObject, m as Creature);

            GameObject effect = Creator.Create("Spawner", pos, "Spawner");
            effect.transform.forward = -(planet.transform.position - pos).normalized;
        }
    }

    void RemoveReferences(Creature c) {
		// If the creature is an alien, tell the chasing monster that the target is dead
		foreach (DictionaryEntry d in gs.monsters) {
			Monster m = d.Value as Monster;
			if(m.alienTargets.Contains (c.GameObject)) {
				m.alienTargets.Remove(c.GameObject);
			}
		}
	}
}
