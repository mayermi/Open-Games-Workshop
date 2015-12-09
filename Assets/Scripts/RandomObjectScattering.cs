using UnityEngine;

public class RandomObjectScattering : MonoBehaviour
{

    public int minDetails = 1;
    public int maxDetails = 4;

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
        var ico = c.Create(subdivisions: 3);
        Vector3[] verts = ico.GetComponent<MeshFilter>().sharedMesh.vertices;
        float radius = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x * 0.5f * gameObject.transform.localScale.x;

        foreach (var vertex in verts)
        {
            Vector3 pos = vertex.normalized * radius;
            float scaleFactor = 1.0f;
            string mainObjectName = DecideMainObject();
            GameObject mainObject = null;
            if (!mainObjectName.Equals("nothing"))
            {
                if (mainObjectName.Equals("rocks_0"))
                    scaleFactor = 0.3f;

                mainObject = Instantiate((GameObject)Resources.Load(mainObjectName));
                mainObject.transform.up = -(transform.position - pos).normalized;
                mainObject.transform.position = pos;
                mainObject.transform.Rotate(mainObject.transform.up, Random.Range(0f, 360f), Space.World);
                var scale = scaleFactor * ScaleFunction(Random.Range(1.0f, 2.0f));
                mainObject.transform.localScale = new Vector3(scale, scale, scale);
            }

            var detailCount = Random.Range(minDetails, maxDetails);
            Vector3 helpVector = RandomVector();
            
            for (int j = 0; j < detailCount; j++)
            {
                var rotation = Random.Range(0.03f, 0.06f);
                Vector3 sec_pos = Vector3.RotateTowards(pos, helpVector * pos.magnitude, rotation, pos.magnitude);
                float angle = j * 360.0f/detailCount;
                
                var detail_pos = Quaternion.AngleAxis(angle, pos) * sec_pos;

                var detailName = DecideDetailObject();
                GameObject detail = Instantiate((GameObject)Resources.Load(detailName));
                detail.transform.up = -(transform.position - detail_pos).normalized;
                detail.transform.position = detail_pos;
                var scale = 0.5f * ScaleFunction(Random.Range(1.0f, 2.0f));
                detail.transform.localScale = new Vector3(scale, scale, scale);
                detail.transform.Rotate(detail.transform.up, Random.Range(0f, 360f), Space.World);
            }
        }
    }

    string DecideMainObject()
    {
        var r = Random.Range(0.0f, 1.0f);
        string mainObjectName = "nothing";
        if (r > 0.9f)
            mainObjectName = "rock_group_0";
        else if (r > 0.8f)
            mainObjectName = "rock_group_1";
        else if (r > 0.7f)
            mainObjectName = "rock_group_3";

        return mainObjectName;
    }

    string DecideDetailObject()
    {
        var detailDecision = Random.Range(0.0f, 1.0f);
        string detailObjectName;
        if (detailDecision > 0.5f)
            detailObjectName = "mushroom_1";
        else
            detailObjectName = "flower_2";
        return detailObjectName;
    }

    Vector3 RandomVector()
    {
        var x = Random.Range(0f, 1f);
        var y = Random.Range(0f, 1f);
        var z = Random.Range(0f, 1f);
        return new Vector3 (x, y, z).normalized;
    }

    float ScaleFunction(float factor)
    {
        //e^(ax-a²)+1 -> lots of small and few big objects
        return Mathf.Exp(factor * Random.Range(0f, factor) - factor * factor) + 1.0f;
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
