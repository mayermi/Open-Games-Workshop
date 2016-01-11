using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pantheon.Utils;

public class GameController : MonoBehaviour {

    GameState gs;
    SkillController sc;
	GrabController gc;
    TutorialController tc;
    UIManager ui;   
    GameObject planet;
	public AudioClip crashSpaceship;

    float lastSpawn;
    float bakeTimer;
   
    bool readyToBakePathfinding = false;
    bool bakingDone = false;
    bool baking = false;
    bool firstSpawn = true;

	private AudioSource source;
	private float volLowRange = .5f;
	private float volHighRange = 1.0f;
	private float vol;

    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        sc = GameObject.Find("SkillController").GetComponent<SkillController>();
		gc = GameObject.Find("HandOfGod").GetComponent<GrabController>();
        tc = GameObject.Find("Tutorials").GetComponent<TutorialController>();
        ui = GameObject.Find("UI").GetComponent<UIManager>();
        planet = GameObject.Find("Planet");
        GameValues.PlanetRadius = planet.GetComponent<MeshFilter>().mesh.bounds.size.x * 0.5f * planet.transform.localScale.x;

		source = GetComponent<AudioSource>();
		vol = UnityEngine.Random.Range (volLowRange, volHighRange);

        gs.ActiveSkill = 0;
        gs.CollectedResources = 0;
        bakeTimer = Time.time;
      
        // Create planet landscape
        planet.layer = 10;
        planet.GetComponent<RandomObjectScattering> ().Setup ();
        

