using UnityEngine;
using System.Collections;

public class ghostcam : MonoBehaviour {
	public GameObject player;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!PlayerMove.Doubleclick) {
			transform.position = player.transform.position + Vector3.up * 1.0f;
//			transform.eulerAngles = new Vector3 (0,0,0);
//			transform.rotation = Quaternion.Euler(0.0f,0.0f,0.0f);
		} else {
			Vector3 center1 = new Vector3 (Maze.xSize*0.75f, 15.0f, Maze.ySize*0.75f - 0.75f);
			transform.position = center1;
//			Vector3 rot;
//			rot.x = 85.0f;
//			rot.y = 0.0f;
//			rot.z = 0.0f;
//			transform.eulerAngles = rot;
//			transform.rotation = Quaternion.Euler(85,0,0);
//			transform.eulerAngles = new Vector3(85.0f,0.0f,0.0f);

		}
	}
}
