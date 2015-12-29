using UnityEngine;
using System.Collections;

public class CreatePlanetLandscape : MonoBehaviour {

    Mesh mesh;
    // scale of the transformations
    public float scale = 5f;
    // magnitude of transformations
    public float magn = 1f;
    // exponent, controls how extreme the noise is
    public float exp = 1f;

    public int objCount = 1;

    float randomOffset;
    float startTime;
    bool noObjects = true;

	GameState gs;

	void Start () {
		gs = GameObject.Find ("GameState").GetComponent<GameState> ();
        startTime = Time.time;
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        randomOffset = Random.Range(-25.0f, 25.0f);
        //ShapeLandscape();
		          
		/*ShyMonster m = new ShyMonster(15, 100, 0.2f, 10);
		m.GameObject = Creator.Create("monster", new Vector3(0,0,-70), "ShyMonster");
		gs.monsters.Add(m.GameObject, m);
		gs.creatures.Add (m.GameObject, m as Creature);
		         
		Alien a = new Alien(100, 0.2f, 10);
		a.GameObject = Creator.Create("Alien", new Vector3(12,0,-70), "Alien");
		gs.aliens.Add(a.GameObject, a);
		gs.creatures.Add (a.GameObject, a as Creature);*/
	}

    void Update()
    {
        if(noObjects && Time.time - startTime > 1)
        {
           placeObjects();
        }
    }

    void ShapeLandscape()
    {

        Vector3[] verts = mesh.vertices;

        // iterate over all vertices and move them based on PerlinNoise
        for(int i=0; i<verts.Length; i++){
                Vector3 vert = verts[i];
                Vector3 dir = (gameObject.transform.position - vert).normalized;
                verts[i] += Noise(vert.x, vert.y, vert.z)*dir;
        }

        
        // update the mesh with the new vertices
        mesh.vertices = verts;      
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        transform.GetComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;

    }
	
    void placeObjects()
    {
        noObjects = false;
        Vector3[] verts = gameObject.GetComponent<MeshFilter>().mesh.vertices; // diese Zeile ist das Problem
        /*for (int i = 0; i < objCount; i++)
        {      
            Vector3 pos = verts[Random.Range(0, verts.Length)] * transform.localScale.x;          
            ShyMonster m = new ShyMonster(15, 100, 0.2f, 10);
            m.GameObject = Creator.Create("monster", pos, "ShyMonster");
            gs.monsters.Add(m.GameObject, m);
			gs.creatures.Add (m.GameObject, m as Creature);
        }*/

        for (int i = 0; i < objCount; i++)
        {
            Vector3 pos = verts[Random.Range(0, verts.Length)] * transform.localScale.x;
            PredatoryMonster m = new PredatoryMonster(7, 50, 0.3f, 7, false);
            m.GameObject = Creator.Create("monster_small", pos, "PredatoryMonster");
            gs.monsters.Add(m.GameObject, m);
            gs.creatures.Add(m.GameObject, m as Creature);
        }
    }

    // returns a noise value for x,y,z
    // uses three noise calculations with different frequencies
    float Noise(float x, float y, float z)
    {
        x += randomOffset;
        float noise1 = Mathf.PerlinNoise(x / scale, z / scale);
        z -= randomOffset;
        float noise2 = Mathf.PerlinNoise(z / scale * 0.5f, y / scale * 0.5f);
        y += randomOffset;
        float noise3 = Mathf.PerlinNoise(y / scale * 0.25f, z / scale * 0.25f);
        return Mathf.Pow((noise1 + noise2 + noise3)*magn, exp);
    }
}