        readyToBakePathfinding = true;
	}


    void Update()
    {    
        if(readyToBakePathfinding && !bakingDone && !baking && Time.time - bakeTimer > 1f)
        {
            // Init pathfinding
            Debug.Log("started baking");
            baking = true;
            StartCoroutine(BakeNodes());         
        }

        if(bakingDone && !gs.gameReady)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GameObject.Find("StoryCanvas").SetActive(false);
                ui.SetResourceSlider();
                gs.gameReady = true;
                tc.ShowNavigation();               
            }
                
        }
         
        if(gs.gameReady)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                gs.ActiveSkill += 1;
                if (gs.ActiveSkill > 2) gs.ActiveSkill = 0;
				ui.UpdateSkillToggle();
            }

            if(!firstSpawn && gs.aliens.Count == 0)
            {
                if (gs.aliensSaved > 0)
                    ui.showWin();
                else
                    ui.showLose();
            }
        }       

        if (Input.GetKeyDown (KeyCode.O)) {
			StartCoroutine (CrashSpaceShip ());
			source.PlayOneShot(crashSpaceship,vol);
		}

    }

    IEnumerator BakeNodes()
    {
        bool finished = false;
        finished = GameObject.Find("PathFinding").GetComponent<SphericalGrid>().BakeNodeProcess();
        while (!finished)
            yield return null;
        bakingDone = true;

        GameObject.Find("StoryCanvas").SendMessage("LoadingDone");
    }

    IEnumerator MonsterSpawning()
    {
        while(true)
        {
            DecideMonsterSpawning();
            yield return new WaitForSeconds(GameValues.SPAWNTIME);
        }
    }

    IEnumerator CrashSpaceShip()
    {
        
        Vector3 pos = GameValues.ShipPos;
        Vector3 start_pos = (pos.normalized + new Vector3(0.1f, 0, 0)) * GameValues.PlanetRadius * 5f;
        GameObject ship = Creator.Create("Spaceship_whole", start_pos, "SpaceShip");
        Vector3 up = pos.normalized;
        ship.transform.right = -up;

        Camera.main.GetComponent<CameraRotation>().FocusOnPoint(pos, 35f);

        yield return new WaitForSeconds(1f);

        float startTime = Time.time;

        while(Time.time - startTime < 1.5f)
        {
            float frac = (Time.time - startTime) / 1.5f;
            ship.transform.position = Vector3.Lerp(start_pos, pos, frac);
            yield return false;
        }

        Destroy(ship);

        GameObject explosion = Creator.Create("Explosion", pos, "Explosion");
        explosion.transform.up = up;

        GameObject new_ship = Creator.Create("Spaceship_broken", pos, "SpaceShip");
        new_ship.transform.up = up;

        SpawnAliens(gs.maxAliens);
        lastSpawn = Time.time;
        StartCoroutine(MonsterSpawning());

        yield return new WaitForSeconds(3f);
        tc.ShowStory();
    }


    void SpawnAliens(int count) 
	{
		for (int i = 0; i < count; i++) {
			var rotation = Random.Range(0.03f, 0.1f);
			Vector3 sec_pos = Vector3.RotateTowards(GameValues.ShipPos, new Vector3(0,0,5) * GameValues.ShipPos.magnitude, rotation, GameValues.ShipPos.magnitude);
			float angle = i * 360.0f/count;
			
			var detail_pos = Quaternion.AngleAxis(angle, GameValues.ShipPos) * sec_pos;

			Alien a = new Alien (health: 100, speed: 2f, range: 7);
			a.GameObject = Creator.Create ("Alien", detail_pos, "Alien");
			gs.aliens.Add (a.GameObject, a);
			gs.creatures.Add(a.GameObject, a as Creature);
			a.GameObject.transform.up = -(transform.position - GameValues.ShipPos).normalized;
			a.Search();
		}

        // Tell UI that aliens have been spawned
        ui.SetAlienSlider();
        ui.SetResourceSlider();
    }

    void DecideMonsterSpawning()
    {
        float rand = Random.Range(0f, 1f);
        float value = Mathf.Pow(1.00116f, Time.time-lastSpawn)- 1;
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

        if (firstSpawn)
        {
            Vector3 camPos = gs.GetFirstMonster().transform.position;
            tc.ShowMonsters(camPos);
            firstSpawn = false;
        }
    }

    void SpawnShyMonsters()
    {
        int count = Random.Range(1, 2);
        Debug.Log("Spawning " + count + " ShyMonsters");
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = gs.MonsterSpawnPoints.Any();
            ShyMonster m = new ShyMonster(attack: 10, health: 25, speed: 3f, range: 7, contagious: false);
            m.GameObject = Creator.Create("mahluq", pos, "ShyMonster");
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
			bool contagious = false;
			if(Random.Range(0f,1f) < 0.1f) contagious = true;
            PredatoryMonster m = new PredatoryMonster(attack: 7, health: 50, speed: 3.5f, range: 10, contagious: contagious);
            m.GameObject = Creator.Create("monster_small", spawnPos, "PredatoryMonster");
            gs.monsters.Add(m.GameObject, m);
            gs.creatures.Add(m.GameObject, m as Creature);
			if(contagious)
				m.GameObject.transform.Find ("Infection").GetComponent<ParticleSystem>().Play();

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
            EvilMonster m = new EvilMonster(attack: 10, health: 100, speed: 2.5f, range: 8, contagious: false);
            m.GameObject = Creator.Create("monster", pos, "ShyMonster");
            gs.monsters.Add(m.GameObject, m);
            gs.creatures.Add(m.GameObject, m as Creature);

            GameObject effect = Creator.Create("Spawner", pos, "Spawner");
            effect.transform.forward = -(planet.transform.position - pos).normalized;
        }
    }

    void RemoveReferences(Creature c) {
		// If the creature is an alien, tell the chasing monster that the target is dead
		if (c is Alien)
		{
			foreach (DictionaryEntry d in gs.monsters) {
				Monster m = d.Value as Monster;
				if (m.alienTargets.Contains (c.GameObject)) {
					m.alienTargets.Remove (c.GameObject);
				}
			}
		}

		if (gc.objectToBeGrabbed == c.GameObject)
			gc.objectToBeGrabbed = null;
	}

    

}
