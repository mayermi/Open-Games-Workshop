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
    }

    public void LoadScene()
    {
        Application.LoadLevel("main_game");
    }

	public void Quit()
	{
		Application.Quit ();
	}
}
