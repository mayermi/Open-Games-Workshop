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
}
