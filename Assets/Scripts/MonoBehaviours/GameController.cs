using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    GameState gs;
    GameObject planet;
    Camera cam;
    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;

    // Use this for initialization
    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        planet = GameObject.Find("Planet");
        cam = Camera.main;
        Vector3 angles = cam.transform.eulerAngles;
        rotationXAxis = angles.x;
        rotationYAxis = angles.y;
	}
	
	void LateUpdate () {

        if (Input.GetKey(KeyCode.A))
        {
            // planet.transform.RotateAround(new Vector3(0, 1, 0), Time.deltaTime * 0.25f);
            rotationYAxis += 0.1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            // planet.transform.RotateAround(new Vector3(0, 1, 0), -Time.deltaTime * 0.25f);
            rotationYAxis -= 0.1f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            rotationXAxis += 0.1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            rotationXAxis -= 0.1f;
        }

        Quaternion cameraRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
        cam.transform.rotation = cameraRotation;

        // let the camera circle around the planet in a distance of 185w
        Vector3 cameraPosition = cameraRotation * new Vector3(0, 0, -185) + planet.transform.position;
        cam.transform.position = cameraPosition;
    }


    
}
