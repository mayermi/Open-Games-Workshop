using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour {

    GameObject planet;
    Camera cam;
    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;

    public int initCameraDistance = 20;

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
            rotationYAxis += 0.1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotationYAxis -= 0.1f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            rotationXAxis += 0.1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rotationXAxis -= 0.1f;
        }

        Quaternion cameraRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
        cam.transform.rotation = cameraRotation;

        // 07-12-2015 changed to public variable : let the camera circle around the planet in a distance of 185
        Vector3 cameraPosition = cameraRotation * new Vector3(0, 0, initCameraDistance) + planet.transform.position;
        cam.transform.position = cameraPosition;
    }
}
