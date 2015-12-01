using UnityEngine;
using System.Collections;

public class GestureSequence01 : MonoBehaviour
{
    GameObject sphere01;
    Vector3 sphere01InitialPosition;
    bool sequenceToggle01;
    float toggleCount01;

    GameObject sphere02;
    Vector3 sphere02InitialPosition;
    bool sequenceToggle02;
    float toggleCount02;
    // Use this for initialization
    void Start()
    {
        //The Sphere01 Asset is configured for Activate Action on FingersSpread Gesture
        //and for Deactivate Action on Grab Gesture via the GestureReceive01 Asset in the
        //Unity Editor.
        sphere01 = GameObject.Find("Sphere01");
        sphere01.SetActive(false);
        sphere01InitialPosition = sphere01.transform.position;
        sphere01.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f);
        sequenceToggle01 = false;
        toggleCount01 = 0;

        //The Sphere02 Asset is configured for Activate Action on ThumbsDown Gesture
        //and for Deactivate Action on TwoFingerPinch Gesture via the GestureReceive01 Asset in the
        //Unity Editor.
        sphere02 = GameObject.Find("Sphere02");
        sphere02.SetActive(true);
        sphere02InitialPosition = sphere02.transform.position;
        sphere02.GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f);
        sequenceToggle02 = false;
        toggleCount02 = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //If Sphere01 is switched from Inactive to Active via the FingersSpread Gesture
        //then the toggle variable is set to true
        if (!sequenceToggle01 && sphere01.activeSelf)
        {
            sequenceToggle01 = true;
        }
        //If Sphere01 if switched from Active to Inactive via the Grab Gesture
        //then the sphere is returned to its initial position. Sphere01 will
        //not become visible at the initial position until Activated by the
        //FingersSpread Gesture. The toggle count is also incremented and the color of
        //Sphere01 is moved from red towards green.
        if (sequenceToggle01 && !sphere01.activeSelf)
        {
            sequenceToggle01 = false;
            sphere01.transform.position = sphere01InitialPosition;
            toggleCount01 += 1;

            if (toggleCount01 <= 10)
            {
                sphere01.GetComponent<Renderer>().material.color = new Color(1.0f - (toggleCount01 / 10.0f), toggleCount01 / 10.0f, 0.0f);
            }
        }

        //If Sphere02 is switched from Inactive to Active via the TwoFingerPinch Gesture
        //then the toggle variable is set to true
        if (!sequenceToggle02 && sphere02.activeSelf)
        {
            sequenceToggle02 = true;
        }
        //If Sphere02 if switched from Active to Inactive via the TwoFingerPinch Gesture
        //then the sphere is returned to its initial position. Sphere02 will
        //not become visible at the initial position until Activated by the
        //ThumbsDown Gesture. The toggle count is also incremented and the color of
        //Sphere01 is moved from green towards blue. On the third toggle Sphere01's color
        //is reset to red.
        if (sequenceToggle02 && !sphere02.activeSelf)
        {
            sequenceToggle02 = false;
            sphere02.transform.position = sphere02InitialPosition;
            toggleCount02 += 1;

            if (toggleCount02 <= 10)
            {
                sphere02.GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f - (toggleCount01 / 10.0f), toggleCount01 / 10.0f);
            }

            if (toggleCount02 == 3)
            {
                sphere01.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f);
            }
        }
    }
}


