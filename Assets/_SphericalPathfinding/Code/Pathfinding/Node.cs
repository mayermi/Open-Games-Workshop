using UnityEngine;
using System.Collections;

public enum NodeType
{
	Null,
	Walkable,
	NotWalkable
}

[System.Serializable]
public class Node : IHeapItem<Node>
{
	public Vector3 worldPosition;
	public Vector2 gridPosition;
	public int id;

	public bool Mark = false;

	[SerializeField]
	public int[] neighbours;

	public float gCost;
	public float hCost;
	public Node parent;

	public bool path;

	public float fCost
	{
		get {
			return gCost + hCost;
		}
	}

	[SerializeField]
	private NodeType _nodeType;
	public NodeType nodeType
	{
		get { return this._nodeType; }
		set
		{
			this._nodeType = value;
		}
	}

	private int _heapIndex;
	public int HeapIndex
	{
		get {
			return this._heapIndex;
		}
		set {
			this._heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if(compare == 0)
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}

		return -compare;
	}
}
