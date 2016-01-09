using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class TutorialController : MonoBehaviour {

    public GameObject navigation;
    public GameObject story;
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
            if (story.GetComponent<CanvasGroup>().alpha > 0.95f)
            {
                StartCoroutine(FadeTutorial(story, 1f, 0f));
                ShowSkills();
            } else if(skills.GetComponent<CanvasGroup>().alpha > 0.95f)
            {
                GameObject.Find("UI").GetComponent<UIManager>().ShowUI();
                HideTutorials();
            }
            else
            {
                HideTutorials();
            }
                  
        }
    }

    public void ShowNavigation()
    {
        ActivateBlur();
        StartCoroutine(FadeTutorial(navigation, 0f, 1f));
    }

    public void ShowStory()
    {
        ActivateBlur();
        StartCoroutine(FadeTutorial(story, 0f, 1f));
    }

    public void ShowSkills()
    {
        ActivateBlur();
        StartCoroutine(FadeTutorial(skills, 0f, 1f));
    }

    public void ShowMonsters(Vector3 pos)
    {
        Camera.main.GetComponent<CameraRotation>().FocusOnPoint(pos, 15);
        StartCoroutine(WaitForCamera());
    }

    public void HideTutorials()
    {
        Debug.Log(navigation.GetComponent<CanvasGroup>().alpha);
        if (navigation.GetComponent<CanvasGroup>().alpha > 0.95f) StartCoroutine(FadeTutorial(navigation, 1f, 0f));
        if (story.GetComponent<CanvasGroup>().alpha > 0.95f) StartCoroutine(FadeTutorial(story, 1f, 0f));
        if (skills.GetComponent<CanvasGroup>().alpha > 0.95f) StartCoroutine(FadeTutorial(skills, 1f, 0f));
        if (monsters.GetComponent<CanvasGroup>().alpha > 0.95f) StartCoroutine(FadeTutorial(monsters, 1f, 0f));
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
        StartCoroutine(FadeTutorial(monsters, 0f, 1f));
        Time.timeScale = 0;
    }

    IEnumerator FadeTutorial(GameObject tutorial, float from, float to)
    {   
        float start = Time.time;
        if(from == 1f) Time.timeScale = 1f;
        while (Time.time - start < 0.3f)
        {
            tutorial.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(from, to, (Time.time - start)/0.3f);
            yield return true;
        }
        if(from == 0f) Time.timeScale = 0;
    }
}
