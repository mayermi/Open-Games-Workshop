using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlienController : MonoBehaviour {

	public float speed;
	public Slider resourceSlider;
	public Slider alienSlider;
	public Toggle fireToggle;
	public Toggle lightningToggle;
	public Toggle waterToggle;
	
	private Rigidbody rb;
	private int countResources;
	private int countAliens;
	private string skillState;
	private bool isInSkillState;
	public Text countResourcesText;
	public Text countAliensText;
	
	void Start() {
		rb = GetComponent<Rigidbody>();
		countResources = 0;
		countAliens = 100;
		isInSkillState = false;
		SetResourceSlider();
		fireToggle.onValueChanged.AddListener((x) => ToggleSkillButton(fireToggle));
		lightningToggle.onValueChanged.AddListener((x) => ToggleSkillButton(lightningToggle));
		waterToggle.onValueChanged.AddListener((x) => ToggleSkillButton(waterToggle));
	}

	void Die() {
		gameObject.SetActive(false);
		if (gameObject.CompareTag ("Alien")) {
			countAliens = countAliens - 1;
			SetAlienSlider ();
		}
	}
	
	void FixedUpdate() {
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		
		rb.AddForce (movement * speed);
	}

	void ToggleSkillButton(Toggle toggle) {
		if (toggle.isOn) {
			this.fireToggle.image.rectTransform.sizeDelta = new Vector2 (15, 15);
			this.lightningToggle.image.rectTransform.sizeDelta = new Vector2 (15, 15);
			this.waterToggle.image.rectTransform.sizeDelta = new Vector2 (15, 15);

			if(toggle != this.fireToggle) {
				this.fireToggle.isOn = false;
			}
			if(toggle != this.lightningToggle) {
				this.lightningToggle.isOn = false;
			}
			if(toggle != this.waterToggle) {
				this.waterToggle.isOn = false;
			}

			toggle.image.rectTransform.sizeDelta = new Vector2 (20, 20);
		} else {
			toggle.image.rectTransform.sizeDelta = new Vector2 (15, 15);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Resource")) {
			other.gameObject.SetActive(false);
			countResources = countResources + 1;
			SetResourceSlider();
		} else if (other.gameObject.CompareTag("Monster")) {
			Die();
		}
	}

	void SetAlienSlider() {
		countAliensText.text = countAliens.ToString();
		alienSlider.value = countAliens;
	}

	void SetResourceSlider() {
		countResourcesText.text = countResources.ToString ();
		resourceSlider.value = countResources;
	}
}