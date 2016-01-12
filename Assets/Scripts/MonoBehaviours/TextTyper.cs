using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper : MonoBehaviour {
	
	private float letterPause = 2f;
	Text textComp;
	string textComp1;
	string textComp2;

    Text loadText;

    // Use this for initialization
    void Start () {
		textComp = GameObject.Find("StoryText").GetComponent<Text>();
		textComp1 = "Ein junger Gott befindet sich in den letzten Zügen seiner Ausbildung zu einem anerkannten Gott. Seine letzte Abschlussprüfung besteht darin, den kleinen Planeten Namek vor den drohenden Gefahren des Weltalls zu bewahren. Nur einer gönnt ihm diesen bevorstehenden Erfolg nicht: sein böser Bruder Tvorac. Von Eifersucht und Missgunst getrieben, versucht er alles, um den Planeten ins Chaos zu stürzen... \n\nAls eine außerirdische Expedition auf dem kleinen Planeten abstürzt, nutzt er die Situation und versucht diese mit seinen überaus grausamen Fähigkeiten zu vernichten um somit die Reifeprüfung seines Bruders zu sabotieren. Dieser möchte den gestrandeten Gästen helfen ihr zu Bruch gegangenes Raumschiff zu reparieren, um wieder Ruhe auf dem kleinen Planeten einkehren zu lassen. ";
		textComp2 = "\n\n\n<color=#ff6699><i>Schlüpfe in die Rolle des jungen Gottes und wende die Attacken des Bruders ab, um der Expedition eine sichere Abreise zu ermöglichen!</i></color>";
		textComp.text = textComp1;

        loadText = GameObject.Find("LoadedText").GetComponent<Text>();
        loadText.text = "Spiel wird geladen...";
    }
	
    public void LoadingDone()
    {
        textComp.text += textComp2;
        loadText.text = "Drücke Enter, um zu starten.";
        StartCoroutine(BlinkText(loadText));
    }

    IEnumerator BlinkText(Text text)
    {
        while (true)
        {
            yield return new WaitForSeconds(1.2f);
            float alpha = 1f;
            if (text.canvasRenderer.GetAlpha() == 1f) alpha = 0.1f;
            text.CrossFadeAlpha(alpha, 1.2f, false);
        }
    }

    
}
