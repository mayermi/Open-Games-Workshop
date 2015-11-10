using UnityEngine;
using System.Collections;

public class MoveOnSphere : MonoBehaviour {

    public float runSpeed;

    Vector3 target =  Vector3.zero;
    Vector3 from = Vector3.zero;
    Vector3 lookAt;
    float moveTime;
    float startTime;

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
                target = hit.point;
                from = transform.position;
                moveTime = calcMovementTime();
                startTime = Time.time;

                // look at target
                Vector3 up = target - GameObject.Find("Planet").transform.position;
                Vector3 lookAt = Vector3.Cross(up, target) + up;
                transform.LookAt(lookAt, up);

            }

        }

        move();
    }

    void move()
    {
        
        if(target != Vector3.zero)
        {
            anim.Play("Run");
            float progress = (Time.time - startTime) / moveTime;
            transform.position = Vector3.Slerp(from, target, progress);

            // at target
            if (progress >= 1.0)
            {
                target = Vector3.zero;
                anim.Stop("Run");      
            }
 
        }
        
    }

    float calcMovementTime()
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

        // return the time 
        float distance = R * c;
        return distance / runSpeed;
    }

}