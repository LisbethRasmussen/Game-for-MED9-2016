using UnityEngine;
using System.Collections;

public class CollectableObject : MonoBehaviour {

	private bool JustSpawned = true;
	private float[] MyForce = new float[3]; //x,y,z
	private float MyForceReduction = 0f;

	private bool RunOnce = true;

	private GameObject MasterScriptObject;
	private Master M;

	private GameObject RobotHost;
	private RobotScript HosterScript; //debug

	private GameObject RobotClient;
	private RobotScript LocallyScript; //debug

	// Use this for initialization
	void Start () {

		MasterScriptObject = GameObject.Find ("MasterScript");
		M = MasterScriptObject.GetComponent<Master> ();

		RobotHost = GameObject.Find ("Hostering");
		HosterScript = RobotHost.GetComponent<RobotScript> (); //debug

		if (M.MyBooleans [2]) {
			RobotClient = GameObject.Find ("Locally");
			LocallyScript = RobotClient.GetComponent<RobotScript> (); //debug
		}

		for (int i = 0; i < MyForce.Length; i++) {
			MyForce [i] = Random.Range (0.1f, 0.3f);
		}
		MyForceReduction = MyForce [1] * 0.1f;
	}
	
	// Update is called once per frame
	void Update () {

		if (JustSpawned) {
			transform.position += new Vector3 (MyForce [0] * Time.deltaTime, (MyForce [1] - MyForceReduction) * Time.deltaTime, MyForce [2] * Time.deltaTime);
		}

		transform.Rotate(1f,1f,1f);
	
	}
	void OnCollisionEnter (Collision other){
		if (other.gameObject.tag == "AllPurposeTag" && JustSpawned) {
			JustSpawned = false;
		}

		if (other.gameObject.name == "Hostering") {
			if (RunOnce) {
				M.AchivementFloats [3] += 1f;
				//We need to know whether or not the shield is active at all, and whether or not it is already at its maximum value.
				if (HosterScript.MyShield.activeSelf && HosterScript.ShieldHealthSlider.value != M.PublicValuesToBeAssignedInInspector [3]) {
					HosterScript.ShieldHealthSlider.value += M.PublicValuesToBeAssignedInInspector [19];
				}
				if (!HosterScript.MyShield.activeSelf) {
					HosterScript.MyShield.SetActive (true);
					HosterScript.ShieldHealthSlider.value += M.PublicValuesToBeAssignedInInspector [19];
				}
				RunOnce = false;
			}
			Destroy (gameObject);
		}

		if (other.gameObject.name == "Locally") {
			if (RunOnce) {
				M.AchivementFloats [3] += 1f;
				if (LocallyScript.MyShield.activeSelf && LocallyScript.ShieldHealthSlider.value != M.PublicValuesToBeAssignedInInspector [3]) {
					LocallyScript.ShieldHealthSlider.value += M.PublicValuesToBeAssignedInInspector [19];
				}
				if (!LocallyScript.MyShield.activeSelf) {
					LocallyScript.MyShield.SetActive (true);
					LocallyScript.ShieldHealthSlider.value += M.PublicValuesToBeAssignedInInspector [19];
				}
				RunOnce = false;
			}
			Destroy (gameObject);
		}

	}
}
