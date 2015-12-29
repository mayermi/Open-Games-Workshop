using UnityEngine;
using System.Collections;

public class MoveOnSphere : MonoBehaviour {

	private Creature c;
	public string animName;

    Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
		c = GameObject.Find ("GameState").GetComponent<GameState> ().creatures [gameObject] as Creature;
    }

	void Update() 
	{
		if(c == null)
			c = GameObject.Find ("GameState").GetComponent<GameState> ().creatures [gameObject] as Creature;
	}

	public void moveTowards(Vector3 targetPos) 
	{
			anim.Play(animName);
			transform.position += c.Speed * Time.deltaTime * transform.forward;         
	}

    

}