using UnityEngine;
using System.Collections;

public class MoveOnSphere : MonoBehaviour {

    Vector3 target =  Vector3.zero;
    float moveTime;
    float startTime;

	void Update () {
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
                moveTime = calcMovementTime();
                startTime = Time.time;
            }
            
        }

        move();
    }

    void move()
    {
        
        if(target != Vector3.zero)
        {            
            float delta = (Time.time - startTime) / moveTime;
            Vector3 from = transform.position;
            transform.position = Vector3.Slerp(from, target, delta);

            // at target
            if (delta == 1.0) target = Vector3.zero;
 
        }
        
    }

    float calcMovementTime()
    {
        /*var R = 6371000; // metres
        var φ1 = lat1.toRadians();
        var φ2 = lat2.toRadians();
        var Δφ = (lat2 - lat1).toRadians();
        var Δλ = (lon2 - lon1).toRadians();

        var a = Math.sin(Δφ / 2) * Math.sin(Δφ / 2) +
                Math.cos(φ1) * Math.cos(φ2) *
                Math.sin(Δλ / 2) * Math.sin(Δλ / 2);
        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

        var d = R * c;*/

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
        return R * c;
    }

}