using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour {

	public GameObject ClosedCrate;
	public GameObject OpenCrate;
	private GameObject EnergyRechargeCollectable;
	private bool SetThisValueOnce = true;
	private int AmountOfObjectsSpawned = 0;

	private Vector3 EnergyRechargeSpawnPosition;

	private GameObject MasterScriptObject;
	private Master M;

	private Transform Explosion;
	private bool AlreadyOpen = false;

	private bool StartCounting = false;
	private float SaftyCounter = 0f;

	void Start () {
		MasterScriptObject = GameObject.Find ("MasterScript");
		M = MasterScriptObject.GetComponent<Master> ();

		Explosion = M.TransformObjects [1];
		EnergyRechargeCollectable = M.GameobjectObjects [6];

		OpenCrate.SetActive (false);

		EnergyRechargeSpawnPosition = new Vector3 (transform.position.x, transform.position.y + 4f, transform.position.z);

		if (!M.MyBooleans [6]) {
			AmountOfObjectsSpawned = 3;
		}
	}

	void Update () {

		if (SetThisValueOnce && M.MyBooleans [6]) {
			AmountOfObjectsSpawned = Random.Range (1, 6);
			SetThisValueOnce = false;
		}


		//We do this to aviod that both crates are destroyed, as the robot shoots two energyballs at a time.
		if (StartCounting) {
			SaftyCounter += 1 * Time.deltaTime;
			if (SaftyCounter >= 1) {
				AlreadyOpen = true;
				StartCounting = false;
			}
		}
	
	}

	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "EnergyBall" && !AlreadyOpen && !StartCounting){
			Instantiate (Explosion, transform.position, Quaternion.identity);
			for (int i = 0; i < AmountOfObjectsSpawned; i++) {
				Instantiate (EnergyRechargeCollectable, EnergyRechargeSpawnPosition, Quaternion.identity);
			}
			ClosedCrate.SetActive (false);
			OpenCrate.SetActive (true);
			StartCounting = true;
		}
		if (other.gameObject.tag == "EnergyBall" && AlreadyOpen) {
			Instantiate (Explosion, transform.position, Quaternion.identity);
			Destroy (gameObject);
		}
	}
}
