using UnityEngine;

public class RandomObjectScattering : MonoBehaviour
{

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
        var c = new IcoSphereFactory();
        var ico = c.Create(subdivisions: 2);
        Vector3[] verts = ico.GetComponent<MeshFilter>().sharedMesh.vertices;
        float radius = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x * 1.5f;
        Debug.Log(verts.Length);
        for (int i = 0; i < verts.Length; i++)
        {
            if (Random.Range(0.0f, 1.0f) < 0.7)
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
