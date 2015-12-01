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
        /*for (int i = 0; i < objCount; i++)
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
        /*var r = transform.GetComponent<Renderer>().bounds.size.x * 0.5f;
        for (int theta = -90; theta < 90; theta+=10)
        {
            for (int phi = 0; phi < 360; phi+=10)
            {
                var t = (float)theta;
                var p = (float)phi;
                Vector3 point = CoordinateHelper.PolarToCartesian(t, p, r) ;
                var random = Noise(point.x,point.y,point.z);
                if (random > 0.3f)
                {
                   
                    
                    Vector3 dir = (transform.position - point);

                    GameObject rock = Instantiate((GameObject)Resources.Load("flyingrock"));
                    rock.transform.up = -dir;
                    rock.transform.position = point;
                    //rock.transform.RotateAround(rock.transform.up, Random.Range(0f, 360f));
                }
            }
        }*/
        /*
        var bounds = transform.GetComponent<Renderer>().bounds.size.x;
        var radius = transform.GetComponent<Renderer>().bounds.size.x * 0.5f;
        int off = (int)(bounds * 0.5f);
        float goldenAngle = Mathf.PI * (3 - Mathf.Sqrt(3));
        for (float i = -1; i <= 1; i += 0.1f)
        {
            for (float j = -1; j <= 1; j += 0.1f)
            {
                for (float k = -1; k <= 1; k += 0.1f)
                {
                    
                    var v = new Vector3(i,j,k);
                    var length = v.magnitude;
                    var random = Random.Range(0.0f, 1.0f);
                    var random2 = Random.Range(0.0f, 1.0f);
                    if (length <= 1)
                    {
                        GameObject rock = Instantiate((GameObject)Resources.Load("flyingrock"));
                        v = v.normalized * (radius + 5.0f);
                        rock.transform.position = v;
                    }
                }
            }
        }*/

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

    // returns a noise value for x,y,z
    // uses three noise calculations with different frequencies
    float Noise(float x, float y, float z)
    {
        x += 1;
        var scale = 10;
        float noise1 = Mathf.PerlinNoise(x / scale, z / scale);
        z -= 1;
        float noise2 = Mathf.PerlinNoise(z / scale * 0.5f, y / scale * 0.5f);
        y += 1;
        float noise3 = Mathf.PerlinNoise(y / scale * 0.25f, z / scale * 0.25f);
        return Mathf.Pow((noise1 + noise2 + noise3) / 3.0f, 1);
    }
}
