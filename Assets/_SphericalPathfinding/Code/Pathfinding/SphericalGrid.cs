using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GridPointX
{
	public GridPointX(int size)
	{
		gridPointY = new GridPointY[size];
	}

	public GridPointY[] gridPointY;
}

[System.Serializable]
public class GridPointY
{
	public GridPointY()
	{
		neighbourPoints = new Vector2[0];
		nodeIds = new List<int>();
	}

	public Vector2[] neighbourPoints;
	public List<int> nodeIds = new List<int>();
}

public class GridPointDistance
{
	public GridPointDistance(int count)
	{
		dist = new float?[count, count];
	}

	public float?[,] dist;
}

[System.Serializable]
public class SphericalGrid : MonoBehaviour
{
	// Icosahedron values
	[Range(1, 56)]
	public int subDivisions = 4;	

	// planet
	public float planetRadius = 2f;
	public Vector3 planetCenter = Vector3.zero;

	// node
	public float nodeRadius = 0.04f;

	// icosahedron
	public Mesh[] icosahedronMesh;

	// grid
	[SerializeField][HideInInspector]
	GridPointX[] gridPointX;
	[SerializeField]
	[HideInInspector]
	public Node[] nodes; //TODO make this a multi dimension array for memory issues
	[SerializeField][HideInInspector]
	int roundNumber;

	// baking
	Vector3[] gridPositions;

	// Used for duplicates, finding neighbours and normalising 
	private float minVertDist;

	// Used by the heap in pathfinding
	[SerializeField]
	[HideInInspector]
	public int maxSize;



	// *************************
	//          BAKING
	// *************************

	public void RemoveNodes()
	{
		Debug.Log ("RemoveNodes");
		gridPositions = new Vector3[0];
		gridPointX = new GridPointX[0];
		nodes = new Node[0];
	}

	// Not used by the inspector
	public void BakeNodeProcess()
	{
		Debug.Log ("Started baking process");
		
		// sub divide the mesh
		// grab the grid positions from the vertices
		CreateGridPositions();
		
		// strip all duplicate points
		RemoveDuplicatePoints();
	
		// create nodes using grid Positions
		CreateNodes();
		
		// Add neighNodes
		AddNeighbourNodes();

		Debug.Log ("Baking process complete!");
	}


	// Used to to sub divide the icoshedron meshes
	// and to find the grid positions from the vertices
	public void CreateGridPositions()
	{
		Mesh[] newMeshes = new Mesh[icosahedronMesh.Length];
		List<Vector3> gridPosList = new List<Vector3>();

		// subdivide the icosahedron meshes and grab vertices points
		for(int i = 0; i < icosahedronMesh.Length; i++)
		{
			newMeshes[i] = this.Subdivide(icosahedronMesh[i], subDivisions);
			
			NormalMesh(newMeshes[i]);
			
			for(int p = 0; p < newMeshes[i].vertices.Length; p++)
			{
				gridPosList.Add(newMeshes[i].vertices[p]);
			}
		}

		gridPositions = gridPosList.ToArray();
	}

