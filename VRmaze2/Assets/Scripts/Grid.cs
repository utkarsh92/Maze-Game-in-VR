using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public GameObject key;
	Node[,] grid;
	private int[,] temp;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Start() {
		nodeDiameter = nodeRadius*2;
		gridWorldSize.x = Maze.xSize*1.5f;
		gridWorldSize.y = Maze.ySize*1.5f;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}

	void CreateGrid() {
		Debug.Log ("Grid Started!");
		grid = new Node[gridSizeX,gridSizeY];
		Vector3 center1 = new Vector3 (Maze.xSize*0.75f, 0.3f, Maze.ySize*0.75f - 0.75f);
		transform.position = center1;
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
				grid[x,y] = new Node(walkable,worldPoint, x,y);
			}
		}
		GenrateKeys ();
	}

	public void GenrateKeys() {

		int ranX = 0, ranY = 0, keyCount = 0, count = 0, i, j;
		temp = new int [3, 2];
		Node currentNode;

		while (keyCount != 3) {
			count = 0;
			ranX = Random.Range (1, gridSizeX - 1);
			ranY = Random.Range (1, gridSizeY - 1);
			currentNode = grid [ranX, ranY];

			for (i = 0; i < keyCount; i++) {
					if (temp [i, 0] == ranX && temp [i, 1] == ranY)
						continue;
			}

			if (currentNode.walkable) {
				if (!grid [ranX, ranY + 1].walkable)
					count++;
				if (!grid [ranX + 1, ranY].walkable)
					count++;
				if (!grid [ranX, ranY - 1].walkable)
					count++;
				if (!grid [ranX - 1, ranY].walkable)
					count++;

				if (count == 3) {
					Instantiate (key, currentNode.worldPosition, Quaternion.identity);
					temp [keyCount, 0] = ranX;
					temp [keyCount, 1] = ranY;
					keyCount++;
				}
			} else {
				continue;
			}
		}

		Debug.Log ("Keys Genrated!");
	}

	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}


	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}

	public List<Node> path;

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		if (grid != null) {
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				if (path != null) {
					if (path.Contains (n))
						Gizmos.color = Color.black;
				}
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}
}