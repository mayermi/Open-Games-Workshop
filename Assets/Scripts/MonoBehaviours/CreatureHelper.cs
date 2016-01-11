using UnityEngine;
using System.Collections;

public class CreatureHelper : MonoBehaviour {

    private Canvas c;
    Creature creature;
    protected GameState gs;

    public virtual void Start () {
        c = gameObject.GetComponentInChildren<Canvas>();
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        creature = gs.creatures[gameObject] as Creature;
    }
	
	public virtual void Update () {
        // make sure the health bar faces the camera
        //c.transform.LookAt(Camera.main.transform.position);
    }

    public virtual void AdjustHealthBar()
    {
        float percentage = 100 * (float)creature.CurrentHealth / (float)creature.MaxHealth;


        //GameObject.GetComponentInChildren<Slider>().value = (float)CurrentHealth / (float)MaxHealth;
        Transform healthbar = transform.Find("Canvas").Find("Health");
        foreach (Transform child in healthbar)
        {
            if (System.Int32.Parse(child.name) > percentage)
            {
                child.gameObject.SetActive(false);
            } else
                child.gameObject.SetActive(true);
        }
    }

    public virtual void NoPathFound() { }

}
