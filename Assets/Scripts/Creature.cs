using UnityEngine;
using System.Collections;

public abstract class Creature{

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

	public void moveTo(Vector3 target)
	{
	}

	public void takeDamage(int d) 
	{
		CurrentHealth = CurrentHealth - d;
	}

	public void die() 
	{

	}

	public bool isInRange(GameObject g)
	{
		if(true /* TODO */) 
			return true;
		return false;
	}

}
