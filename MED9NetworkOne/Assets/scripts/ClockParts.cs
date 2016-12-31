using UnityEngine;
using System.Collections;

public class ClockParts : MonoBehaviour {

	private float DistanceFromRobot = 0f;
	private GameObject PlayerRobot;

	private GameObject MasterScriptObject;
	private Master M;

	private bool RunOnce = true;

	// Use this for initialization
	void Start () {

		PlayerRobot = GameObject.Find ("Robot2");

		MasterScriptObject = GameObject.Find ("MasterScript");
		M = MasterScriptObject.GetComponent<Master> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (PlayerRobot == null) {
			PlayerRobot = GameObject.Find ("Robot2");
		}
			DistanceFromRobot = Vector3.Distance (transform.position, PlayerRobot.transform.position);

		if (M.MyBooleans [2]) { //This is mainly for debugging, but if a player does not succeed in picking up the required amount of object, this helps there too.
			Destroy (gameObject);
		}
	}

	void OnGUI(){
		if (DistanceFromRobot <= 2f) {
			GUI.Box (new Rect (Screen.width / 2f - 50f, Screen.height / 2f - 25f, 100f, 50f), "Press E to\n pick up part");
			if (Input.GetKeyDown (KeyCode.E)) {
				if (name == "ClockPart1" && RunOnce) {
					M.FloatsToAccessInOtherScripts [0] += 1f;
					RunOnce = false;
				}
				if (name == "ClockPart2" && RunOnce) {
					M.FloatsToAccessInOtherScripts [1] += 1f;
					RunOnce = false;
				}
				if (name == "ClockPart3" && RunOnce) {
					M.FloatsToAccessInOtherScripts [2] += 1f;
					RunOnce = false;
				}
				Destroy (gameObject);
			}
		}
	}
}
