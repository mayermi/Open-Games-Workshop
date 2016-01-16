using UnityEngine;
using System.Collections;

public class CreatureHelper : MonoBehaviour {

    private Canvas c;
    Creature creature;
    protected GameState gs;
    private AudioSource source;
    private AudioClip deadsound;

    public string deadName;

    public virtual void Start() {
        c = gameObject.GetComponentInChildren<Canvas>();
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        creature = gs.creatures[gameObject] as Creature;
        source = gameObject.AddComponent<AudioSource>();

        if (creature is PredatoryMonster)
            deadsound = (AudioClip)Resources.Load("predatorydead");         
        else if (creature is Alien)
            deadsound = (AudioClip)Resources.Load("aliendead3");    
        else if (creature is EvilMonster)
            deadsound = (AudioClip)Resources.Load("evildead2");
        else
            deadsound = (AudioClip)Resources.Load("shydead");

        source.clip = deadsound;
        source.playOnAwake = false;
    }
	
	public virtual void Update () {
        // make sure the health bar faces the camera
        //c.transform.LookAt(Camera.main.transform.position);
    }

    public virtual void AdjustHealthBar()
    {
        float percentage = 100 * (float)creature.CurrentHealth / (float)creature.MaxHealth;

        Transform healthbar = transform.Find("Canvas").Find("Health");
        foreach (Transform child in healthbar)
        {
            if (System.Int32.Parse(child.name) > percentage+20)
            {
                child.gameObject.SetActive(false);
            } else
                child.gameObject.SetActive(true);
        }
    }

    public virtual void NoPathFound() { }


	void OnParticleCollision(GameObject other)
	{
        if (gameObject.tag != "Dead" && other.name == "Fire_Damaging") 
		{  
			creature.TakeDamage(1, other);
		}
	}

    public void StartDying()
    {
        StartCoroutine(playDeathSound());
        transform.Find("Canvas").Find("Health").gameObject.SetActive(false);

        Animation anim = gameObject.GetComponent<Animation>();

        anim.CrossFade("Die", 0.1f, PlayMode.StopAll);

        StartCoroutine(ReplaceModel());
    }

    IEnumerator ReplaceModel()
    {
        yield return new WaitForSeconds(2);
        Quaternion rot = gameObject.transform.rotation;
        Vector3 pos = gameObject.transform.position;

        GameObject deadCreature = Creator.Create(deadName, pos, "Corpse");
        deadCreature.transform.rotation = rot;
        gameObject.SetActive(false);
    }

    IEnumerator playDeathSound()
    {
        yield return new WaitForSeconds(1);
        source.Play();
    }
}
