using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    GameObject planet;

    void Start()
    {
        planet = GameObject.Find("Planet");
    }

    void Update()
    {
        planet.transform.RotateAround(planet.transform.forward, Time.deltaTime/-2f);
    }

    public void LoadScene()
    {
        Application.LoadLevel("sphere_movement");
    }
}
