using UnityEngine;
using System.Collections;

public class BridgeBehaviour : MonoBehaviour {

	//Note, after checking it out, the bridge parts does move along the x axis. But I will keep my comments about changing to the z axis.

	private GameObject MasterScriptObject; //two lines for accessing public variables in the Master script placed on the MasterScript object
	private Master M;

	private float BridgeWidth = 0f;
	private float MoveSpeed = 0f;

	private float MaximumPositionX = 0f; //This may need to be changed to z, dependent on how the object will be turned.
	private float MinimumPositionX = 0f; //This may need to be changed to z, dependent on how the object will be turned.

	private Vector3 MyOriginalPosition;

	// Use this for initialization
	void Start () {

		MyOriginalPosition = transform.position;

		MasterScriptObject = GameObject.Find ("MasterScript"); //Looking for the original MasterScript object
		M = MasterScriptObject.GetComponent<Master> ();			//Retrieving the Master script component

		BridgeWidth = M.PublicValuesToBeAssignedInInspector[0];
		MoveSpeed = M.PublicValuesToBeAssignedInInspector [1];

		if (gameObject.name == "R"){ //Remember to either rename the objects, or rename these names.
			transform.localPosition += new Vector3 ((BridgeWidth / 2f),0f,0f);
			MaximumPositionX = transform.localPosition.x; //May be changed to Z
			MinimumPositionX = MaximumPositionX - BridgeWidth;
		}
		if (gameObject.name == "L") {
			transform.localPosition -= new Vector3 ((BridgeWidth / 2f),0f,0f);
			MinimumPositionX = transform.localPosition.x; //May be changed to Z
			MaximumPositionX = MinimumPositionX + BridgeWidth;
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (M.MyBooleans [0]) {

			transform.localPosition += new Vector3 (MoveSpeed * Time.deltaTime, 0, 0);

			if (transform.localPosition.x <= MinimumPositionX) {
				MoveSpeed = M.PublicValuesToBeAssignedInInspector [1];
			}
			
			if (transform.localPosition.x >= MaximumPositionX) {
				MoveSpeed = M.PublicValuesToBeAssignedInInspector [1] * -1;
			}
		}
		else {
			transform.position = MyOriginalPosition;
			Destroy (gameObject.GetComponent<BridgeBehaviour> ());
		}
	
	}
}
