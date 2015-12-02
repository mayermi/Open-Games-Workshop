using UnityEngine;
using System.Collections;

[RequireComponent (typeof (LineRenderer))]
public class RecursiveLightning : MonoBehaviour {
	public int vertexCount = 17;
	public Vector3[] vertices;
	public Vector3 firstVertexPosition; 
	public Vector3 lastVertexPosition;
	public float fadeOutTime = 0.3f;
	public bool strikeOnStart = false;
	public bool fadeOutAfterStrike = true;
	public RecursiveLightning leftBranch = null;
	public RecursiveLightning rightBranch = null;
	LineRenderer lineRenderer;
	int leftBranchVertex = -1;
	int rightBranchVertex = -1;
	
	void Start () {
		InitializeLineRenderer();

		if(strikeOnStart)
			StrikeLightning();

	}

	void InsertFirstAndLastNode(){
		InsertNodeInLineRenderer(0, firstVertexPosition);
		InsertNodeInLineRenderer(vertexCount-1, lastVertexPosition);
	}

	void InsertVertexBetween(int start, int end){
		int currentVertexNumber = (start + end) /2;
        float dist = (firstVertexPosition - lastVertexPosition).magnitude;

		if(currentVertexNumber != start){
			vertices[currentVertexNumber] = (vertices[start] + vertices[end]) /2 + new Vector3(Random.Range(-dist*0.05f, dist * 0.05f), Random.Range(-dist * 0.05f, dist * 0.05f), 0);

			InsertNodeInLineRenderer(currentVertexNumber, vertices[currentVertexNumber]);

			InsertVertexBetween(start, currentVertexNumber);
			InsertVertexBetween(currentVertexNumber, end);
		}

		if(leftBranchVertex == currentVertexNumber){
			ConfigureBranch(leftBranch, vertices[currentVertexNumber], vertices[currentVertexNumber] + new Vector3(-2f,-3f,0));
			leftBranch.StrikeLightning();
		}

		if(rightBranchVertex == currentVertexNumber){
			ConfigureBranch(rightBranch, vertices[currentVertexNumber], vertices[currentVertexNumber] + new Vector3(2f,-3f,0));
			rightBranch.StrikeLightning();
		}
	}

	void ConfigureBranch(RecursiveLightning branch, Vector3 firstVertexPosition, Vector3 lastVertexPosition){
		branch.firstVertexPosition = firstVertexPosition;

		if(lastVertexPosition.y < vertices[vertexCount-1].y)
			lastVertexPosition.y = vertices[vertexCount-1].y;

		branch.lastVertexPosition = lastVertexPosition;

		branch.fadeOutTime = fadeOutTime;
		branch.fadeOutAfterStrike = fadeOutAfterStrike;
	}

	void InsertNodeInLineRenderer(int position, Vector3 vertex){
		lineRenderer.SetPosition(position, vertex);
	}

	void InitializeLineRenderer(){
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(vertexCount);

		lineRenderer.enabled = false;
	}

	public void StrikeLightning(){
		if(!lineRenderer.enabled){
			if(leftBranch){
				leftBranchVertex = Random.Range(0, vertexCount - 1);
			}

			if(rightBranch){
				rightBranchVertex = Random.Range(0, vertexCount - 1);
			}

			vertices = new Vector3[vertexCount];
			
			vertices[0] = firstVertexPosition;
			vertices[vertexCount -1] = lastVertexPosition;

			lineRenderer.enabled = true;
			InsertFirstAndLastNode();			
			InsertVertexBetween(0, vertexCount-1);

			if(fadeOutAfterStrike)
				FadeOut();
		}
	}

	public void FadeOut(){
		StartCoroutine("FadeOutQuickly");
	}

	IEnumerator FadeOutQuickly(){
		yield return new WaitForSeconds(fadeOutTime);
		lineRenderer.enabled = false;
	}

  
}
