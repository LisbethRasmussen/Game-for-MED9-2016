using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MobSpawner : NetworkBehaviour {

	private GameObject MasterScriptObject;
	private Master M;

	private bool ImActiveNow = false;
	private float Counter = 0;
	private float TimeForNextSpawn = 0f;

	public static float AmountOfSmallMobs = 0; //Need this number to act the same no matter what spawner is active, ergo static is used here.
	public static float GetAmountOfMobs(){return AmountOfSmallMobs;}
	public static void SetAmountOfMobs (float x){AmountOfSmallMobs = x;}

	public static bool SpawnSmallMob = false; //using the static here since there is no need to go through the master script, and I don't want things to interfer with each other
	public static bool GetSpawnSmallMob () { return SpawnSmallMob;}
	public static void SetSpawnSmallMob (bool x){SpawnSmallMob = x;}

	// Use this for initialization
	void Start () {

		MasterScriptObject = GameObject.Find ("MasterScript");
		M = MasterScriptObject.GetComponent<Master> ();

		TimeForNextSpawn = Random.Range (1, M.PublicValuesToBeAssignedInInspector [6]);
	}

	// Update is called once per frame
	void Update () {

		if (ImActiveNow) {
			if (AmountOfSmallMobs < M.PublicValuesToBeAssignedInInspector [7]) {
				Counter += 1 * Time.deltaTime;
				if (Counter >= TimeForNextSpawn) {
					M.Positions3D [1] = transform.position; //Sending the spawn box's position to the main sripct, which will tell the robot script where to spawn the small mobs.
					SpawnSmallMob = true;
					//var SmallMobi = (GameObject)Instantiate (M.GameobjectObjects [5], transform.position, Quaternion.identity);
					/*if (M.MyBooleans [2]) {
						NetworkServer.Spawn (SmallMobi);
					}*/
					Counter = 0; //Resetting our counter for how long the spawn time is.
					TimeForNextSpawn = Random.Range (1, M.PublicValuesToBeAssignedInInspector [6]); //Finding a new spawn time
				}
			}
			if ((M.MyBooleans[2] && M.MyBooleans [7] && M.MyBooleans [8]) || (!M.MyBooleans[2] && M.MyBooleans[7])){
				ImActiveNow = false; //If both players are dead in the multiplayer version, it will stop spawning mobs, and if player is dead in single version, it will stop too.
			}
		}
	}

	void OnTriggerEnter (Collider other){ //If the robot trigger sphere triggers the spawner, then it will function
		if (other.gameObject.tag == "RobotComingThrough") {
			ImActiveNow = true;
		}
	}
	void OnTriggerExit (Collider other){ // If the robot sphere trigger exits the mob spawner trigger, then it will stop working
		if (other.gameObject.tag == "RobotComingThrough") {
			ImActiveNow = false;
		}
	}
}