	// Using the given grid positions returns
	// nodes created for each position
	public void CreateNodes()
	{
		float sqrtOfNodeCount = Mathf.Sqrt((float)gridPositions.Length);
		roundNumber = Mathf.CeilToInt(360f / sqrtOfNodeCount);
		int arraySize = Mathf.CeilToInt(360f / roundNumber) + 1;

		gridPointX = new GridPointX[arraySize];
		for(int i = 0; i < gridPointX.Length; i++)
		{
			gridPointX[i] = new GridPointX(arraySize);
			
			for(int k = 0; k < gridPointX[i].gridPointY.Length; k++)
			{
				gridPointX[i].gridPointY[k] = new GridPointY();
			}
		}

		nodes = new Node[gridPositions.Length];

		for(int i = 0; i < gridPositions.Length; i++)
		{
			Vector3 spawnPos = -((planetCenter - gridPositions[i]).normalized) * planetRadius;

			// set up node
			Node newNode = new Node();
			newNode.nodeType = GroundTypeFromWorldPoint(spawnPos);
			newNode.worldPosition = spawnPos;

			// Create polar coordinates
			Vector2 newPolar = SphereicalCoordinateHelper.CartesianToPolar(newNode.worldPosition);
			newPolar.x += 180;
			newPolar.y += 180;

			int x = Mathf.RoundToInt(newPolar.x / roundNumber) * roundNumber;
			int y = Mathf.RoundToInt(newPolar.y / roundNumber) * roundNumber;

			x = Mathf.CeilToInt(x/roundNumber);
			y = Mathf.CeilToInt(y/roundNumber);

			newNode.gridPosition = new Vector2(x, y);
			newNode.id = i;

			nodes[i] = newNode;
			gridPointX[x].gridPointY[y].nodeIds.Add (i);
		}

		gridPositions = new Vector3[0];

		// ******************************************************** //
		// Mapping neighbour points to grid point
	
		GridPointDistance[,] gridPointDistance = new GridPointDistance[arraySize,arraySize];
		for(int x = 0; x < gridPointDistance.GetLength(0); x++)
		{
			for(int y = 0; y < gridPointDistance.GetLength(1); y++)
			{
				gridPointDistance[x,y] = new GridPointDistance(arraySize);
			}
		}

		float minGridPointDist = 10000f;

		for(int x = 0; x < 5; x++)
		{
			for(int y = 0; y < 5; y++)
			{
				for(int x2 = 0; x2 < 5; x2++)
				{
					for(int y2 = 0; y2 < 5; y2++)
					{
						if(x != x2 && y != y2)
						{
							float curDist = GetSphericalDistance(new Vector2((int)(x*roundNumber), (int)(y*roundNumber)), 
							                                     new Vector2((int)(x2*roundNumber), (int)(y2*roundNumber)));
							if(curDist < minGridPointDist)
							{
								minGridPointDist = curDist;
							}
						}
				   	}
			  	}
		   	}
	  	}

		minGridPointDist = minGridPointDist * 1.3f;

		for(int x = 0; x < arraySize; x++)
		{
			for(int y = 0; y < arraySize; y++)
			{
				List<Vector2> neighbourPoints = new List<Vector2>();

				for(int x2 = 0; x2 < arraySize; x2++)
				{
					for(int y2 = 0; y2 < arraySize; y2++)
					{
						if(gridPointDistance[x,y].dist[x2, y2] == null)
						{
							gridPointDistance[x2,y2].dist[x, y] = gridPointDistance[x,y].dist[x2, y2] = GetSphericalDistance(new Vector2((int)(x*roundNumber), (int)(y*roundNumber)), 
							                                                                                                 new Vector2((int)(x2*roundNumber), (int)(y2*roundNumber)));
							
							if(gridPointDistance[x,y].dist[x2, y2] < minGridPointDist)
							{
								neighbourPoints.Add (new Vector2(x2, y2));
							}
						}
						else
						{
							if(gridPointDistance[x,y].dist[x2, y2] < minGridPointDist)
							{
								neighbourPoints.Add (new Vector2(x2, y2));
							}
						}
					}
				}

				if(neighbourPoints.Count > 0)
				{
					gridPointX[x].gridPointY[y].neighbourPoints = neighbourPoints.ToArray();
				}
			}
		}

		gridPointDistance = new GridPointDistance[0,0];
        Debug.Log("Create Nodes done.");
	}

	// Adds neighbour nodes to all of the nodes
	public void AddNeighbourNodes()
	{
		for(int x = 0; x < gridPointX.Length; x++)
		{
			for(int y = 0; y < gridPointX[x].gridPointY.Length; y++)
			{
				for(int i = 0; i < gridPointX[x].gridPointY[y].nodeIds.Count; i++)
				{
					AddNeighbours(gridPointX[x].gridPointY[y].nodeIds[i], ((minVertDist*planetRadius)*1.5f));
				}
			}
		}

		//nodes = new Node[0];
	}



