using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pantheon.Utils;

public class GameController : MonoBehaviour {

    GameState gs;
	GrabController gc;
    TutorialController tc;
    UIManager ui;   
    GameObject planet;
	

    public const float CRASH_SPACESHIP_AFTER_SECONDS = 60f;

    float lastSpawn;
    float bakeTimer;
    float gsReady = 0.0f;
   
    bool readyToBakePathfinding = false;
    bool bakingDone = false;
    bool baking = false;
    bool firstSpawn = true;
    bool spaceshipCrashed = false;

	private AudioSource source;
    private AudioSource attackSource;
    private AudioSource musicsource;
    private AudioSource jetStartSource;

	public AudioClip attackSound;
    public AudioClip jetstartSound;
    public AudioClip crashSpaceship;
	public AudioClip alarmMusic;
	public AudioClip backgroundMusic;
	public AudioClip finalMusic;
	public AudioClip sadFinalMusic;
    private SoundHelper sh;

    List<Alien> fleeingAliens = new List<Alien>();
    private bool fleeing = false;
    private int actionSwitched = 0;

    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
		gc = GameObject.Find("HandOfGod").GetComponent<GrabController>();
        tc = GameObject.Find("Tutorials").GetComponent<TutorialController>();
        ui = GameObject.Find("UI").GetComponent<UIManager>();
        sh = GameObject.Find("SoundHelper").GetComponent<SoundHelper>();
        planet = GameObject.Find("Planet");

        musicsource = gameObject.GetComponent<AudioSource>();
        StartCoroutine(startMusic());

        source = gameObject.AddComponent<AudioSource>();
        source.clip = crashSpaceship;
        source.loop = false;
        attackSource = gameObject.AddComponent<AudioSource>();
        attackSource.clip = attackSound;
        attackSource.loop = false;
        jetStartSource = gameObject.AddComponent<AudioSource>();
        jetStartSource.clip = jetstartSound;
        jetStartSource.loop = false;

        gs.ActiveSkill = 0;
        gs.CollectedResources = 0;
        bakeTimer = Time.time;
      
        // Create planet landscape
        planet.layer = 10;
        planet.GetComponent<RandomObjectScattering> ().Setup ();
        

