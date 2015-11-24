using UnityEngine;
using System.Collections;

public class SphereicalCoordinateHelper : MonoBehaviour{

	public static Vector2 CartesianToPolar(Vector3 point)
	{
		Vector2 polar;
		
		// calculate longitude
		polar.y = Mathf.Atan2(point.x, point.z);
		
		float xzLen = new Vector2(point.x, point.z).magnitude;
		
		polar.x = Mathf.Atan2(-point.y, xzLen);
		
		//convert to deg
		polar *= Mathf.Rad2Deg;
		
		return polar;
	}
	
	public static Vector2 CartesianToUV(Vector3 pos)
	{
		float nx = pos.x + 1;
		float ny = pos.z + 1;
		float nz = pos.y + 1;
		float u=0.0f, v=0.0f;
		
		u = Mathf.Sin(nx / (Mathf.Sqrt( (nx*nx)+(nz*nz) )))/ Mathf.PI / 2 + 0.25f;
		if (nz < 0)
			u = 1-u;
		
		v = Mathf.Sin(ny)/ Mathf.PI + 0.5f;
		
		// flip u & v
		u = 1.0f-u;
		v = 1.0f-v;
		
		return new Vector2(u, v);
	}
	
	public static Vector3 UVToCartesian(Vector2 uv, float radius)
	{
		Vector3 returnPos = Vector3.zero;
		float theta = (uv.x - 1f) * 2f * Mathf.PI; // lat
		float phi = (uv.y - 1.5f) * Mathf.PI;
		
		returnPos.y = Mathf.Sin(theta) * Mathf.Cos(phi);
		returnPos.z = Mathf.Sin(phi);
		returnPos.x = Mathf.Cos(theta) * Mathf.Cos(phi);
		
		return (returnPos * radius);
	}
	
	public static Vector3 PolarToCartesian(Vector2 polar, float radius)
	{
		// an origin vector3, representing lat, lon of 0,0.
		Vector3 origin = new Vector3(0,0,radius);
		
		//build a quaternion using euler angles for lat, lon
		Quaternion rotation = Quaternion.Euler(polar.x , polar.y, 0);
		
		//transform our reference vector3 by the rotation.
		Vector3 point = rotation*origin;
		
		return point;
	}
}
