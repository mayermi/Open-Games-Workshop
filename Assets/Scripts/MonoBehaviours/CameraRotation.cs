using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour {

    public int camDistance = 185;
    GameObject planet;
    GameObject light;
    Camera cam;
    float fov;
    public float fovSpeed = 8f;
    public float camSpeed = 5f;
    public float SLERPTIME = 1f;
	private int timesFingerPinch = 0;

    void Start () {
        planet = GameObject.Find("Planet");
        cam = Camera.main;
        fov = cam.fieldOfView;
        light = GameObject.Find("Headlight");
    }

    void LateUpdate()
    {

        if (Input.GetKey(KeyCode.A))
        {
            Vector3 verticalaxis = transform.TransformDirection(Vector3.down);
            transform.RotateAround(planet.transform.position, verticalaxis, -camSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 verticalaxis = transform.TransformDirection(Vector3.up);
            transform.RotateAround(planet.transform.position, verticalaxis, -camSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 horizontalaxis = transform.TransformDirection(Vector3.left);
            transform.RotateAround(planet.transform.position, horizontalaxis, -camSpeed * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 horizontalaxis = transform.TransformDirection(Vector3.right);
            transform.RotateAround(planet.transform.position, horizontalaxis, -camSpeed * Time.deltaTime);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 ) // forward
        {
            if (fov >= 10) fov -= 5;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 ) // back
        {
            if (fov <= 100) fov += 5;
        }

        if (Input.GetKey(KeyCode.Y)) // forward
        {
            if (fov >= 10) fov -= 0.75f;
        }
        if (Input.GetKey(KeyCode.X)) // back
        {
            if (fov <= 100) fov += 0.75f;
        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, Time.deltaTime * fovSpeed);


        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Focus(GameValues.ShipPos, 35f));
        }

        UpdateLight();
    }

    public void FocusOnPoint(Vector3 pos, float newFov)
    {
        StartCoroutine(Focus(pos, newFov));
    }

    IEnumerator Focus(Vector3 pos, float newFov)
    {
        float startTime = Time.time;
        fov = newFov;

        Vector3 cameraPosition = pos.normalized * camDistance;

        Vector3 fromPos = cam.transform.position;
        Vector3 fromRot = -transform.position.normalized;

        while (Time.time < startTime + SLERPTIME)
        {
            cam.transform.forward = Vector3.Slerp(fromRot, -pos.normalized, (Time.time - startTime) / SLERPTIME);
            cam.transform.position = Vector3.Slerp(fromPos, cameraPosition, (Time.time - startTime) / SLERPTIME);
            UpdateLight();
            yield return null;
        }       

    }

	//Needed for RealSense
	public void FocusOnSpaceship(){
		timesFingerPinch++;
		if(timesFingerPinch % 2 != 0)
		StartCoroutine(Focus(GameValues.ShipPos, 35f));
	}

    public int getCamDistance()
    {
        return camDistance;
    }

    void UpdateLight()
    {
        light.transform.position = transform.position;
        light.transform.rotation = transform.rotation;
    }
}
