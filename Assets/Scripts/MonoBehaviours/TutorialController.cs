using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour {

    public GameObject navigation;
    public GameObject skills;
    public GameObject monsters;

	void Start () {
	    
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            HideTutorials();
        }
    }

    public void ShowNavigation()
    {
        navigation.SetActive(true);
        Time.timeScale = 0;
    }

    public void ShowSkills()
    {
        skills.SetActive(true);
        Time.timeScale = 0;
    }

    public void ShowMonsters()
    {
        monsters.SetActive(true);
        Time.timeScale = 0;
    }

    public void HideTutorials()
    {
        navigation.SetActive(false);
        skills.SetActive(false);
        monsters.SetActive(false);
        Time.timeScale = 1f;
    }
}
