﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

    public Slider alienSlider;
    public Text countAliensText;
    public Text countResourcesText;
    public Slider resourceSlider;

    public Toggle fireToggle;
    public Toggle lightningToggle;
    public Toggle healToggle;

    GameState gs;

    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        alienSlider.maxValue = gs.maxAliens;
        resourceSlider.maxValue = gs.maxResources;

        UpdateSkillToggle();
    }

    void Update()
    {
        if(gs == null) gs = GameObject.Find("GameState").GetComponent<GameState>();
    }
	
    public void SetResourceSlider()
    {
        int resourcesCount = gs.CollectedResources;
       
        countResourcesText.text = "Resources found: " + resourcesCount.ToString() + "/" + gs.maxResources;
        resourceSlider.value = resourcesCount;
    }

    public void SetAlienSlider()
    {
        int aliensCount = gs.aliens.Count;
        countAliensText.text = "Aliens alive: " + aliensCount.ToString();
        alienSlider.value = aliensCount;
        Debug.Log(aliensCount);
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
}