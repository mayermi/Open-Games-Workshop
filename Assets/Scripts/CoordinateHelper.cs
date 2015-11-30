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
}
