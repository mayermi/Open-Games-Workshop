using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

    /* To be used like this
	 * CREATE
	 * ShyMonster m = new ShyMonster (1,1,0.15f,1);
	 * m.GameObject = Creator.Create ("monster", new Vector3(0,80,-100));
	 * gs.monsters.Add (m.GameObject, m);
	 * 
	 * USE
	 * Monster c = gs.monsters [transform.gameObject] as Monster;
	 * 
	*/

    public Hashtable creatures = new Hashtable();
	public Hashtable monsters = new Hashtable();
	public Hashtable aliens = new Hashtable();

	[SerializeField]
	public int maxAliens = 10;
    [SerializeField]
    public int maxResources = 20;
	public int CollectedResources { get; set; }
    public int ActiveSkill { get; set; }

	public Vector3 ShipPos { get; set; }

    private List<Vector3> _monsterSpawnPoints = new List<Vector3>();
    public List<Vector3> MonsterSpawnPoints { get { return _monsterSpawnPoints; } }

	void RemoveCreature(Creature c) {
		if (c is Monster) {
			monsters.Remove (c.GameObject);
		} else {
			aliens.Remove (c.GameObject);
		}
        creatures.Remove(c.GameObject);
	}

}
