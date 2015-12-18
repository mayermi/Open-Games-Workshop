using UnityEngine;
using System.Collections;

public class GrabController : MonoBehaviour {

    GameState gs;
    public float speed = 1f;
    bool grabbing = false;
    GameObject objectToBeGrabbed = null;

    // Use this for initialization
    void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
    }
	
	// Update is called once per frame
	void Update () {

        GameObject planet = GameObject.Find("Planet");
        float distance = Vector3.Distance(planet.transform.position, transform.position);
        float planetradius = planet.GetComponent<MeshFilter>().mesh.bounds.size.x * 0.5f * planet.transform.localScale.x;
       
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

        this.gameObject.transform.position = v3;

        if (Input.GetMouseButtonDown(1))
        {
            if (!grabbing) Grab();
            else Release();
        }

        // Y axis
        /*if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0,0.1f * speed, 0);
        } else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += new Vector3(0, -0.1f * speed, 0);
        }

        // X Axis
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(0.1f * speed, 0, 0);
        } else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-0.1f * speed, 0, 0);
        }
        
        // Z Axis
        if (Input.GetKey(KeyCode.L))
        {
            transform.position += new Vector3(0, 0, -0.1f * speed);
        }
        else if (Input.GetKey(KeyCode.K))
        {
            transform.position += new Vector3(0, 0, 0.1f * speed);
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            if (!grabbing) Grab();
            else Release();
        }*/

        transform.up = (transform.position - GameObject.Find("Planet").transform.position).normalized;

    }

    void Grab()
    {

        //Debug.Log(objectToBeGrabbed);
        // the selected Object gets bound to the Hand, physics do not affect it anymore     
            if ( !grabbing && objectToBeGrabbed != null)
            {
                Debug.Log("grab");
                grabbing = true;
                Monster m = gs.monsters[objectToBeGrabbed] as Monster;
                m.GetGrabbed();
                objectToBeGrabbed.transform.SetParent(transform);
            
                objectToBeGrabbed.GetComponent<Rigidbody>().isKinematic = true;
                Vector3 pos = GameObject.Find("HandOfGod").transform.position;
                objectToBeGrabbed.transform.position = pos;
            }
    }

    void Release()
    {
        if (grabbing)
        {
            Debug.Log("Release");
            Monster m = gs.monsters[objectToBeGrabbed] as Monster;
            objectToBeGrabbed.transform.SetParent(null);
            objectToBeGrabbed.transform.position = transform.position;
            objectToBeGrabbed.GetComponent<Rigidbody>().isKinematic = false;
            m.Idle();
            grabbing = false;
            objectToBeGrabbed = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        // if a Monster is in range, set it as the GameObject that will be grabbed
        if (gs.monsters[other.gameObject] != null && !grabbing)
            objectToBeGrabbed = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Leave");
        // if the selected Monster leaves the range, deselect it 
        if (objectToBeGrabbed == other.gameObject || gs.monsters[other.gameObject] == null)
            objectToBeGrabbed = null;
    }

    public bool getGrabbed()
    {
        return grabbing;
    }
}
