﻿using System.Collections.Generic;
using UnityEngine;
using Pantheon.Utils;

public class RandomObjectScattering : MonoBehaviour
{

    public int minDetails = 1;
    public int maxDetails = 4;
    public int monsterSpawnPointAmount = 5;

	Vector3[] verts;
	float radius;
	Vector3 ship_pos;
    int placedResources = 0;
    int maxResources;
    GameState gameState;
    GameObject objects;

    Material rockMaterial;

    List<string> planetMats = new List<string>();
    List<string> rockMats = new List<string>();

    void Start()
    {
		GameValues.PlanetRadius = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x * 0.5f * gameObject.transform.localScale.x;

        objects = new GameObject();
        objects.name = "EnvironmentObjects";

        planetMats.Add("PlanetGround_1");
        planetMats.Add("PlanetGround_2");
        planetMats.Add("PlanetGround_3");
        rockMats.Add("Rocks_1");
        rockMats.Add("Rocks_2");
        rockMats.Add("Rocks_3");
    }

    void Update()
    {
    }

	public void Setup()
	{
		radius = GameValues.PlanetRadius;
		var c = new IcoSphereFactory();
		var ico = c.Create(subdivisions: 3);
		verts = ico.GetComponent<MeshFilter>().sharedMesh.vertices;
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        maxResources = GameObject.Find("GameState").GetComponent<GameState>().maxResources;
        PaintPlanet();
		PlaceSpaceship ();
        PlaceMonsterSpawnPoints();
		PlaceObjects ();
	}

    void PaintPlanet()
    {
        int i = UnityEngine.Random.Range(0, planetMats.Count);
        string name = planetMats[i];
        Material mat = Resources.Load("Materials/" + name) as Material;
        gameObject.GetComponent<Renderer>().material = mat;

        rockMaterial = Resources.Load("Materials/" + rockMats[i]) as Material;
    }

	void PlaceSpaceship() 
	{
		int index = Random.Range (0, verts.Length);
		ship_pos = verts [index].normalized * radius;
        GameValues.ShipPos = ship_pos;

        /*
        GameObject ship = Creator.Create("Spaceship_broken", ship_pos, "SpaceShip");
		Vector3 up = -(transform.position - ship_pos).normalized;
		ship.transform.up = up;
        */

        //Let Camera look directly at spaceship
		if (Camera.main.GetComponent<CameraRotation> ())
		{
			Camera.main.transform.position = verts [index].normalized * Camera.main.GetComponent<CameraRotation> ().getCamDistance();
			Camera.main.transform.LookAt (transform.position);
		}

	}

    void PlaceMonsterSpawnPoints()
    {
        int i = 0;
        while (i < monsterSpawnPointAmount)
        {
            int index = Random.Range(0, verts.Length);
            var pos = verts[index].normalized * radius;
            if (gameState.MonsterSpawnPoints.Contains(pos) || pos == ship_pos)
                continue; //make sure each point is unique and we do not spawn on top of the ship
            if (CoordinateHelper.calcDistance(pos, ship_pos) < 600.0f)
                continue; //Do not spawn to close to the ship

            i++;
            gameState.MonsterSpawnPoints.Add(pos);
            GameObject spawn = Creator.Create("Spawn_Point", pos, string.Format("MonsterSpawnPoint_{0}", i));
            Vector3 up = -(transform.position - pos).normalized;
            spawn.transform.up = up;
            foreach (var renderer in spawn.GetComponentsInChildren<Renderer>())
                renderer.material = rockMaterial;
        }

    }