	// *************************
	//        UTILITIES
	// *************************

	public NodeType GroundTypeFromWorldPoint(Vector3 worldPos)
	{
		Vector3 dir = (planetCenter - worldPos).normalized;
		Vector3 startRayPos = -dir * (planetRadius * 1.1f);

		Ray ray = new Ray();
		ray.origin = startRayPos;
		ray.direction = dir;
		
		int nodeTypeLayer = 1<<10;
		RaycastHit hit;

		//Debug.DrawRay(startRayPos,  dir * radius);
		//Debug.Break();

		if(Physics.SphereCast(ray, nodeRadius, out hit, nodeTypeLayer))
		{
			return (NodeType)System.Enum.Parse(typeof(NodeType), hit.transform.tag);//hit.transform.tag
		}

		return NodeType.Null;
	}

	public float GetSphericalDistance(Node nodeA, Node nodeB)
	{
		float angleFromCenter = Vector3.Angle((nodeA.worldPosition - planetCenter),
		                                      (nodeB.worldPosition - planetCenter));
		
		return (2*Mathf.PI*planetRadius) * (angleFromCenter/360);
    }

	public float GetSphericalDistance(Vector3 pos01, Vector3 pos02)
	{
		float angleFromCenter = Vector3.Angle((pos01 - planetCenter),
		                                      (pos02 - planetCenter));
		
		return (2*Mathf.PI*planetRadius) * (angleFromCenter/360);
	}

	public float GetSphericalDistance(Vector2 polarPos01, Vector2 polarPos02)
	{
		Vector3 pos01 = SphereicalCoordinateHelper.PolarToCartesian(polarPos01, planetRadius);
		Vector3 pos02 = SphereicalCoordinateHelper.PolarToCartesian(polarPos02, planetRadius);

		float angleFromCenter = Vector3.Angle((pos01 - planetCenter),
		                                      (pos02 - planetCenter));
		
		return (2*Mathf.PI*planetRadius) * (angleFromCenter/360);
	}

	public Node NodeFromWorldPoint(Vector3 pos)
	{	
		// Convert world position to polar point and snap it to our grid
		Vector2 polarUVPoint = SphereicalCoordinateHelper.CartesianToPolar(pos);
		
		polarUVPoint.x += 180;
		polarUVPoint.y += 180;
		
		polarUVPoint.x = Mathf.RoundToInt(polarUVPoint.x / roundNumber) * roundNumber;
		polarUVPoint.y = Mathf.RoundToInt(polarUVPoint.y / roundNumber) * roundNumber;
		
		// Using the grid position gather all the nearest nodes from grid positions around the point
		int xPoint = (int)(polarUVPoint.x/roundNumber);
		int yPoint = (int)(polarUVPoint.y/roundNumber);
		Vector2[] neghbourGridPoints = gridPointX[xPoint].gridPointY[yPoint].neighbourPoints;
		List<int> curNodeCluster = new List<int>();
		
		for(int i = 0; i < neghbourGridPoints.Length; i++)
		{
			curNodeCluster.AddRange(gridPointX[(int)neghbourGridPoints[i].x].gridPointY[(int)neghbourGridPoints[i].y].nodeIds);
		}
		
		// Compare all the nearest node positions and return the closest
		float dist = 1000;
		Node curNode = null;
		
		for(int i = 0; i < curNodeCluster.Count; i++)
		{
			float curDist = GetSphericalDistance(pos, nodes[curNodeCluster[i]].worldPosition);
			if(curDist < dist)
			{
				dist = curDist;
				curNode = nodes[curNodeCluster[i]];
			}
		}
		
		return curNode;
	}
	
