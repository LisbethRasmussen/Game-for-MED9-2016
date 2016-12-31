using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FireballBehaviour : NetworkBehaviour {

	private GameObject MasterScriptObject;
	private Master M;

	private GameObject BotHost;
	private GameObject BotClient;

	private float DistanceBotHost = 0f;
	private float DistanceBotClient = 0f;

	// Use this for initialization
	void Start () {
	
		MasterScriptObject = GameObject.Find ("MasterScript");
		M = MasterScriptObject.GetComponent<Master> ();

		BotHost = GameObject.Find ("Hostering");
		BotClient = GameObject.Find("Locally");

		//transform.Rotate (0f, GameObject.Find("TheBoss(Clone)").transform.rotation.y+160f, -45f);

		//transform.LookAt

		DistanceBotHost = Vector3.Distance (transform.position, BotHost.transform.position);
		DistanceBotClient = Vector3.Distance (transform.position, BotClient.transform.position);

		if (DistanceBotHost < DistanceBotClient) {
			transform.LookAt (BotHost.transform.position);
		}
		if (DistanceBotHost > DistanceBotClient) {
			transform.LookAt (BotClient.transform.position);
		}

	}
	
	// Update is called once per frame
	void Update () {

		//transform.localPosition += new Vector3 ((M.PublicValuesToBeAssignedInInspector [22] * Time.deltaTime)*-1f, (M.PublicValuesToBeAssignedInInspector [22] * Time.deltaTime) * -1f, 0f);
	
		transform.position += transform.forward * M.PublicValuesToBeAssignedInInspector [22] * Time.deltaTime;

	}

	void OnCollisionEnter (Collision other){
		
		Instantiate (M.TransformObjects [1], transform.position, Quaternion.identity);
		Destroy (gameObject);

	}
}
