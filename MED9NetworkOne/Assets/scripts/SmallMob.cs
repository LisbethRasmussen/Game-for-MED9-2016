using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SmallMob : NetworkBehaviour {

	private float HowManyMobsAreThereNow = 0f;

	private GameObject MasterScriptObject;
	private Master M;

	private GameObject RobotHost;
	private RobotScript HosterScript;
	private float DistanceFromServer = 0f;

	private GameObject RobotClient;
	private RobotScript LocallyScript;
	private float DistanceFromClient = 0f;

	private Animator Anim;

	public GameObject AvatarWithMaterial;
	private Renderer MyRendere;

	private bool DisintegrateCounterStart = false;
	private float Counter = 0f;
	private Vector3 WaitingToDie;

	private bool[] Scenes = new bool[3];

	void Start () {

		HowManyMobsAreThereNow = MobSpawner.GetAmountOfMobs ();
		MobSpawner.SetAmountOfMobs (HowManyMobsAreThereNow + 1);

		MasterScriptObject = GameObject.Find ("MasterScript");
		M = MasterScriptObject.GetComponent<Master> ();

		RobotHost = GameObject.Find ("Hostering");
		HosterScript = RobotHost.GetComponent<RobotScript> ();
		if (M.MyBooleans [2]) {
			RobotClient = GameObject.Find ("Locally");
			LocallyScript = RobotClient.GetComponent<RobotScript> ();
		}

		Anim = GetComponent<Animator> ();

		MyRendere = AvatarWithMaterial.GetComponent<Renderer> ();

		for (int i = 0; i < Scenes.Length; i++) {
			Scenes [i] = false;
		}
	
	}

	void Update () {

		for (int i = 0; i < Scenes.Length; i++) {
			if (M.MyBooleans [i]) {
				Scenes [i] = M.MyBooleans [i];
			}
			if ((Scenes[0] && Scenes[1]) || (Scenes[0] && Scenes[2]) || (Scenes[1] && Scenes[2])){
				Destroy(gameObject);
			}
		}

		if (M.MyBooleans [2]) { //move towards closest bot if we are in the relatedness scenario, where there are two bots
			DistanceFromServer = Vector3.Distance (transform.position, RobotHost.transform.position);
			DistanceFromClient = Vector3.Distance (transform.position, RobotClient.transform.position);

			if (DistanceFromServer < DistanceFromClient) {
				transform.LookAt (RobotHost.transform.position);
			}
			if (DistanceFromServer > DistanceFromClient) {
				transform.LookAt (RobotClient.transform.position);
			}
		}
		else { //if we are not in the relatedness scenario, we only have one bot to attack
			transform.LookAt (RobotHost.transform.position);
		}
			
		transform.localPosition += transform.forward * M.PublicValuesToBeAssignedInInspector[8]*Time.deltaTime; //remember to change the speed of the animation too.

		//Mob needs to die when hurt or die when the player(s) are dead
		if (DisintegrateCounterStart == true || (M.MyBooleans[2] && M.MyBooleans[7] && M.MyBooleans[8]) || (!M.MyBooleans[2] && M.MyBooleans[7])) {
			Counter += 1 * Time.deltaTime;
			transform.position = WaitingToDie;
			if (Counter > M.PublicValuesToBeAssignedInInspector[5]){
				HowManyMobsAreThereNow = MobSpawner.GetAmountOfMobs ();
				MobSpawner.SetAmountOfMobs (HowManyMobsAreThereNow - 1);
				Destroy (gameObject);
			}
		}
	}

	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "EnergyBall"){
			MyRendere.material = M.materials [0];
			if (Anim.enabled) {
				M.AchivementFloats [2] += 1f;
				M.FloatsToAccessInOtherScripts [4] += 6f;
			}
			Anim.enabled = false;
			WaitingToDie = transform.position;
			DisintegrateCounterStart = true;
		}

		if (other.gameObject.tag == "Hostering") {
			HosterScript.ShieldHealthSlider.value -= M.PublicValuesToBeAssignedInInspector [9];
			MyRendere.material = M.materials [0];
			Anim.enabled = false;
			WaitingToDie = transform.position;
			DisintegrateCounterStart = true;
		}
		if (other.gameObject.tag == "Locally") {
			LocallyScript.ShieldHealthSlider.value -= M.PublicValuesToBeAssignedInInspector [9];
			MyRendere.material = M.materials [0];
			Anim.enabled = false;
			WaitingToDie = transform.position;
			DisintegrateCounterStart = true;
		}
		if (other.gameObject.name == "Hostering") {
			HosterScript.InitiateDeaht = true;
		}
		if (other.gameObject.name == "Locally") {
			LocallyScript.InitiateDeaht = true;
		}
	}
}
