using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour {

    public int camDistance = 185;
    GameObject planet;
    Camera cam;
    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;
    float fov;
    public float fovSpeed = 8f;
    public float camSpeed = 5f;

    void Start () {
        planet = GameObject.Find("Planet");
        cam = Camera.main;
        fov = cam.fieldOfView;
        Vector3 angles = cam.transform.eulerAngles;
        rotationXAxis = angles.x;
        rotationYAxis = angles.y;
    }

    void LateUpdate()
    {

        Vector3 camAngles = cam.transform.eulerAngles;

        if (Input.GetKey(KeyCode.A))
        {
            rotationYAxis += camSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotationYAxis -= camSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W) /*&& (camAngles.x < 30 || camAngles.x > 320)*/)
        {
            rotationXAxis += camSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) /*&& (camAngles.x < 40 || camAngles.x > 330)*/)
        {
            rotationXAxis -= camSpeed * Time.deltaTime;
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

        Quaternion cameraRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
        cam.transform.rotation = cameraRotation;

        // let the camera circle around the planet in a distance of 185
        Vector3 cameraPosition = cameraRotation * new Vector3(0, 0, -camDistance) + planet.transform.position;
        cam.transform.position = cameraPosition;
    }


    public int getCamDistance()
    {
        return camDistance;
    }
}
