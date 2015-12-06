using UnityEngine;
using System.Collections;

public class MoveOnSphere : MonoBehaviour {

    public float runSpeed;
    public bool RunningLocked { get; set; }
    public Vector3 target;
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
                SetTarget(hit.point);                            
            }

        }
        
    }

    void FixedUpdate()
    {
        if(!RunningLocked) move();
    }

    public void SetTarget(Vector3 t)
    {
        target = t;
        Vector3 up = target - GameObject.Find("Planet").transform.position;
        Vector3 lookAt = Vector3.Cross(up, target) + up;
        transform.LookAt(lookAt, up);
    }

    void move()
    {
        // moves toward target until arrived
        if(CoordinateHelper.calcDistance(transform.position, target) > 0.1f)
        {
            anim.Play("Run");
            transform.position += runSpeed * transform.forward;         
        } else {
            target = transform.position;
            anim.Stop("Run");
        }
        
    }

    

}