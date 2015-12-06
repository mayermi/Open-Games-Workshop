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
        float radius = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x * 1.49f;

        for (int i = 0; i < verts.Length; i++)
        {
            var r = Random.Range(0.0f, 1.0f);
            Vector3 pos = verts[i].normalized * radius;
            GameObject mainObject;
            if ( r > 0.7f)
            {
                mainObject = Instantiate((GameObject)Resources.Load("rock"));
                mainObject.transform.up = -(transform.position - pos).normalized;
                mainObject.transform.position = pos;
            }
            else if (r > 0.5f)
            {
                mainObject = Instantiate((GameObject)Resources.Load("flyingrock"));
                mainObject.transform.up = -(transform.position - pos).normalized;
                mainObject.transform.position = pos;

                var detailCount = Random.Range(0, 4);
                for (int j = 0; j < detailCount; j++)
                {
                    var x_rot = Random.Range(-0.05f, 0.05f);
                    var y_rot = Random.Range(-0.05f, 0.05f);
                    var z_rot = Random.Range(-0.05f, 0.05f);

                    var sec_pos = Vector3.RotateTowards(pos, new Vector3(1, 0, 0) * pos.magnitude, x_rot, pos.magnitude);
                    sec_pos = Vector3.RotateTowards(sec_pos, new Vector3(0, 1, 0) * pos.magnitude, y_rot, pos.magnitude);
                    sec_pos = Vector3.RotateTowards(sec_pos, new Vector3(0, 0, 1) * pos.magnitude, z_rot, pos.magnitude);

                    var detailDecision = Random.Range(0.0f, 1.0f);
                    GameObject detail;
                    if(detailDecision > 0.5f)
                        detail = Instantiate((GameObject)Resources.Load("mushroom_1"));
                    else
                        detail = Instantiate((GameObject)Resources.Load("flower_2"));
                    detail.transform.up = -(transform.position - sec_pos).normalized;
                    detail.transform.position = sec_pos;
                }
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
