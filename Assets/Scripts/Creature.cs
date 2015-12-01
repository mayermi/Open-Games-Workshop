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
	}

	public void TakeDamage(int d) 
	{
		CurrentHealth = CurrentHealth - d;
        Debug.Log(CurrentHealth);
        GameObject.GetComponentInChildren<Slider>().value = (float)CurrentHealth / (float)MaxHealth;
        if (CurrentHealth < 0) Die();
	}

	public void Die() 
	{
        MonoBehaviour.Destroy(GameObject);
	}

	public bool IsInRange(GameObject g)
	{
        float d = CoordinateHelper.calcDistance(GameObject.transform.position, g.transform.position);
        if (d < VisionRange)  
			return true;
		return false;
	}

}
