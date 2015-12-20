using UnityEngine;
using System.Collections;

public class MoveOnSphere : MonoBehaviour {

    public float runSpeed;
	public string animName;

    Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
    }

	void Update ()
    {
       
    }

	public void moveTowards(Vector3 targetPos) 
	{
			anim.Play(animName);
			transform.position += runSpeed * Time.deltaTime * transform.forward;         
	}

    

}