﻿using UnityEngine;
using System.Collections;

public class GrabController : MonoBehaviour {

    GameState gs;
    public float speed = 1f;
    bool grabbing = false;
    GameObject objectToBeGrabbed = null;
    GameObject lastObjectToBeGrabbed = null;
    int timesGrabbed = 0;

	// Use this for initialization
	void Start () {
        gs = GameObject.Find("GameState").GetComponent<GameState>();
    }
	
	// Update is called once per frame
	void Update () {
        // Y axis
        if (Input.GetKey(KeyCode.UpArrow))
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

        transform.up = (transform.position - GameObject.Find("Planet").transform.position).normalized;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (!grabbing) Grab();
            else Release(); 
        }
    }

    void Grab()
    {
        timesGrabbed++;
        
        //Debug.Log(objectToBeGrabbed);
        // the selected Object gets bound to the Hand, physics do not affect it anymore     
            if ( !grabbing && objectToBeGrabbed != null && objectToBeGrabbed != lastObjectToBeGrabbed)
            {
                Debug.Log("grab");
                grabbing = true;
                Monster m = gs.monsters[objectToBeGrabbed] as Monster;
                m.GetGrabbed();
                objectToBeGrabbed.transform.SetParent(transform);
            
                objectToBeGrabbed.GetComponent<Rigidbody>().isKinematic = true;
                Vector3 pos = GameObject.Find("HandOfGod").transform.position;
                objectToBeGrabbed.transform.position = pos;

                lastObjectToBeGrabbed = objectToBeGrabbed;
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
        // if a Monster is in range, set it as the GameObject that will be grabbed
        if(gs.monsters[other.gameObject] != null && !grabbing)
            objectToBeGrabbed = other.gameObject;
    }

    void OnTriggerLeave(Collider other)
    {
        // if the selected Monster leaves the range, deselect it 
        if (objectToBeGrabbed == other.gameObject || gs.monsters[other.gameObject] == null)
            objectToBeGrabbed = null;
    }

    public bool getGrabbed()
    {
        return grabbing;
    }
}
