using UnityEngine;
using System.Collections;

public static class CoordinateHelper {

    public static Vector2 CartesianToPolar(Vector3 point)
    {
        Vector2 polar;
 
        //calc longitude
        polar.y = Mathf.Atan2(point.x,point.z);
 
        //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
        float xzLen = new Vector2(point.x, point.z).magnitude;
        //atan2 does the magic
        polar.x = Mathf.Atan2(-point.y,xzLen);
 
        //convert to deg
        polar *= Mathf.Rad2Deg;
 
        return polar;
    }


    public static Vector3 PolarToCartesian(Vector2 polar)
    {
 
        //an origin vector, representing lat,lon of 0,0. 
        Vector3 origin = new Vector3(0, 0, 1);

        //build a quaternion using euler angles for lat,lon
        Quaternion rotation = Quaternion.Euler(polar.x, polar.y, 0);
        //transform our reference vector by the rotation. Easy-peasy!
        Vector3 point = rotation* origin;
 
        return point;

    }

    public static Vector3 PolarToCartesian(float theta, float phi, float radius)
    {
        var x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
        var y = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
        var z = radius * Mathf.Cos(theta);

        return new Vector3(x, y, z);
    }

    public static float DegreeToRadians(float a)
    {
        return a * Mathf.PI / 180;
    }

    public static float calcDistance(Vector3 from, Vector3 to)
    {
        float R = 10f * GameObject.Find("Planet").transform.localScale.x;
        Vector2 polarTarget = CoordinateHelper.CartesianToPolar(to);
        Vector2 polarFrom = CoordinateHelper.CartesianToPolar(from);

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
        float distance =  R * c;
        return distance;
    }

	public static Vector3 GroundPosition(Vector3 currentPosition)
	{
		float planetRadius = GameValues.PlanetRadius;
		Vector3 dir = (new Vector3(0,0,0) - currentPosition).normalized;
		Vector3 startRayPos = -dir * (planetRadius * 1.1f);
		
		Ray ray = new Ray();
		ray.origin = startRayPos;
		ray.direction = dir;
		
		int groundTypeLayer = 1<<10;
		RaycastHit hit;
		
		//Debug.DrawRay(startRayPos,  dir * radius);
		//Debug.Break();
		if(Physics.Raycast(startRayPos, dir, out hit, (planetRadius * 1.1f), groundTypeLayer))
		{
			return hit.point;
		}
		
		return currentPosition;
	}
}
