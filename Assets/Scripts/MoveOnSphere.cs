using UnityEngine;
using System.Collections;

public class MoveOnSphere : MonoBehaviour {

    public float runSpeed;

    Vector3 target;
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
                target = hit.point;

                // look at target
                Vector3 up = target - GameObject.Find("Planet").transform.position;
                Vector3 lookAt = Vector3.Cross(up, target) + up;
                transform.LookAt(lookAt, up);             
            }

        }
        
    }

    void FixedUpdate()
    {
        move();
    }

    void move()
    {
        // moves toward target until arrived
        if(calcDistance() > 0.1f)
        {
            anim.Play("Run");
            transform.position += runSpeed * transform.forward;         
        } else {
            target = transform.position;
            anim.Stop("Run");
        }
        
    }

    float calcDistance()
    {
        float R = GameObject.Find("Planet").transform.localScale.x;
        Vector2 polarTarget = CoordinateHelper.CartesianToPolar(target);
        Vector2 polarFrom = CoordinateHelper.CartesianToPolar(transform.position);

        // convert Lat to radians
        float lat1 = CoordinateHelper.DegreeToRadians(polarTarget.x);
        float lat2 = CoordinateHelper.DegreeToRadians(polarFrom.x);

        // delta angle in radians
        float deltaLat = CoordinateHelper.DegreeToRadians(polarTarget.x - polarFrom.x);
        float deltaLon = CoordinateHelper.DegreeToRadians(polarTarget.y - polarFrom.y);

        float a = Mathf.Sin(deltaLat / 2.0f) * Mathf.Sin(deltaLat / 2.0f) +
                  Mathf.Cos(lat1) * Mathf.Cos(lat2) *
                  Mathf.Sin(deltaLon / 2.0f) * Mathf.Sin(deltaLon / 2.0f);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        // return the distance 
        float distance = R * c;
        return distance;
    }

}