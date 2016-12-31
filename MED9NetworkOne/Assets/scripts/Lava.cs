using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour {

	private GameObject MasterScriptObject;
	private Master M;

	public static bool LavaHit = false;
	public static bool GetLavaHit(){return LavaHit;}

	// Use this for initialization
	void Start () {

		MasterScriptObject = GameObject.Find ("MasterScript");
		M = MasterScriptObject.GetComponent<Master> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (LavaHit) {
			LavaHit = false;
		}
	
	}

	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "RobotPart") {
			LavaHit = true;
			M.PublicValuesToBeAssignedInInspector [3] = -1f;
		}
	}
}
