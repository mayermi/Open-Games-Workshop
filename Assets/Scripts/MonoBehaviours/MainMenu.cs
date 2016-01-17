using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    GameObject planet;
	bool setup = false;

    void Start()
    {
        planet = GameObject.Find("Planet");
    }

    void Update()
    {
        //planet.transform.RotateAround(planet.transform.forward, Time.deltaTime/8f);
		if (!setup) 
		{
			planet.GetComponent<RandomObjectScattering> ().Setup();
			setup = true;
		}
    }

    public void LoadScene()
    {
        Application.LoadLevel("main_game");
    }
}
