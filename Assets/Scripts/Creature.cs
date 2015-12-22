using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Creature {

	public int MaxHealth { get; set;}
	public int CurrentHealth { get; set;}
	public float Speed { get; set; }
	public int VisionRange { get; set;}
	public GameObject GameObject { get; set;}

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
        GameObject.GetComponentInChildren<Slider>().value = (float)CurrentHealth / (float)MaxHealth;
        if (CurrentHealth <= 0) Die();
	}

	public virtual void Die() 
	{
		GameObject.Find ("GameController").SendMessage ("RemoveReferences", this);
		GameObject.Find ("GameState").SendMessage ("RemoveCreature", this);
        GameObject.SetActive(false);
        //MonoBehaviour.Destroy(GameObject);
	}

	public bool IsInRange(GameObject g)
	{
        float d = CoordinateHelper.calcDistance(GameObject.transform.position, g.transform.position);
        if (d < VisionRange)  
			return true;
		return false;
	}

}
