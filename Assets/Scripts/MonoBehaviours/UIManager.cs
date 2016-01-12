using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class UIManager : MonoBehaviour {

    public Slider alienSlider;
    public Text countAliensText;
	public Slider resourceSlider;
    public Text countResourcesText;
    
    public Toggle fireToggle;
    public Toggle lightningToggle;
    public Toggle healToggle;

	public Slider fireSlider;
	public Slider lightningSlider;
	public Slider healSlider;

    public GameObject failure;
    public GameObject win;

    GameState gs;

    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        alienSlider.maxValue = gs.maxAliens;
        resourceSlider.maxValue = gs.resourcesNeeded;

        UpdateSkillToggle();
        GetComponent<CanvasGroup>().alpha = 0;
    }

    void Update()
    {
        if(gs == null) gs = GameObject.Find("GameState").GetComponent<GameState>();
    }
	
    public void ShowUI()
    {
        StartCoroutine(FadeUI(gameObject, 0f, 1f, 1f));
    }

    public void SetResourceSlider()
    {
        int resourcesCount = gs.CollectedResources;
       
        countResourcesText.text = "Resources found: " + resourcesCount.ToString() + "/" + gs.resourcesNeeded;
        resourceSlider.value = resourcesCount;
    }

    public void SetAlienSlider()
    {
        int aliensCount = gs.aliens.Count;
        countAliensText.text = "Aliens alive: " + aliensCount.ToString();
        alienSlider.value = aliensCount;
    }

    public void UpdateSkillToggle()
    {
        int active = gs.ActiveSkill;

        switch (active)
        {
            case 0:
                this.fireToggle.image.rectTransform.sizeDelta = new Vector2(15, 15);
                this.healToggle.image.rectTransform.sizeDelta = new Vector2(15, 15);
                this.lightningToggle.image.rectTransform.sizeDelta = new Vector2(20, 20);
                break;
            case 1:
                this.healToggle.image.rectTransform.sizeDelta = new Vector2(15, 15);
                this.lightningToggle.image.rectTransform.sizeDelta = new Vector2(15, 15);
                this.fireToggle.image.rectTransform.sizeDelta = new Vector2(20, 20);
                break;
            case 2:
                this.lightningToggle.image.rectTransform.sizeDelta = new Vector2(15, 15);
                this.fireToggle.image.rectTransform.sizeDelta = new Vector2(15, 15);
                this.healToggle.image.rectTransform.sizeDelta = new Vector2(20, 20);
                break;
            default:
                break;
        }
    }

    public void showWin()
    {
        ActivateBlur();
        StartCoroutine(FadeUI(GameObject.Find("GameUI"), 1f, 0f, 1f));
        StartCoroutine(FadeUI(win, 0f, 1f, 1f));
    }

    public void showLose()
    {
        ActivateBlur();
        StartCoroutine(FadeUI(GameObject.Find("GameUI"), 1f, 0f, 1f));
        StartCoroutine(FadeUI(failure, 0f, 1f, 3f));
    }

	public void UpdateTimeout(int skill, float timeFrac)
	{
		Slider s;
		if (skill == 0)
			s = lightningSlider;
		else if (skill == 1)
			s = fireSlider;
		else
			s = healSlider;

		if (s.gameObject.activeSelf == false) 
		{
			s.gameObject.SetActive (true);
			s.transform.parent.GetComponent<Image>().CrossFadeAlpha(0.25f, 0.25f, false);
		}
			
		s.value = timeFrac;

		if (s.value < 0.02f) 
		{
			s.gameObject.SetActive (false);
			s.transform.parent.GetComponent<Image>().CrossFadeAlpha(1f, 0.25f, false);
		}
			
	}

    void ActivateBlur()
    {
        Camera.main.GetComponent<BlurOptimized>().enabled = true;
    }

    IEnumerator FadeUI(GameObject tutorial, float from, float to, float duration)
    {
        float start = Time.time;
        while (Time.time - start < duration)
        {
            tutorial.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(from, to, (Time.time - start) / duration);
            yield return false;
        }
        tutorial.GetComponent<CanvasGroup>().alpha = to;
    }
}
