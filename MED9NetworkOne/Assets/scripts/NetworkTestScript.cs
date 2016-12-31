using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkTestScript : NetworkBehaviour {

	private bool RunnedOnce = false;
	private GameObject NetMan;
	private NetworkManagerHUD NMH;

	private bool RunOnce = true;

	// Use this for initialization
	void Start () {

		NetMan = GameObject.Find("NETWORKmanager");
		NMH = NetMan.GetComponent<NetworkManagerHUD> ();
	
	}
	
	// Update is called once per frame
	void Update () {

		NetworkManager.singleton.networkAddress = "192.168.2.15";

		if (GameObject.Find ("Hostering") && RunOnce) {
			NMH.showGUI = false;
			RunOnce = false;
		}
		if (Input.GetKeyDown (KeyCode.H)) { //debug
			NMH.showGUI = true;
		}
		if (Input.GetKeyDown (KeyCode.J)) { //debug
			NMH.showGUI = true;
		}

	}
}
