using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class TutorialController : MonoBehaviour {

    public GameObject navigation;
    public GameObject skills;
    public GameObject monsters;
    GameObject cam;


	void Start () {
        cam = Camera.main.gameObject;
        cam.GetComponent<BlurOptimized>().enabled = false;
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
        ActivateBlur();
        navigation.SetActive(true);
        Time.timeScale = 0;
    }

    public void ShowSkills()
    {
        ActivateBlur();
        skills.SetActive(true);
        Time.timeScale = 0;
    }

    public void ShowMonsters(Vector3 pos)
    {
        StartCoroutine(Camera.main.GetComponent<CameraRotation>().FocusOnPoint(pos, 15));
        StartCoroutine(WaitForCamera());
    }

    public void HideTutorials()
    {
        navigation.SetActive(false);
        skills.SetActive(false);
        monsters.SetActive(false);
        cam.GetComponent<BlurOptimized>().enabled = false;
        Time.timeScale = 1f;
    }

    void ActivateBlur()
    {
        cam.GetComponent<BlurOptimized>().enabled = true;
    }

    IEnumerator WaitForCamera()
    {
        yield return new WaitForSeconds(1.1f);
        ActivateBlur();
        monsters.SetActive(true);
        Time.timeScale = 0;
    }
}
