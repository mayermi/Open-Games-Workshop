using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

[RequireComponent (typeof (SphericalGrid))]
[RequireComponent (typeof (PathRequestManager))]
public class AStarPathfinding : MonoBehaviour
{
	PathRequestManager requestManager;
	SphericalGrid 
		sphericalGrid;


	#region Unity

	void Awake()
	{
		requestManager = GetComponent<PathRequestManager>();
		sphericalGrid = GetComponent<SphericalGrid>();
	}

	#endregion



	public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
		StartCoroutine(FindPath(startPos,targetPos));
	}



	IEnumerator FindPath(Vector3 _startPos, Vector3 _targetPos)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = sphericalGrid.NodeFromWorldPoint(_startPos);
		Node targetNode = sphericalGrid.NodeFromWorldPoint(_targetPos);

		Heap<Node> openSet = new Heap<Node>(sphericalGrid.maxSize);
		HashSet<Node> closedSet = new HashSet<Node>();

		openSet.Add(startNode);

		while(openSet.Count > 0)
		{
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add (currentNode);

			if(currentNode == targetNode)
			{
				sw.Stop();
				print ("Path found: " + sw.ElapsedMilliseconds + "ms");
				pathSuccess = true;
				break;
			}

			foreach(int nodeId in currentNode.neighbours)
			{
				Node neighbour = sphericalGrid.nodes[nodeId];

				if (neighbour.nodeType == NodeType.NotWalkable || closedSet.Contains(neighbour))
					continue;

				float newMovementCostToNeighbour = currentNode.gCost + sphericalGrid.GetSphericalDistance(currentNode, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost ||
				    !openSet.Contains(neighbour))
				{
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = sphericalGrid.GetSphericalDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if (!openSet.Contains(neighbour))
					{
						openSet.Add(neighbour);
					}
					else
						openSet.UpdateItem(neighbour);
				}
			}
		}

		yield return null;

		if(pathSuccess)
		{
			waypoints = RetracePath(startNode, targetNode);
		}

		requestManager.FinishedProcessingPath(waypoints, pathSuccess);
	}



	Vector3[] RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		path.Reverse();
		return waypoints;
	}


	Vector3[] SimplifyPath(List<Node> path)
	{
		List<Vector3> waypoints = new List<Vector3>();
		Vector3 directionOld = Vector3.zero;
		
		for (int i = 1; i < path.Count; i ++)
		{
			Vector3 directionNew = (path[i-1].worldPosition - path[i].worldPosition).normalized;
			float diff = Vector3.Distance(directionNew, directionOld);

			//if (directionNew != directionOld)
			if(diff >= 0.1f)
			{
				waypoints.Add(path[i].worldPosition);
			}

			directionOld = directionNew;
		}

		return waypoints.ToArray();
	}
}
