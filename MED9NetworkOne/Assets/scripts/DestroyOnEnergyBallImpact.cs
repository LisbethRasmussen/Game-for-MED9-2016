using UnityEngine;
using System.Collections;

public class DestroyOnEnergyBallImpact : MonoBehaviour {

	private GameObject MasterScriptObject;
	private Master M;

	private Renderer MyRendere;
	private bool DisintegrateCounterStart = false;
	private float Counter = 0f;

	// Use this for initialization
	void Start () {

		MasterScriptObject = GameObject.Find ("MasterScript");
		M = MasterScriptObject.GetComponent<Master> ();

		MyRendere = GetComponent<Renderer> ();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (DisintegrateCounterStart == true) {
			Counter += 1 * Time.deltaTime;
			if (Counter > M.PublicValuesToBeAssignedInInspector[5]){
				M.AchivementFloats [1] += 1f;
				M.FloatsToAccessInOtherScripts [4] += 1f;
				Destroy (gameObject);
			}
		}
	
	}

	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "EnergyBall"){
			MyRendere.material = M.materials [0];
			DisintegrateCounterStart = true;
		}
	}
	void OnCollisionEnter (Collision other){
		if (other.gameObject.tag == "EnergyBall"){
			MyRendere.material = M.materials [0];
			DisintegrateCounterStart = true;
		}
	}
}
