using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper : MonoBehaviour {
	
	private float letterPause = 0.02f;
	string message;
	Text textComp;
	
	// Use this for initialization
	void Start () {
		textComp = GetComponent<Text>();
		message = textComp.text;
		textComp.text = "";
		StartCoroutine(TypeText ());
	}
	
	IEnumerator TypeText () {
		foreach (char letter in message.ToCharArray()) {
			textComp.text += letter;
			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}
	}
}
