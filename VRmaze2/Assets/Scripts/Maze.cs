using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
	[System.Serializable]
	public class Cell {
		public bool visited;
		public GameObject north;//1
		public GameObject east;//2
		public GameObject west;//3
		public GameObject south;//4

	}
		
	public Texture floor;
	public GameObject wall;
	public float wallLength = 1.0f;
	public static int xSize = 10;
	public static int ySize = 10;
	private GameObject wallHolder;
	private GameObject floorHolder;
	private Cell[] cells;
	private int currentCell = 0;
	private int totalCells;
	private int visitedCells = 0;
	private bool startedBuilding = false;
	private int currentNeighbour;
	private List<int> lastCells;
	private int backingUp = 0;
	private int wallToBreak = 0;

	// Use this for initialization
	void Awake() {
		CreateWalls ();
	}

	void CreateWalls (){
		wallHolder = new GameObject ();
		wallHolder.name = "Maze";
		//floorHolder = new GameObject ();
		//floorHolder.name = "Maze Floor";

		Vector3 myPos;
		GameObject tempWall;

		GameObject tempPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		// meshRenderer = tempPlane.GetComponent<MeshRenderer> ();
		//Material mat = new Material(Shader.Find("Standard"));
		//mat.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		//meshRenderer.material = mat;

		Vector3 center = new Vector3(xSize*0.75f, 0.3f, ySize*0.75f - 0.75f);
		tempPlane.transform.position = center;
		tempPlane.transform.localScale = new Vector3(xSize/6.5f, 1.0f, ySize/6.5f);

		//For x axis
		for (int i = 0; i < ySize; i++) {
			for (int j = 0; j <= xSize; j++) {
				
				myPos = new Vector3 ((j * wallLength), 0.5f, (i * wallLength));
				tempWall = Instantiate (wall, myPos,Quaternion.identity) as GameObject;
				tempWall.transform.parent = wallHolder.transform;
//				tempWall.layer = 8;

				/*if (j != xSize) {
					tempFloor = Instantiate (floor, myPos - (new Vector3(-0.8f, 0.3f, 0.0f)), Quaternion.identity) as GameObject;
					tempFloor.transform.parent = floorHolder.transform;
				}*/
			}
		}

		//For y axis
		for (int i = 0; i <= ySize; i++) {
			for (int j = 0; j < xSize; j++) {
				myPos = new Vector3 ((j * wallLength) + wallLength/2, 0.5f, (i * wallLength) - wallLength/2);
				tempWall = Instantiate (wall, myPos,Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
				tempWall.transform.parent = wallHolder.transform;
//				tempWall.layer = 8;
			}
		}

		CreateCells ();
	}

	void CreateCells(){
		lastCells = new List<int> ();
		lastCells.Clear ();
		totalCells = xSize * ySize;
		GameObject[] allWalls;
		int children = wallHolder.transform.childCount;
		//print ("Children: " + children);
		allWalls = new GameObject[children];
		cells = new Cell[xSize * ySize];
		int eastWestProcess = 0;
		int childProcess = 0;
		int termCount = 0;

		//Get all children
		for (int i = 0; i < children; i++) {
			allWalls [i] = wallHolder.transform.GetChild (i).gameObject;
		}

		//Assigns walls to cells
		for (int cellprocess = 0; cellprocess < cells.Length; cellprocess++) {

			if (termCount == xSize) {
				eastWestProcess++;
				termCount = 0;
			}

			cells [cellprocess] = new Cell ();
			cells [cellprocess].west = allWalls [eastWestProcess];
			cells [cellprocess].south = allWalls [childProcess + ((xSize + 1) * ySize)];
			cells [cellprocess].west.layer = 8;
			cells [cellprocess].south.layer = 8;

			eastWestProcess++;
			termCount++;
			childProcess++;
			cells [cellprocess].east = allWalls [eastWestProcess];
			cells [cellprocess].north = allWalls [(childProcess + ((xSize + 1) * ySize)) + xSize - 1];
			cells [cellprocess].east.layer = 8;
			cells [cellprocess].north.layer = 8;
		}

		CreateMaze ();
	}

	void CreateMaze (){
		while (visitedCells < totalCells) {
			if (startedBuilding) {
				GiveMeNeighbours ();
				if (cells [currentNeighbour].visited == false && cells [currentCell].visited == true) {
					BreakWall ();
//					GiveMeNeighbours ();
//					BreakWall ();
					cells [currentNeighbour].visited = true;
					visitedCells++;
					lastCells.Add (currentCell);
					currentCell = currentNeighbour;

					if (lastCells.Count > 0) {
						backingUp = lastCells.Count - 1;
					}
				}
			} else {
				currentCell = Random.Range (0, totalCells);
				cells [currentCell].visited = true;
				visitedCells++;
				startedBuilding = true;
			}

			//Invoke ("CreateMaze", 0.0f);
		}

		Debug.Log ("Maze Creation Finished!");
	}

	void BreakWall (){
		switch (wallToBreak) {
		case 1:
			cells [currentCell].north.layer = 1;
			Destroy (cells [currentCell].north);
			break;
		case 2:
			cells [currentCell].east.layer = 1;
			Destroy (cells [currentCell].east);
			break;
		case 3:
			cells [currentCell].west.layer = 1;
			Destroy (cells [currentCell].west);
			break;
		case 4:
			cells [currentCell].south.layer = 1;
			Destroy (cells [currentCell].south);
			break;
		}
	}

	void GiveMeNeighbours (){
		int length = 0;
		int[] neightbours = new int[4];
		int[] connectingWalls = new int[4];
		int check = (currentCell + 1) / xSize;
		check -= 1;
		check *= xSize;
		check += xSize;

		//east
		if ((currentCell + 1) < totalCells && (currentCell + 1) != check) {
			if (cells [currentCell + 1].visited == false) {
				neightbours [length] = currentCell + 1;
				connectingWalls [length] = 2;
				length++;
			}
		}

		//west
		if ((currentCell + 1) >= 0 && currentCell != check) {
			if (cells [currentCell - 1].visited == false) {
				neightbours [length] = currentCell - 1;
				connectingWalls [length] = 3;
				length++;
			}
		}

		//north
		if (currentCell + xSize < totalCells) {
			if (cells [currentCell + xSize].visited == false) {
				neightbours [length] = currentCell + xSize;
				connectingWalls [length] = 1;
				length++;
			}
		}

		//south
		if (currentCell - xSize >= 0) {
			if (cells [currentCell - xSize].visited == false) {
				neightbours [length] = currentCell - xSize;
				connectingWalls [length] = 4;
				length++;
			}
		}

		if (length != 0) {
			int theChosenOne = Random.Range (0, length);
			currentNeighbour = neightbours [theChosenOne];
			wallToBreak = connectingWalls [theChosenOne];
		} else {
			if (backingUp > 0) {
				currentCell = lastCells [backingUp];
				backingUp--;
			}
		}

		/*for (int i = 0; i < length; i++) {
			Debug.Log (neightbours [i]);
		}*/
	}
	// Update is called once per frame
	void Update () {
	
	}
}
