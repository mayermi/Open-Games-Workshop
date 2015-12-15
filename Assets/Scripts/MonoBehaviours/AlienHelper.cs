using UnityEngine;
using System.Collections;

public class AlienHelper : CreatureHelper {

	Alien alien;
    GameState gs;
    bool movingToResource = false;
    bool movingToShip = false;

	public override void Start () {
		base.Start ();
        gs = GameObject.Find("GameState").GetComponent<GameState>();
        alien = gs.aliens[gameObject] as Alien;
        gameObject.GetComponent<SphereCollider>().radius = alien.VisionRange;

    }

	public override void Update () {
		base.Update ();
        if (alien.state == Alien.AlienState.SEARCHING && !movingToResource) alien.Search();
        else if (alien.state == Alien.AlienState.CARRYING) alien.CarryResource(gs.ShipPos);

        if (movingToResource) CheckDistToResource();
        if (movingToShip) CheckDistToShip();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "resource" && alien.state == Alien.AlienState.SEARCHING)
        {
            // check if resource is free to pick up
            if(DoesNotBelongToOtherAlien(other.gameObject))
            {
                Debug.Log("Detected Resource");
                alien.MoveTo(other.gameObject.transform.position);
                movingToResource = true;
                alien.Resource = other.gameObject;
            }
            
        }
    }

    void OnTriggerLeave(Collider other)
    {
        if (other.gameObject.name == "resource")
        {
            alien.Resource = null;
        }
    }

    void CheckDistToResource()
    {
        float dist = (gameObject.transform.position - alien.Resource.transform.position).magnitude;
        if (dist <= 5f)
        {
            alien.TakeResource();
            movingToResource = false;
            movingToShip = true;
        }
    }
    void CheckDistToShip()
    {
        float dist = (gameObject.transform.position - gs.ShipPos).magnitude;
        if (dist <= 5f)
        {        
            gs.CollectedResources += 1;
            GameObject res = alien.Resource;
            // check if other Aliens were trying to reach this specific resource too
            RemoveResourceReferences(res);
            Destroy(res);
            movingToShip = false;
            Debug.Log("Deposit Resource. Collected REsources: " + gs.CollectedResources);
        }
    }

    bool DoesNotBelongToOtherAlien(GameObject res)
    {
        foreach (DictionaryEntry d in gs.aliens)
        {
            Alien a = d.Value as Alien;
            if (a.Resource == res)
            {
                return false;
            }
        }
        return true;
    }

    void RemoveResourceReferences(GameObject res)
    {
        foreach(DictionaryEntry d in gs.aliens)
        {
            Alien a = d.Value as Alien;
            if (a.Resource == res)
            {
                a.DropResource();
                a.state = Alien.AlienState.SEARCHING;
            }
        }
    }
}
