using UnityEngine;
using System.Collections;

public class CorpseCleaner : MonoBehaviour {

    float start;
    Vector3 startPos;
    Vector3 endPos;

	// Use this for initialization
	void Start () {
        start = Time.time;
        startPos = transform.position;
        endPos = startPos - (startPos.normalized * 5f);
    }
	
	// Update is called once per frame
	void Update () {
	
        if( (Time.time - start) > 45f)
        {
            StartCoroutine(Sink());
        }

	}

    IEnumerator Sink()
    {
        float begin = Time.time;
        while(Time.time - begin < 5f)
        {
            float frac = (Time.time - begin) / 5f;
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, frac);
            yield return true;
        }

        Destroy(this);
    }

}
