﻿using UnityEngine;
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

	void Start () {
        startTime = Time.time;
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        randomOffset = Random.Range(-25.0f, 25.0f);
        ShapeLandscape();
	}

    void Update()
    {
        if(noObjects && Time.time - startTime > 1)
        {
          // placeObjects();
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
        gameObject.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);

        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    // funktioniert noch nicht ganz richtig... Objekte sind nicht immer auf der Oberfläche
    void placeObjects()
    {
        noObjects = false;
        Vector3[] verts = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;
        for (int i = 0; i < objCount; i++)
        {
            GameObject tree = Instantiate((GameObject)Resources.Load("_Prefabs/Tree"));
            tree.transform.localScale *= 1.5f;
            Vector3 pos = verts[Random.Range(0, verts.Length)] * transform.localScale.x;          
            Vector3 dir = (transform.position - pos).normalized;
            tree.transform.forward = -dir;
            tree.transform.position = pos;
            
        }
    }

    // returns a noise value for x,y,z
    // uses two noise calculations with different frequencies
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