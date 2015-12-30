using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper : MonoBehaviour {
	
	private float letterPause = 3f;
	string[] message = new string[3];
	Text textComp;
	string textComp1;
	string textComp2;
	
	// Use this for initialization
	void Start () {
		textComp = GetComponent<Text>();
		textComp1 = "Ein junger Gott befindet sich in den letzten Zügen seiner Ausbildung zu einem anerkannten Gott. Seine letzte Abschlussprüfung besteht darin, den kleinen Planeten Namek vor den drohenden Gefahren des Weltalls zu bewahren. Nur einer gönnt ihm diesen bevorstehenden Erfolg nicht: sein böser Bruder Tvorac. Von Eifersucht und Missgunst getrieben versucht er alles, um den Planeten ins Chaos zu stürzen... \n\nAls eine außerirdische Expedition auf dem kleinen Planeten abstürzt, nutzt er die Situation und versucht diese mit seinen überaus grausamen Fähigkeiten zu vernichten um somit die Reifeprüfung seines Bruders zu sabotieren. Dieser möchte den gestrandeten Gästen helfen ihr zu Bruch gegangenes Raumschiff zu reparieren, um wieder Ruhe auf dem kleinen Planeten einkehren zu lassen. ";
		textComp2 = "\n\n\n<color=yellow><i>Schlüpfe in die Rolle des jungen Gottes und wende die Attacken des Bruders ab, um der Expedition eine sichere Abreise zu ermöglichen!</i></color>";
		message[0] = textComp1;
		message[1] = textComp2;
		textComp.text = "";
		StartCoroutine(TypeText ());
	}
	
	IEnumerator TypeText () {
		foreach (string letter in message) {
			Debug.Log (letter);
			textComp.text += letter;
			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}
	}
}
