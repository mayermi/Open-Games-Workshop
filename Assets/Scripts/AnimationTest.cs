using UnityEngine;
using System.Collections;

public class AnimationTest : MonoBehaviour {
	
	Animation animation;
	Camera camera;

	void Start() {
		animation = GetComponent<Animation> ();
		camera = Camera.main;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			animation.Play("Run");
		}

		if (animation.IsPlaying ("Run")) {
			transform.position += 0.1f * Vector3.forward;
		}

	}
}
