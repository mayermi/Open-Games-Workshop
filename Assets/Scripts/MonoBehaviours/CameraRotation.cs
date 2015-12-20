using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour {

    public int camDistance = 185;
    GameObject planet;
    Camera cam;
    float rotationXAxis_local = 0.0f;
    float rotationYAxis_local = 0.0f;
    float fov;
    public float fovSpeed = 8f;
    public float camSpeed = 5f;
    public Vector3 planetpos;

    void Start () {
        planet = GameObject.Find("Planet");
        planetpos = planet.transform.position;
        transform.LookAt(planetpos);
        cam = Camera.main;
        fov = cam.fieldOfView;
        Vector3 localAngles = cam.transform.localEulerAngles;
        rotationXAxis_local = localAngles.x;
        rotationYAxis_local = localAngles.y;

    }

    void LateUpdate()
    {

        if (Input.GetKey(KeyCode.A))
        {
            //rotationYAxis_local += camSpeed * Time.deltaTime;
            Vector3 verticalaxis = transform.TransformDirection(Vector3.down);
            transform.RotateAround(planet.transform.position, verticalaxis, -camSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            //rotationYAxis_local -= camSpeed * Time.deltaTime;
            Vector3 verticalaxis = transform.TransformDirection(Vector3.up);
            transform.RotateAround(planet.transform.position, verticalaxis, -camSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            //rotationXAxis_local += camSpeed * Time.deltaTime;
            Vector3 horizontalaxis = transform.TransformDirection(Vector3.left);
            transform.RotateAround(planet.transform.position, horizontalaxis, -camSpeed * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.S))
        {
            //rotationXAxis_local -= camSpeed * Time.deltaTime;
            Vector3 horizontalaxis = transform.TransformDirection(Vector3.right);
            transform.RotateAround(planet.transform.position, horizontalaxis, -camSpeed * Time.deltaTime);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            if(fov >= 10) fov -= 5;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            if (fov <= 120) fov += 5;
        }


        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, Time.deltaTime * fovSpeed);       

        /*Quaternion cameraRotation = Quaternion.Euler(rotationXAxis_local, rotationYAxis_local, 0);
        cam.transform.localRotation = cameraRotation;

        // let the camera circle around the planet in a distance of 185
        Vector3 cameraPosition = cameraRotation * new Vector3(0, 0, -camDistance) + planet.transform.position;
        cam.transform.position = cameraPosition;*/

    }


    public int getCamDistance()
    {
        return camDistance;
    }
}
