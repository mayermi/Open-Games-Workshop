using UnityEngine;
using System.Collections;

public class SkillController : MonoBehaviour {

    GameState gs;
    RecursiveLightning lightning;

    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        lightning = GameObject.Find("Lightning").GetComponent<RecursiveLightning>();
    }

    public void PerformActiveSkill()
    {
        int active = gs.ActiveSkill;
        switch(active)
        {
            case 1:
                Lightning();
                break;
            case 2:
                Nummer2();
                break;
            case 3:
                Nummer3();
                break;
            default:
                break;
        }
           
    }

    // Skill 1
    void Lightning()
    {
        // Start and end positions are placeholders at the moment
        Vector3 from = GameObject.Find("Alien(Clone)").transform.position + new Vector3(0, 1, 0);
        Vector3 to = GameObject.Find("monster(Clone)").transform.position;
        lightning.firstVertexPosition = from;
        lightning.lastVertexPosition = to;
        lightning.StrikeLightning();
        CauseDamage(to, 5f, 25);
    }

    // Skill 2
    void Nummer2()
    {
        Debug.Log("Skill 2 triggered");
    }

    // Skill 3
    void Nummer3()
    {
        Debug.Log("Skill 3 triggered");
    }

    void CauseDamage(Vector3 pos, float radius, int damage)
    {
        foreach(DictionaryEntry d in gs.monsters)
        {
            Monster m = d.Value as Monster;
            float dist = (pos - m.GameObject.transform.position).magnitude;
            if(dist < radius)
            {
                m.TakeDamage(damage);
            }
        }
    }
}
