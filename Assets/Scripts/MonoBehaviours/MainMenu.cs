using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pantheon.Utils;

public class MainMenu : MonoBehaviour {

    GameObject planet;
	bool setup = false;

	private AudioSource source;
	private AudioSource musicsource;
	
	public AudioClip backgroundMusic;
	private SoundHelper sh;

    void Start()
    {
        planet = GameObject.Find("Planet");
		
		sh = GameObject.Find("SoundHelper").GetComponent<SoundHelper>();
		
		musicsource = gameObject.GetComponent<AudioSource>();
		sh.Fade(musicsource, true, 0.5f);
    }

    public void LoadScene()
    {
		StartCoroutine(stopMusic ());
    }
	
	IEnumerator stopMusic()
	{
		sh.Fade(musicsource, false, 1f);
		yield return new WaitForSeconds(0.55f);
		Application.LoadLevel("main_game");
	}
	
	public void Quit()
	{
		Application.Quit ();
	}
}
