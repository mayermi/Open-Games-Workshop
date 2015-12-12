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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);
            // we hit the planet -> set target
            if (hit.transform && hit.transform.gameObject == GameObject.Find("Planet"))
            {
                //GetComponent<PathNavigator>().SetTarget(hit.point);                            
            }

        }
        
    }

	public void moveTowards(Vector3 targetPos) 
	{
			anim.Play(animName);
			transform.position += runSpeed * Time.deltaTime * transform.forward;         
	}

    

}