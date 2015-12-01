using UnityEngine;
using System.Collections;

public class RandomObjectScattering : MonoBehaviour {

    public int objCount = 1;

    void Start()
    {
        placeObjects();
    }

    void Update()
    {
    }

    void placeObjects()
    {
        /* for (int i = 0; i < objCount; i++)
         {
             var theta = Random.Range(0f, 360f);
             var phi = Random.Range(0f, 360f);
             var r = transform.localScale.x;
             Vector3 pos = RandomPointOnSphere();
             Vector3 dir = (transform.position - pos).normalized;

             GameObject pyramid = Instantiate((GameObject)Resources.Load("pyramid"));
             pyramid.transform.forward = -dir;
             pyramid.transform.position = pos;
             pyramid.transform.localScale *= ScaleFunction(4f);
             pyramid.transform.RotateAround(pyramid.transform.forward, Random.Range(0f, 360f));
         }

         for (int i = 0; i < objCount / 10; i++)
         {
             var theta = Random.Range(0f, 360f);
             var phi = Random.Range(0f, 360f);
             var r = transform.localScale.x;
             Vector3 pos = RandomPointOnSphere();
             Vector3 dir = (transform.position - pos).normalized;
             pos -= Random.RandomRange(2f, 4f) * dir;

             GameObject rock = Instantiate((GameObject)Resources.Load("flyingrock"));
             rock.transform.up = -dir;
             rock.transform.position = pos;
             rock.transform.RotateAround(rock.transform.up, Random.Range(0f, 360f));
         }*/

        GameObject ico = GameObject.Find("icosphere");
        Vector3[] verts = ico.GetComponent<MeshFilter>().mesh.vertices;
        float radius = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x*1.5f;

        for (int i = 0; i < verts.Length; i++)
        {
            if (Random.Range(0f, 1f) > 0.96f)
            {
                Vector3 pos = verts[i].normalized * radius;
                GameObject rock = Instantiate((GameObject)Resources.Load("flyingrock"));
                rock.transform.up = -(transform.position - pos).normalized;
                rock.transform.position = pos;
            }
        }
    }

    float ScaleFunction(float factor)
    {
        //e^(ax-a²)+0.5 -> lots of small and few big objects
        return Mathf.Exp(factor * Random.Range(0f, factor) - factor * factor) + 0.5f;
    }

    Vector3 RandomPointOnSphere()
    {
        var theta = Random.Range(0f, 360f);
        var phi = Random.Range(0f, 360f);
        var r = transform.GetComponent<Renderer>().bounds.size.x * 0.5f;

        var x = r * Mathf.Sin(theta) * Mathf.Cos(phi);
        var y = r * Mathf.Sin(theta) * Mathf.Sin(phi);
        var z = r * Mathf.Cos(theta);

        return new Vector3(x, y, z);
    }
}
