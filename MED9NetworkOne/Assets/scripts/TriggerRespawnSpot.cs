using UnityEngine;
using System.Collections;

public class TriggerRespawnSpot : MonoBehaviour {

	private GameObject MasterScriptObject; //two lines for accessing public variables in the Master script placed on the MasterScript object
	private Master M;

	void Start () {

		MasterScriptObject = GameObject.Find ("MasterScript"); //Looking for the original MasterScript object
		M = MasterScriptObject.GetComponent<Master> ();			//Retrieving the Master script component
	
	}

	void Update () {
	
	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "RobotPartC") {
			M.Positions3D [0] = transform.position;
		}
	}
}
