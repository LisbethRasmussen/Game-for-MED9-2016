using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager {

	// Use this for initialization
	void Start () {
		SetupSceneButtons ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartupHost(){
		SetPort ();
		string ipAddress = GameObject.Find ("InputFieldIPAddress").transform.FindChild ("Text").GetComponent<Text> ().text;
		//NetworkManager.singleton.networkAddress = ipAddress;
		NetworkManager.singleton.serverBindToIP = true;
		NetworkManager.singleton.serverBindAddress = ipAddress;
		print ("StartupHost 1 Singleton = " + NetworkManager.singleton.networkAddress);
		NetworkManager.singleton.StartHost ();
		print ("StartupHost 2 Singleton = " + NetworkManager.singleton.networkAddress);
	}

	public void JoinGame(){
		string ipAddress = GameObject.Find ("InputFieldIPAddress").transform.FindChild ("Text").GetComponent<Text> ().text;
		NetworkManager.singleton.networkAddress = ipAddress;
		print ("Join 1 Singleton = " + NetworkManager.singleton.networkAddress);
		NetworkManager.singleton.StartClient ();
		print ("Join 2 Singleton = " + NetworkManager.singleton.networkAddress);
	}

	/*void SetIPAddress(){
		string ipAddress = GameObject.Find ("InputFieldIPAddress").transform.FindChild ("Text").GetComponent<Text> ().text;
		NetworkManager.singleton.networkAddress = ipAddress;
		print ("Set ip Singleton = " + NetworkManager.singleton.networkAddress);
	}*/

	void SetPort(){
		NetworkManager.singleton.networkPort = 7777;
	}

	void SetupSceneButtons(){
		GameObject.Find ("ButtonStartHost").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonStartHost").GetComponent<Button> ().onClick.AddListener (StartupHost);

		GameObject.Find ("ButtonStartClient").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonStartClient").GetComponent<Button> ().onClick.AddListener (JoinGame);
	
		//RactivateButtons ();

		print (" SetupScenebuttons Singleton = " + NetworkManager.singleton.networkAddress);
	}

	void RactivateButtons(){
		GameObject.Find ("ButtonStartHost").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonStartHost").GetComponent<Button> ().onClick.AddListener (StartupHost);

		GameObject.Find ("ButtonStartClient").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonStartClient").GetComponent<Button> ().onClick.AddListener (JoinGame);
	
		SetupSceneButtons ();

		print ("Ractivate Singleton = " + NetworkManager.singleton.networkAddress);
	}
}