	Vector2 ClampToGridPosition(Vector2 p)
	{
		int xP = (int)p.x;
		int yP = (int)p.y;

		int gridCount = Mathf.CeilToInt(gridPointX.Length * roundNumber);

		if(xP < 0)
		{
			xP = gridCount - (roundNumber - xP);
		}
		else if(xP > gridCount)
		{
			xP = 0 + (roundNumber - (gridCount - xP));
		}
		else if(xP == 360)
		{
			xP = 0;
		}

		if(yP < 0)
		{
			yP = gridCount - (roundNumber - yP);
		}
		else if(yP > gridCount)
		{
			yP = 0 + (roundNumber - (gridCount - yP));
		}
		else if(yP == 360)
		{
			yP = 0;
		}
		
		return new Vector2(xP, yP);
	}



	// *************************
	//     BAKING UTILITIES
	// *************************

	public void AddNeighbours(int nodeId, float distance)
	{	
		Node node = nodes[nodeId];
		
		// Convert world position to polar point and snap it to our grid
		Vector2 polarUVPoint = SphereicalCoordinateHelper.CartesianToPolar(node.worldPosition);
		
		polarUVPoint.x += 180;
		polarUVPoint.y += 180;
		
		polarUVPoint.x = Mathf.RoundToInt(polarUVPoint.x / roundNumber) * roundNumber;
		polarUVPoint.y = Mathf.RoundToInt(polarUVPoint.y / roundNumber) * roundNumber;
		
		int xPoint = (int)(polarUVPoint.x/roundNumber);
		int yPoint = (int)(polarUVPoint.y/roundNumber);
		
		Vector2[] neghbourGridPoints = gridPointX[xPoint].gridPointY[yPoint].neighbourPoints;
		
		List<int> curNodeCluster = new List<int>();
		
		for(int i = 0; i < neghbourGridPoints.Length; i++)
		{
			curNodeCluster.AddRange(gridPointX[(int)neghbourGridPoints[i].x].gridPointY[(int)neghbourGridPoints[i].y].nodeIds);
		}
		
		// Compare all the nearest node positions and return the closest
		List<int> neighbourIds = new List<int>();
		
		for(int i = 0; i < curNodeCluster.Count; i++)
		{
			float curDist = GetSphericalDistance(node.worldPosition, nodes[curNodeCluster[i]].worldPosition);
			if(curDist < distance)
			{
				neighbourIds.Add (nodes[curNodeCluster[i]].id);
			}
		}
		
		node.neighbours = neighbourIds.ToArray();
	}

	public void RemoveDuplicatePoints()
	{

		Debug.Log ("RemoveDuplicatePoints - start count: "+gridPositions.Length);

		List<int> crossList = new List<int>();
		crossList.Add(0);

		for(int i = 1; i < gridPositions.Length; i++)
		{
			bool duplicate = false;

			for(int p = 0; p < crossList.Count; p++)
			{
				if(i != crossList[p])
				{
					if(Vector3.Distance(gridPositions[i], gridPositions[crossList[p]]) < (minVertDist * 0.5f))
					{
						duplicate = true;
						break;
					}
				}
			}

			// if current i wasn't a duplicate to anything in the cross list
			// add it to the list
			if(!duplicate)
			{
				crossList.Add(i);
			}
		}

		// Build a new list with all the non duplicate nodes
		List<Vector3> newPoints = new List<Vector3>();
		for(int i = 0; i < crossList.Count; i++)
		{
			newPoints.Add(gridPositions[crossList[i]]);
		}

		maxSize = crossList.Count;
		Debug.Log ("RemoveDuplicatePoints - end count: "+crossList.Count);
		gridPositions = newPoints.ToArray();
	}

