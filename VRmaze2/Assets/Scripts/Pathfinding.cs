using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

	List<Node> path = new List<Node>();
	public Transform seeker, target;
	public GameObject floor;
	private GameObject tempFloor;
	private GameObject[] pathFloors;
	private GameObject mainCamera;
	private int flag = 2;
	private int w, h;
	Grid grid;

	void Start() {
		grid = GetComponent<Grid> ();
		mainCamera = GameObject.FindWithTag ("MainCamera");

		w = Screen.width / 2;
		h = Screen.height / 2;
	}

	void Update() {
		if (PlayerMove.Doubleclick) {

			Ray ray = mainCamera.GetComponent<Camera> ().ScreenPointToRay (new Vector3 (w, h));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {

				//print ("Hit: " + hit.collider.tag);
				if (hit.collider.name.Equals("key_gold(Clone)")) {
					//Debug.Log("Found Key!");
					target = hit.collider.transform;
					flag = 0;
				}
			}
				
			if (flag == 0) {
				//Debug.Log("3");

				for (int i = 0; i < path.Count; i++) {
					Destroy (pathFloors [i]);
				}
				path.Clear ();

				FindPath (seeker.position, target.position);
				//Debug.Log ("path: " + path.Count);
				pathFloors = new GameObject[path.Count];

				for (int i = 0; i < path.Count; i++) {
					tempFloor = Instantiate (floor, path [i].worldPosition, Quaternion.identity) as GameObject;
					pathFloors [i] = tempFloor;
				}
				flag = 1;
			}

		} else {
			if (flag == 1) {
				//Debug.Log("4");
				for (int i = 0; i < path.Count; i++) {
					Destroy (pathFloors [i]);
				}
				flag = 2;
			}
		}
	}

	void FindPath(Vector3 startPos, Vector3 targetPos) {
		Vector3 center1 = new Vector3 (Maze.xSize*0.75f, 25.3f, Maze.ySize*0.75f - 0.75f);
		startPos = startPos - center1;
		targetPos = targetPos - center1;
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0) {
			Node node = openSet[0];
			for (int i = 1; i < openSet.Count; i ++) {
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode) {
				RetracePath(startNode,targetNode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbours(node)) {
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}

	void RetracePath(Node startNode, Node endNode) {
		
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();

		grid.path = path;

	}

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
}