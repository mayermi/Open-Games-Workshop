using UnityEngine;
using System.Collections;

public class AnimationTest : MonoBehaviour {
	
	private Animation anim;

	void Start() {
		anim = GetComponent<Animation> ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			anim.Play("Run");
		}

		if (anim.IsPlaying ("Run")) {
			transform.position += 0.1f * Vector3.forward;
		}

	}
}
