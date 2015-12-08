using UnityEngine;
using System.Collections;

public class MoveOnSphere : MonoBehaviour {

    public float runSpeed;
    public bool RunningLocked { get; set; }
    public Vector3 target;
	public string animName;
    Vector3 lookAt;

    Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
        target = transform.position;
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
                //SetTarget(hit.point);                            
            }

        }
        
    }

	public void moveTowards(Vector3 targetPos) 
	{
			anim.Play(animName);
			transform.position += runSpeed * transform.forward;         
	}

    

}