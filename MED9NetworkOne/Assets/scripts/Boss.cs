using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Boss : NetworkBehaviour {

	private GameObject MasterScriptObject;
	private Master M;

	private GameObject BotHost;
	private GameObject BotClient;

	private float DistanceBotHost = 0f;
	private float DistanceBotClient = 0f;

	private RobotScript HosterScript;
	private RobotScript ClientScript;

    private float BossHeight = 0f;

	//Obviously we need the real boss for these to work
	/*private Animator anim;
	public GameObject AvatarWithMaterial; //Because we want to create an effect when boss is hit
	private Renderer MyRender;*/

	private float NormalSpeed = 0f;
	private float FasterSpeed = 0f;
	private float UsedSpeed = 0f;

	private float FleeWhenHealthIsBelowTwoThirds = 0f;
	private float FleeWhenHealthIsBelowOneThird = 0f;
	private bool NotFleeing = true;
	private bool FleeingFirstTime = false;
	private bool FleeingSecondTime = false;
	private bool ReachedFirstDestination = false;
	private bool ReachedSecondDestination = false;

	void Start () {

		MasterScriptObject = GameObject.Find ("MasterScript");
		M = MasterScriptObject.GetComponent<Master> ();

		BotHost = GameObject.Find ("Hostering");
		HosterScript = BotHost.GetComponent<RobotScript> ();
		if (M.MyBooleans [2]) {
			BotClient = GameObject.Find("Locally");
			ClientScript = BotClient.GetComponent<RobotScript> ();
		}

		//Obviously we need the real boss for these to work
		/*anim = GetComponent<Animator>();
		MyRender = AvatarWithMaterial.GetComponent<Renderer>();*/

		NormalSpeed = M.PublicValuesToBeAssignedInInspector[20];
		FasterSpeed = M.PublicValuesToBeAssignedInInspector[21];
		UsedSpeed = NormalSpeed;

		FleeWhenHealthIsBelowTwoThirds = (M.PublicValuesToBeAssignedInInspector [14] *0.33f) * 2f;
		FleeWhenHealthIsBelowOneThird = M.PublicValuesToBeAssignedInInspector [14] * 0.33f;



	}

	void Update () {

		if (!M.MyBooleans [2] && BotHost == null) {
			BotHost = GameObject.Find ("Hostering");
		}
		if (M.MyBooleans [2]) {
			if (BotHost == null) {
				BotHost = GameObject.Find ("Hostering");
			}
			if (BotClient == null) {
				BotClient = GameObject.Find("Locally");
			}
		}

		if (NotFleeing) {
			if (!M.MyBooleans [2] && !M.MyBooleans [7]) {
				transform.LookAt (BotHost.transform.position);
                BossHeight = BotHost.transform.position.y + 20f;
			}
			if (!M.MyBooleans [2] && M.MyBooleans [7]) {
				transform.LookAt (M.TransformObjects [3]);
				transform.position += transform.forward * UsedSpeed * Time.deltaTime;
			}

			//Boss follows the closest Bot

			if (M.MyBooleans [2]) { //move towards closest bot if we are in the relatedness scenario, where there are two bots
				DistanceBotHost = Vector3.Distance (transform.position, BotHost.transform.position);
				DistanceBotClient = Vector3.Distance (transform.position, BotClient.transform.position);

				if ((DistanceBotHost < DistanceBotClient && !M.MyBooleans [7]) || M.MyBooleans [8]) {
					transform.LookAt (BotHost.transform.position);
                    BossHeight = BotHost.transform.position.y + 20f;
                }
				if ((DistanceBotHost > DistanceBotClient && !M.MyBooleans [8]) || M.MyBooleans [7]) {
					transform.LookAt (BotClient.transform.position);
                    BossHeight = BotClient.transform.position.y + 20f;
                }
			}
			if (!M.MyBooleans [2] && !M.MyBooleans [7]) {
				transform.position += transform.forward * UsedSpeed * Time.deltaTime;
                transform.position = new Vector3(transform.position.x,BossHeight,transform.position.z);
			}

			if (FleeWhenHealthIsBelowTwoThirds >= M.PublicValuesToBeAssignedInInspector [14] && !FleeingFirstTime) {
				FleeingFirstTime = true;
				NotFleeing = false;
			}
			if (FleeWhenHealthIsBelowOneThird >= M.PublicValuesToBeAssignedInInspector [14] && !FleeingSecondTime) {
				FleeingSecondTime = true;
				NotFleeing = false;
			}

		}
		if (!NotFleeing) {

			if (FleeingFirstTime && !ReachedFirstDestination) {
				transform.LookAt (M.TransformObjects [8]);
				transform.position += transform.forward * UsedSpeed * Time.deltaTime;
				if (Vector3.Distance (transform.position,M.TransformObjects [8].transform.position) <= 7){
					ReachedFirstDestination = true;
					NotFleeing = true;
				}
			}
			if (FleeingSecondTime && !ReachedSecondDestination) {
				transform.LookAt (M.TransformObjects [9]);
				transform.position += transform.forward * UsedSpeed * Time.deltaTime;
				if (Vector3.Distance (transform.position, M.TransformObjects [9].transform.position) <= 7) {
					ReachedSecondDestination = true;
					NotFleeing = true;
				}
			}
		}
	}
	//Need to figure out if I am gonna use triggers or collisions for this with the spawned energy balls in mind.
	void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "Hostering") {
			HosterScript.ShieldHealthSlider.value = 0f;
			HosterScript.InitiateDeaht = true;
		}
		if (other.gameObject.name == "Locally") {
			ClientScript.ShieldHealthSlider.value = 0f;
			ClientScript.InitiateDeaht = true;
		}
	}
	void OnTriggerStay(Collider other){
		if (other.gameObject.tag == "Fast") { 
			UsedSpeed = NormalSpeed;
		}
		else {
			UsedSpeed = FasterSpeed;
		}
	}
}
