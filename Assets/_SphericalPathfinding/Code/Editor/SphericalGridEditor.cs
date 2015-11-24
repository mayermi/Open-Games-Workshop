using UnityEditor;
using UnityEngine;
using System.Collections;

[System.Serializable]
[CustomEditor(typeof(SphericalGrid))]
public class SphericalGridEditor : Editor
{
	SphericalGrid sphericalGrid;

	// Baking process
	[HideInInspector]
	public float progressComplete = 0; // 0 - 1
	[HideInInspector]
	public string progressMessage = "";

	private BakingState bakingState = BakingState.NotBaking;
	public enum BakingState
	{
		StartBaking,
		NotBaking,
		Complete,
		GridPositions,
		RemoveDuplicates,
		CreatingNodes,
		AddNeighbourNodes,
		Cancel
	}

	float timerStart;

	void Awake()
	{
		sphericalGrid = (SphericalGrid)target;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if(bakingState == BakingState.NotBaking)
		{
			if(GUILayout.Button("Bake Nodes"))
			{
				bakingState = BakingState.StartBaking;
				sphericalGrid.RemoveNodes();
				BakeNodeProcess();
			}
		}
		else
		{
			if(GUILayout.Button("Cancel"))
			{
				bakingState = BakingState.Cancel;
			}
		}


		if(GUILayout.Button("Remove Nodes") && bakingState == BakingState.NotBaking)
		{
			sphericalGrid.RemoveNodes();

			EditorUtility.UnloadUnusedAssets();
			Resources.UnloadUnusedAssets();
			System.GC.Collect();
		}
	}

	void OnInspectorUpdate()
	{
		Repaint();
	}
	
	public void BakeNodeProcess()
	{
		if(bakingState != BakingState.NotBaking)
		{
			switch(bakingState)
			{

			case(BakingState.StartBaking):

				EditorApplication.update += BakeNodeProcess;
				timerStart = (float)EditorApplication.timeSinceStartup;
				bakingState = BakingState.GridPositions;

				break;

			case(BakingState.GridPositions):

				if(((float)EditorApplication.timeSinceStartup - timerStart) <= 1f)
				{
					progressComplete = 0.3f;
					progressMessage = "Sub dividing mesh and finding grid positions in new vertices";
				}
				else
				{
					// sub divide the mesh
					// grab the grid positions from the vertices
					sphericalGrid.CreateGridPositions();

					timerStart = (float)EditorApplication.timeSinceStartup;
					bakingState = BakingState.RemoveDuplicates;
				}

				EditorUtility.DisplayProgressBar("Baking Nodes", progressMessage, progressComplete);

				break;

			case(BakingState.RemoveDuplicates):

				if(((float)EditorApplication.timeSinceStartup - timerStart) <= 1f)
				{
					progressComplete = 0.5f;
					progressMessage = "Removing duplicate points";
				}
				else
				{
					// strip all duplicate points
					sphericalGrid.RemoveDuplicatePoints();

					timerStart = (float)EditorApplication.timeSinceStartup;
					bakingState = BakingState.CreatingNodes;
				}

				EditorUtility.DisplayProgressBar("Baking Nodes", progressMessage, progressComplete);

				break;

			case(BakingState.CreatingNodes):

				if(((float)EditorApplication.timeSinceStartup - timerStart) <= 1f)
				{
					progressComplete = 0.7f;
					progressMessage = "Creating nodes";
				}
				else
				{
					// create nodes using grid Positions
					sphericalGrid.CreateNodes();

					timerStart = (float)EditorApplication.timeSinceStartup;
					bakingState = BakingState.AddNeighbourNodes;
				}

				EditorUtility.DisplayProgressBar("Baking Nodes", progressMessage, progressComplete);

				break;

			case(BakingState.AddNeighbourNodes):

				if(((float)EditorApplication.timeSinceStartup - timerStart) <= 2f)
				{
					progressComplete = 0.9f;
					progressMessage = "Adding neighbour Nodes";
				}
				else
				{
					// Add neighNodes
					sphericalGrid.AddNeighbourNodes();

					timerStart = (float)EditorApplication.timeSinceStartup;
					bakingState = BakingState.Complete;
				}

				EditorUtility.DisplayProgressBar("Baking Nodes", progressMessage, progressComplete);

				break;

			case(BakingState.Complete):

				if(((float)EditorApplication.timeSinceStartup - timerStart) <= 2f)
				{
					progressComplete = 1f;
					progressMessage = "Bake complete!";
					EditorUtility.DisplayProgressBar("Baking Nodes", progressMessage, progressComplete);
				}
				else
				{
					
					EditorUtility.ClearProgressBar();
					bakingState = BakingState.NotBaking;

					EditorUtility.SetDirty(sphericalGrid);
					serializedObject.ApplyModifiedProperties();

					EditorUtility.UnloadUnusedAssets();
					Resources.UnloadUnusedAssets();
				}

				break;

			case(BakingState.Cancel):
			
				if(((float)EditorApplication.timeSinceStartup - timerStart) <= 2f)
				{
					progressComplete = 0f;
					progressMessage = "Bake cancelled!";
					EditorUtility.DisplayProgressBar("Baking Nodes", progressMessage, progressComplete);
				}
				else
				{
					
					EditorUtility.ClearProgressBar();

					EditorUtility.UnloadUnusedAssets();
					Resources.UnloadUnusedAssets();
					System.GC.Collect();

					bakingState = BakingState.NotBaking;
				}

				break;
			}
		}
		else
		{
			EditorApplication.update -= BakeNodeProcess;
		}
	}
}

