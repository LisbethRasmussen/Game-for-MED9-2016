using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnergyBall : NetworkBehaviour {

	private GameObject MasterScriptObject;
	private Master M;

	private Transform WhereToAimAt;

	// Use this for initialization
	void Start () {

		MasterScriptObject = GameObject.Find ("MasterScript"); //Looking for the original MasterScript object
		M = MasterScriptObject.GetComponent<Master> ();			//Retrieving the Master script component

		WhereToAimAt = M.TransformObjects[5];
		transform.LookAt (M.Positions3D[2]);

	}
	
	// Update is called once per frame
	void Update () {

		transform.localPosition += transform.forward * M.PublicValuesToBeAssignedInInspector[4]*Time.deltaTime;
	
	}

	void OnTriggerEnter (Collider other){

		if (other.gameObject.tag == "AllPurposeTag"){
			Destroy (gameObject);
		}
	}
	void OnCollisionEnter (Collision other){

		if (other.gameObject.tag == "AllPurposeTag"){
			Destroy (gameObject);
		}
	}
}