    void PlaceObjects()
    {           
        foreach (var vertex in verts)
        {
            Vector3 pos = vertex.normalized * radius;

			if(pos == ship_pos)
                continue;
            if(gameState.MonsterSpawnPoints.Count > 0 && gameState.MonsterSpawnPoints.Contains(pos))
                continue;

			float distToShip = (ship_pos-pos).magnitude;

            float scaleFactor = 1f;
            string mainObjectName = DecideMainObject();
            GameObject mainObject = null;
            if (!mainObjectName.Equals("nothing") && distToShip > 10f)
            {
                if (mainObjectName.Equals("rocks_0"))
                    scaleFactor = 0.3f;
			
                mainObject = Creator.Create(mainObjectName, pos, mainObjectName);
                mainObject.transform.up = -(transform.position - pos).normalized;
                mainObject.transform.Rotate(mainObject.transform.up, Random.Range(0f, 360f), Space.World);
                var scale = scaleFactor * ScaleFunction(Random.Range(1.0f, 2.0f));
				mainObject.transform.localScale *= scale;
                mainObject.transform.SetParent(objects.transform);
				mainObject.layer = 10;
                mainObject.GetComponentInChildren<Renderer>().material = rockMaterial;

                var detailCount = Random.Range(minDetails, maxDetails);
                Vector3 helpVector = RandomVector();

                //small objects around rocks
                for (int j = 0; j < detailCount; j++)
                {
                    var rotation = Random.Range(0.03f, 0.06f);
                    Vector3 sec_pos = Vector3.RotateTowards(pos, helpVector * pos.magnitude, rotation, pos.magnitude);
                    float angle = j * 360.0f / detailCount;

                    var detail_pos = Quaternion.AngleAxis(angle, pos) * sec_pos;

                    var detailName = DecideDetailObject();
                    GameObject detail = Creator.Create(detailName, detail_pos, detailName);
                    detail.transform.up = -(transform.position - detail_pos).normalized;
                    var small_scale = 0.5f * ScaleFunction(Random.Range(1.0f, 2.0f));
					detail.transform.localScale *= small_scale;
                    detail.transform.Rotate(detail.transform.up, Random.Range(0f, 360f), Space.World);
                    detail.transform.SetParent(objects.transform);
                }
            } else if(placedResources < maxResources){     // resources, where no rocks are so that aliens can reach them        
                GameObject detail = Creator.Create("crystal", pos, "resource");
                detail.transform.up = -(transform.position - pos).normalized;
				//var small_scale = 0.5f * ScaleFunction(Random.Range(1.0f, 2.0f));
				//detail.transform.localScale *= small_scale;
                detail.transform.Rotate(detail.transform.up, Random.Range(0f, 360f), Space.World);
                placedResources++;
            }

            // small objects anywhere
            for (int j = 0; j < Random.Range(0,2); j++)
            {
                var rotation = Random.Range(0.03f, 0.06f);
                Vector3 sec_pos = Vector3.RotateTowards(pos, RandomVector() * pos.magnitude, rotation, pos.magnitude);
                float angle = j * 360.0f / 4;

                var detail_pos = Quaternion.AngleAxis(angle, pos) * sec_pos;
                var detailName = DecideDetailObject();
                GameObject detail = Creator.Create(detailName, detail_pos, detailName);
                detail.transform.up = -(transform.position - detail_pos).normalized;
				var small_scale = 0.5f * ScaleFunction(Random.Range(1.0f, 2.0f));
				detail.transform.localScale *= small_scale;
                detail.transform.Rotate(detail.transform.up, Random.Range(0f, 360f), Space.World);
                detail.transform.SetParent(objects.transform);
            }

        }

        // not enough Resources placed
        if(placedResources < maxResources)
        {
            Debug.Log("Only " + placedResources + " placed.");
        }

    }

    string DecideMainObject()
    {
        var r = Random.Range(0.0f, 1.0f);
        string mainObjectName = "nothing";
        if (r > 0.975f)
            mainObjectName = "rock_group_0";
        else if (r > 0.95f)
            mainObjectName = "rock_group_1";
        else if (r > 0.925f)
			mainObjectName = "rock_group_3";
        else if (r > 0.9f)
            mainObjectName = "s_rock_0";
        else if (r > 0.875f)
            mainObjectName = "s_rock_1";
        else if (r > 0.85f)
            mainObjectName = "s_rock_2";


        return mainObjectName;
    }

    string DecideDetailObject()
    {
        var detailDecision = Random.Range(0.0f, 1.0f);
        string detailObjectName;
        if (detailDecision > 0.85f)
            detailObjectName = "flower_3";
        else if (detailDecision > 0.7f)
            detailObjectName = "flower_1";
        else if (detailDecision > 0.65f)
            detailObjectName = "tree";
        else if (detailDecision > 0.55f)
            detailObjectName = "mushroom";
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
