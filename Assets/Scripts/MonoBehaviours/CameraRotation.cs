using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour {

    private int camDistance = 185;
    GameObject planet;
    Camera cam;
    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;

    void Start () {
        planet = GameObject.Find("Planet");
        cam = Camera.main;
        Vector3 angles = cam.transform.eulerAngles;
        rotationXAxis = angles.x;
        rotationYAxis = angles.y;
    }

    void LateUpdate()
    {

        if (Input.GetKey(KeyCode.A))
        {
            rotationYAxis += 5f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotationYAxis -= 5f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            rotationXAxis += 5f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rotationXAxis -= 5f * Time.deltaTime;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            Camera.main.fieldOfView = Camera.main.fieldOfView - 5;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            Camera.main.fieldOfView = Camera.main.fieldOfView + 5;
        }
      

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
