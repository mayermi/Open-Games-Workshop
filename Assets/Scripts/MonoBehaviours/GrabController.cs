using UnityEngine;
using System.Collections;

public class GrabController : MonoBehaviour {

    GameState gs;
    bool grabbing = false;
    public GameObject objectToBeGrabbed = null;
	public AudioClip grabSound;
	private AudioSource source;
	private float volLowRange = .5f;
	private float volHighRange = 1.0f;
	private float vol;
    private Vector3 moveDir;
    private Vector3 prevPos;
    private GameObject handOfGod;
	Quaternion pos;

    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        handOfGod = GameObject.Find("HandOfGod");
        source = GetComponent<AudioSource>();
		vol = UnityEngine.Random.Range (volLowRange, volHighRange);
        prevPos = handOfGod.transform.position;
    }
	
	void Update () {
		handOfGod.transform.rotation = Quaternion.Euler(0, 0, 0);
        GameObject planet = GameObject.Find("Planet");
        float distance = Vector3.Distance(planet.transform.position, transform.position);
        float planetradius = GameValues.PlanetRadius;
       
        Vector3 v3 = Input.mousePosition;
        if (distance > (planetradius + 13))
        {
            v3.z = 130.0f;
        }
        else
        {
            v3.z = 130.0f + (distance - (planetradius + 13));
        }

        v3 = Camera.main.ScreenToWorldPoint(v3);

        gameObject.transform.position = v3;

        if (Input.GetMouseButtonDown(1))
        {
            if (!grabbing) Grab();
            else Release();
        }

        transform.up = (transform.position - planet.transform.position).normalized;

        moveDir = -(prevPos - handOfGod.transform.position);
        prevPos = handOfGod.transform.position;
    }

    void Grab()
    {
        // the selected Object gets bound to the Hand, physics do not affect it anymore     
        if ( !grabbing && objectToBeGrabbed != null)
            {
                Debug.Log("grab");
                GameObject.Find("hand").GetComponent<Animation>().Play("Grab");
                grabbing = true;
                Monster m = gs.monsters[objectToBeGrabbed] as Monster;
                m.GetGrabbed();
                objectToBeGrabbed.transform.SetParent(transform);


                objectToBeGrabbed.GetComponent<Rigidbody>().isKinematic = true;
                Vector3 pos = handOfGod.transform.position;
                objectToBeGrabbed.transform.position = pos;
                objectToBeGrabbed.transform.localPosition = new Vector3(-4f, 5f, 1f);

                source.PlayOneShot(grabSound,vol);
            }
    }


    void Release()
    {
        if (grabbing)
        {
            Debug.Log("Release");
            GameObject.Find("hand").GetComponent<Animation>().Play("Release");
            Monster m = gs.monsters[objectToBeGrabbed] as Monster;
            objectToBeGrabbed.transform.SetParent(null);
            objectToBeGrabbed.GetComponent<Rigidbody>().isKinematic = false;
            objectToBeGrabbed.GetComponent<Rigidbody>().AddForce(moveDir * 25f, ForceMode.Impulse);
            m.TakeDamage( (int) Mathf.Ceil(moveDir.magnitude * 25f) );
            m.Idle();
            grabbing = false;
            objectToBeGrabbed = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // if a Monster is in range, set it as the GameObject that will be grabbed
        if(gs.monsters[other.gameObject] != null &&
           !grabbing &&
           other.isTrigger == false)
        {
            objectToBeGrabbed = other.gameObject;
			objectToBeGrabbed.transform.Find("Highlight").gameObject.SetActive(true);
        }
            
    }

    void OnTriggerExit(Collider other)
    {
        // if the selected Monster leaves the range, deselect it 
        if (objectToBeGrabbed == other.gameObject)
        {           
            objectToBeGrabbed = null;           
        }
		if(other.transform.Find("Highlight"))
			other.transform.Find("Highlight").gameObject.SetActive(false);  
    }

    public bool IsGrabbing()
    {
        return grabbing;
    }
}