	Mesh Subdivide(Mesh startMesh, int subDivisions)
	{
		int Tris = subDivisions * subDivisions;
		int latitude = 0;
		int longitude = 0;
		bool upTri = true;
		int incAmount = 1;

		Vector3[] oldVerts = startMesh.vertices;
		Vector2[] oldUVs = startMesh.uv;

		Vector3[] newVerts = new Vector3[Tris * 3];
		int[] newTris = new int[Tris * 3];
		Vector2[] newUVs = new Vector2[Tris * 3];

		Vector3 midBase = (oldVerts[2]+oldVerts[1]) * 0.5f;
		Vector3 downVec = (midBase - oldVerts[0]) / subDivisions;
		Vector3 rightVec = (oldVerts[2] - oldVerts[1]) / (subDivisions * 2f);

		Vector2 midBaseUV = (oldUVs[2] + oldUVs[1]) * 0.5f;
		Vector2 downVecUV = (midBaseUV - oldUVs[0]) / subDivisions;
		Vector2 rightVecUV = (oldUVs[2] - oldUVs[1]) / (subDivisions * 2f);

		for(int i = 0; i < Tris; i++)
		{
			if(upTri) // is tri facing up
			{
				// new vertices
				newVerts[i*3] = oldVerts[0] + ((longitude * rightVec) + (latitude * downVec));
				newVerts[(i*3)+1] = newVerts[i*3] + downVec - rightVec;
				newVerts[(i*3)+2] = newVerts[i*3] + downVec + rightVec;

				//new UVs
				newUVs[i*3] = oldUVs[0] + (longitude * rightVecUV) + (latitude * downVecUV);
				newUVs[(i*3)+1] = newUVs[i*3] + downVecUV - rightVecUV;
				newUVs[(i*3)+2] = newUVs[i*3] + downVecUV + rightVecUV;
			}
			else // tri is facing down
			{
				// new vertices
				newVerts[i*3] = oldVerts[0] + ((longitude * rightVec) + ((latitude * downVec) + downVec));
				newVerts[(i*3)+1] = newVerts[i*3] - downVec + rightVec;
				newVerts[(i*3)+2] = newVerts[i*3] - downVec - rightVec;
				
				//new UVs
				newUVs[i*3] = oldUVs[0] + (longitude * rightVecUV) + ((latitude * downVecUV) + downVecUV);
				newUVs[(i*3)+1] = newUVs[i*3] - downVecUV + rightVecUV;
				newUVs[(i*3)+2] = newUVs[i*3] - downVecUV - rightVecUV;
			}

			//tris
			newTris[i*3] = i*3;
			newTris[(i*3)+2] = (i*3)+1;
			newTris[(i*3)+1] = (i*3)+2;


			incAmount--;
			longitude++;
			upTri = !upTri;

			if(incAmount == 0)
			{
				latitude++;
				upTri = true;
				longitude = 0 - latitude;
				incAmount = ((latitude+1) * 2) -1;
			}

		}

		Mesh newMesh = new Mesh();
		newMesh.vertices = newVerts;
		newMesh.triangles = newTris;
		newMesh.uv = newUVs;

		return newMesh;
	}

	void NormalMesh(Mesh mesh)
	{
		float minDist = 10f;
		float maxDist = 0f; // Can remove

		Vector3[] newVertices = mesh.vertices;
		
		for(int k = 0; k < newVertices.Length; k++)
		{
			newVertices[k].Normalize();
		}
		
		mesh.vertices = newVertices;
		mesh.normals = newVertices;
		
		for(int k = 0; k < newVertices.Length; k++)
		{
			if(k%3 == 0)
			{
				float pointDist = Vector3.Distance(newVertices[k+1], newVertices[k+2]);
				
				if(pointDist < minDist) minDist = pointDist;
				if(pointDist > maxDist) maxDist = pointDist; // Can remove
			}
		}
		
		minDist *= 0.99f;
		maxDist *= 1.01f; // Can remove
		minVertDist = (minDist);
	}


	// *************************
	//          DEBUG
	// *************************

	public bool ShowBakedNodes = false;

	public void OnDrawGizmos()
	{
		if(ShowBakedNodes)
		{
			for(int i = 0; i < nodes.Length; i++)
			{
				if(nodes[i].nodeType == NodeType.NotWalkable)
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.green;
				}

				Gizmos.DrawSphere(nodes[i].worldPosition, nodeRadius);
			}
		}
	}

}