        readyToBakePathfinding = true;
	}

    IEnumerator startMusic()
    {
        sh.Fade(musicsource, true, 3);
        //musicsource.Play();
        yield return true;
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
            gsReady = gsReady > 0.0f ? gsReady : Time.time;
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				ui.TogglePause();
			}
			
			if (Input.GetKeyDown(KeyCode.Tab))
            {
                gs.ActiveSkill += 1;
                if (gs.ActiveSkill > 2) gs.ActiveSkill = 0;
				ui.UpdateSkillToggle();
            }

            if (!firstSpawn && gs.aliens.Count == 0)
            {
                if (gs.aliensSaved > 0) {
                    StartCoroutine(Win());
            }else {
                    StartCoroutine(playLoseSound());
                    ui.ShowLose();
                }
            }
        }

        var timeSinceReady = Time.time - gsReady;

        if (Input.GetKeyDown (KeyCode.O) || timeSinceReady > CRASH_SPACESHIP_AFTER_SECONDS ) {
			StartCoroutine (CrashSpaceShip ());
        }

        List<Alien> aliens = gs.getAliens ();

		if (!fleeing) {
			foreach (Alien alien in aliens) {
				if (alien.state == Alien.AlienState.FLEEING && !fleeingAliens.Contains(alien)) {
					fleeingAliens.Add (alien);
					fleeing = true;
					StartCoroutine (playAlarmSound ());

                    sh.SwitchFade(musicsource, musicsource.clip, alarmMusic, 1.5f);
                    //musicsource.clip = alarmMusic;
                    //musicsource.Play();
                    break;
                }
                else
                {
                    if (musicsource.clip == alarmMusic && musicsource.clip != finalMusic && musicsource.clip != sadFinalMusic)
                    {

                        sh.SwitchFade(musicsource, musicsource.clip, backgroundMusic, 1.5f);
                        // musicsource.clip = backgroundMusic;
                        // musicsource.Play();
                    }
                }
				if(fleeingAliens.Contains(alien) && alien.state != Alien.AlienState.FLEEING)
                {
					fleeingAliens.Remove(alien);
				}
			}
		}
    }

    void switchActiveSkill()
    {
        actionSwitched++;
        if (actionSwitched % 2 != 0)
        {
            gs.ActiveSkill += 1;
            if (gs.ActiveSkill > 2) gs.ActiveSkill = 0;
            ui.UpdateSkillToggle();
        }
    }

    IEnumerator playLoseSound()
    {
        if (musicsource.clip == backgroundMusic || musicsource.clip == alarmMusic)
        {
            sh.SwitchFade(musicsource, musicsource.clip, sadFinalMusic, 1.5f);
            //musicsource.clip = sadFinalMusic;
            //musicsource.Play();
        }
        yield return new WaitForSeconds(0);
    }
	IEnumerator playAlarmSound(){
        //source.PlayOneShot(attackSound, 1.0f);
        if(!attackSource.isPlaying) attackSource.Play();
        yield return new WaitForSeconds (10);
		fleeing = false;
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
        if (spaceshipCrashed)
            yield break;
        spaceshipCrashed = true;

        if (!source.isPlaying)  source.Play();

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

    IEnumerator Win()
    {
        if (musicsource.clip == backgroundMusic || musicsource.clip == alarmMusic)
        {
            sh.SwitchFade(musicsource, musicsource.clip, finalMusic, 1.5f);
            //musicsource.clip = finalMusic;
            //musicsource.Play();
        }
        float start = Time.time;
        GameObject ship = GameObject.Find("SpaceShip");
        Vector3 start_pos = ship.transform.position;

        while (Time.time - start < 1.5f)
        {
            float frac = (Time.time - start) / 1.5f;
            ship.transform.position = Vector3.Lerp(start_pos, start_pos + (start_pos.normalized * 5f), frac);
            yield return false;
        }

        start = Time.time;
        start_pos = ship.transform.position;

        while (Time.time - start < 2f)
        {
            float frac = (Time.time - start) / 2f;
            ship.transform.position = Vector3.Lerp(start_pos, start_pos + ship.transform.right * 400f, frac);
            yield return false;
        }

        ui.ShowWin();
		yield break;
	}


    void SpawnAliens(int count) 
	{
		for (int i = 0; i < count; i++) {
			var rotation = Random.Range(0.03f, 0.1f);
			Vector3 sec_pos = Vector3.RotateTowards(GameValues.ShipPos, new Vector3(0,0,5) * GameValues.ShipPos.magnitude, rotation, GameValues.ShipPos.magnitude);
			float angle = i * 360.0f/count;
			
			var detail_pos = Quaternion.AngleAxis(angle, GameValues.ShipPos) * sec_pos;

			Alien a = new Alien (health: 100, speed: 2f, range: 7);
			a.GameObject = Creator.Create ("Alien_withDying", detail_pos, "Alien");
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
        float value = Mathf.Pow(1.00316f, Time.time-lastSpawn)- 1;
        if(rand < value)
        {
            DecideMonsterFamily();
            lastSpawn = Time.time;
        }
    }

    void DecideMonsterFamily()
    {
        var r = Random.Range(0.0f, 1.0f);
        if (r > 0.75f)
            SpawnShyMonsters();
        else if (r > 0.50f)
            SpawnPredators();
        else if (r > 0.25f)
            SpawnEvilMonsters();
        else
            SpawnReallyEvilMonsters();

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
            Vector3 pos = gs.MonsterSpawnPoints.Any() + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            pos = CoordinateHelper.GroundPosition(pos);
            ShyMonster m = new ShyMonster(attack: 5, health: 25, speed: 2.5f, range: 7, contagious: false);
            DoSpawn(position: pos, monster: m, resName: "mahluq", ingameName: "ShyMonster");
        }
    }

    void SpawnPredators()
    {
        int count = Random.Range(2,5);
        Vector3 pos = gs.MonsterSpawnPoints.Any();
        for (int i = 0; i < count; i++)
        {
            float wait = i * 2.0f;
            if (i != 0)
                StartCoroutine(SpawnCoroutine(pos, wait));
            else
                DoPredatorSpawn(pos);
        }
    }

    IEnumerator SpawnCoroutine(Vector3 pos, float wait)
    {
        yield return new WaitForSeconds(wait);
        DoPredatorSpawn(pos);
    }

    void DoPredatorSpawn(Vector3 pos)
    {    
        Vector3 spawnPos = pos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        spawnPos = CoordinateHelper.GroundPosition(spawnPos);
        bool contagious = Random.Range(0f, 1f) < 0.15f;
        PredatoryMonster m = new PredatoryMonster(attack: 7, health: 50, speed: 3.5f, range: 10, contagious: contagious);
        DoSpawn(position: pos, monster: m, resName: "monster_small", ingameName: "PredatoryMonster");
		if(contagious) m.GameObject.transform.Find("Infection").GetComponent<ParticleSystem>().Play();
    }

    void SpawnEvilMonsters()
    {
        int count = Random.Range(1, 3);
        Debug.Log("Spawning " + count + " EvilMonsters");
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = gs.MonsterSpawnPoints.Any() + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            pos = CoordinateHelper.GroundPosition(pos);
            EvilMonster m = new EvilMonster(attack: 10, health: 125, speed: 3.25f, range: 8, contagious: false);
            DoSpawn(position: pos, monster: m, resName: "monster", ingameName: "EvilMonster");
        }
    }

    void SpawnReallyEvilMonsters()
    {
        Debug.Log("Spawning 1 ReallyEvilMonster");
        Vector3 pos = gs.MonsterSpawnPoints.Any();
        EvilMonster m = new EvilMonster(attack: 20, health: 200, speed: 2.75f, range: 6, contagious: false);
        DoSpawn(position: pos, monster: m, resName: "evil_final", ingameName: "EvilMonster");
    }

    void DoSpawn(Vector3 position, Monster monster, string resName, string ingameName)
    {
        monster.GameObject = Creator.Create(resName, position, ingameName);
        gs.monsters.Add(monster.GameObject, monster);
        gs.creatures.Add(monster.GameObject, monster as Creature);

        GameObject effect = Creator.Create("Spawner", position, "Spawner");
        effect.transform.forward = -(planet.transform.position - position).normalized;
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

    void ReplaceShipModel()
    {
        Destroy(GameObject.Find("SpaceShip"));
        GameObject new_ship = Creator.Create("Spaceship_whole", GameValues.ShipPos, "SpaceShip");
        new_ship.transform.up = GameValues.ShipPos.normalized;
        if(!jetStartSource.isPlaying) jetStartSource.Play();
    }

    void StartLeaveTimer()
	{
		StartCoroutine (LeaveTimer ());
	}

	IEnumerator LeaveTimer()
	{
		yield return new WaitForSeconds(60f);
		foreach (DictionaryEntry d in gs.aliens)
		{
			Alien a = d.Value as Alien;
			StartCoroutine(BeamAlienToShip(a));
		}
	}

	IEnumerator BeamAlienToShip(Alien a) 
	{
		a.GameObject.transform.Find ("Beam").GetComponent<ParticleSystem> ().Play ();
		yield return new WaitForSeconds(1f);
		a.GameObject.transform.position = GameValues.ShipPos;
	}
	
	
}
