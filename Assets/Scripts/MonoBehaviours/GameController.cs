using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    GameState gs;
    SkillController sc;
    GameObject planet;   

    // Use this for initialization
    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        sc = GameObject.Find("SkillController").GetComponent<SkillController>();
        planet = GameObject.Find("Planet");
        gs.ActiveSkill = 1;
        GameObject.Find("PathFinding").GetComponent<SphericalGrid>().BakeNodeProcess(); 
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            sc.PerformActiveSkill();
        }
    }

    void LateUpdate () {

    }

        
}
