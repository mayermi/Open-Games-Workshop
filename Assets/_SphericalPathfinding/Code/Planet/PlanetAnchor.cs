using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlanetBody))]
public class PlanetAnchor : MonoBehaviour
{
	PlanetBody planetBody;

	void Awake()
	{
		planetBody = GetComponent<PlanetBody>();
	}

	void Update()
	{
		transform.position = planetBody.GroundPosition(transform.position);
	}

}
