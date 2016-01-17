using UnityEngine;
using System.Collections;

public class AlienHelper : CreatureHelper {

	Alien alien;
    UIManager ui;
    bool movingToShipWithResource = false;
    public bool movingToShipToLeave = false;
    bool infectionReady = true;
	private AudioClip resourceSound;
	private AudioSource source;

	public override void Start () {
		base.Start ();
        ui = GameObject.Find("UI").GetComponent<UIManager>();
        alien = gs.aliens[gameObject] as Alien;
        gameObject.GetComponent<SphereCollider>().radius = alien.VisionRange;
		source = gameObject.AddComponent<AudioSource>();
        resourceSound = (AudioClip)Resources.Load ("resource");
		source.clip = resourceSound;
		source.playOnAwake = false;
    }

	public override void Update () {
        base.Update ();

        if (alien == null)
		{
			alien = gs.aliens [gameObject] as Alien;
		} 
		else if (alien.state == Alien.AlienState.SEARCHING && !alien.movingToResource)
		{
			alien.Search ();
		} 
		else if (alien.state == Alien.AlienState.CARRYING)
		{
			alien.CarryResource ();
		} 
		else if (alien.state == Alien.AlienState.FLEEING)
		{
			alien.Flee();
		}

        if (alien.movingToResource) CheckDistToResource();
        if (movingToShipWithResource) CheckDistToShip();
        if (movingToShipToLeave) CheckDistToShip();
        if (alien.Infected && infectionReady)
		{
			infectionReady = false;
			StartCoroutine(InfectionDamage(3));
		}
    }

    public override void NoPathFound()
    {
        base.NoPathFound();
        alien.ResetTarget();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "resource" && alien.state == Alien.AlienState.SEARCHING && !movingToShipToLeave)
        {
            // check if resource is free to pick up
            if(DoesNotBelongToOtherAlien(other.gameObject))
            {
                Debug.Log("Detected Resource");
                alien.MoveTo(other.gameObject.transform.position);
                alien.movingToResource = true;
                alien.Resource = other.gameObject;
            }
            
        }
		
        // for infecting other aliens
        if(alien.Infected)
        {
            if (gs.aliens.Contains(other.gameObject))
            {
                Alien a = gs.aliens[other.gameObject] as Alien;
                a.Infected = true;
                a.GameObject.transform.Find("Infection").GetComponent<ParticleSystem>().Play();
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
            alien.movingToResource = false;
            movingToShipWithResource = true;
        }
    }

    void CheckDistToShip()
    {
        float dist = (gameObject.transform.position - GameValues.ShipPos).magnitude;
        if (dist <= 5f)
        {
            if(movingToShipToLeave)
            {
                gs.aliensSaved += 1;
                alien.EnterSpaceShip();
                movingToShipToLeave = false;
                return;
            }

            gs.CollectedResources += 1;
            source.Play();
            GameObject res = alien.Resource;
            // check if other Aliens were trying to reach this specific resource too
            RemoveResourceReferences(res);
            Destroy(res);
            movingToShipWithResource = false;

			ui.SetResourceSlider();

            if (gs.CollectedResources == gs.resourcesNeeded) CallAliensToShip();
            
        }
    }

    void CallAliensToShip()
    {
        GameObject.Find("GameController").SendMessage("ReplaceShipModel");
        foreach (DictionaryEntry d in gs.aliens)
        {
            Alien a = d.Value as Alien;
            a.ReturnToShip(true);
            if (a.Resource) a.DropResource();
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

    public override void AdjustHealthBar()
    {
        base.AdjustHealthBar();
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

	IEnumerator InfectionDamage(float sec)
	{
		alien.TakeDamage (2);
		yield return new WaitForSeconds (sec);
		infectionReady = true;
	}
	
}
