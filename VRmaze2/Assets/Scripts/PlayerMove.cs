using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {
	public Transform vrCamera;
	public float toggleangle = 30.0f;
	public float speed = 3.0f;
	private bool moveforward;
	private bool clickmove=false;
	public static bool Doubleclick=false;
	private CharacterController cc;
	private float lastclicktime=0.0f;
	float catchtime=0.25f;
	private int keysCollected = 0;
	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController> ();
		keysCollected = 0;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Mouse0)){
			if(Time.time-lastclicktime<catchtime){
				Doubleclick = true;
				print("done:" + (Time.time-lastclicktime).ToString());
			}else{
				Doubleclick = false;
				print("miss:" + (Time.time-lastclicktime).ToString());
			}
			lastclicktime=Time.time;
		}
		if (clickmove) {
			moveforward = true;
			checkmove2 ();
		} 
		else {
			checkmove1 ();
		}
		if(moveforward)
		{
			Vector3 forward = vrCamera.TransformDirection (Vector3.forward);
			cc.SimpleMove (forward * speed);
		}
	}

	void checkmove1(){
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			clickmove = true;
			Doubleclick = false;
		}
	}

	void checkmove2(){
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			clickmove = false;
			moveforward = false;
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.name.Equals ("key_gold(Clone)"))
		{
			other.gameObject.SetActive (false);
			keysCollected++;
			print ("Key " + keysCollected + " collected!");
		}
	}
		
}
