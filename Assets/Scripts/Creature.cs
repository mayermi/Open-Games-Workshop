using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Creature {

	public int MaxHealth { get; set;}
	public int CurrentHealth { get; set;}
	public float Speed { get; set; }
	public int VisionRange { get; set;}
	public GameObject GameObject { get; set;}

    private bool dead;

	public Creature(int health, float speed, int range) {
		MaxHealth = health;
		CurrentHealth = MaxHealth;
		Speed = speed;
		VisionRange = range;
	}

	public void MoveTo(Vector3 target)
	{
        GameObject.GetComponent<PathNavigator>().locked = false;
        GameObject.GetComponent<PathNavigator> ().SetTarget (target);
    }

    public void StopMoving()
    {
        GameObject.GetComponent<PathNavigator>().StopMoving();
    }

    public virtual void TakeDamage(int d, object source=null) 
	{
		CurrentHealth = CurrentHealth - d;
        //GameObject.GetComponentInChildren<Slider>().value = (float)CurrentHealth / (float)MaxHealth;
        GameObject.GetComponent<CreatureHelper>().AdjustHealthBar();
        if (CurrentHealth <= 0) Die();
	}

	public virtual void GetHealed(int d, object source=null) 
	{
		CurrentHealth = CurrentHealth + d;
		if (CurrentHealth > MaxHealth)
			CurrentHealth = MaxHealth;
        //GameObject.GetComponentInChildren<Slider>().value = (float)CurrentHealth / (float)MaxHealth;
        GameObject.GetComponent<CreatureHelper>().AdjustHealthBar();
    }

	public virtual void Die() 
	{
        if (!dead)
        {
            dead = true;
            GameObject.tag = "Dead";
            GameObject.Find("GameController").SendMessage("RemoveReferences", this);
            GameObject.Find("GameState").SendMessage("RemoveCreature", this);
            StopMoving();
            GameObject.GetComponent<CreatureHelper>().StartDying();
        }
	}

	public bool IsInRange(GameObject g)
	{
        float d = CoordinateHelper.calcDistance(GameObject.transform.position, g.transform.position);
        if (d < VisionRange)  
			return true;
		return false;
	}


}
