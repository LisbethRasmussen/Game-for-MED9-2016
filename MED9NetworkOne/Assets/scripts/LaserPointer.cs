using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//Note to future self: Raycast does not ignore triggers unless you go to edit->project settings->physics and tick off "Queries Hit Triggers"

public class LaserPointer : NetworkBehaviour {

	private GameObject MasterScriptObject; //two lines for accessing public variables in the Master script placed on the MasterScript object
	private Master M;

	private GameObject LaserInstance;
	private float OffSet = 0.1f;
	private LineRenderer MyLine;
	private float LenghtOfLaserRay = 0f;

	private Vector3 MyOriginalRotation;
	private Vector3 MyOriginalLocalPosition;

	public GameObject TheRobotPointIamAttacthedTo;
	public Camera BotCam;

	private bool IsRecharging = false;
	private float DiminishFactorInPercent = 0f;
	private float BoxMaxHeight = 0f;
	private float BoxHeight = 0f;

	// Use this for initialization
	void Start () {

		MasterScriptObject = GameObject.Find ("MasterScript"); //Looking for the original MasterScript object
		M = MasterScriptObject.GetComponent<Master> ();			//Retrieving the Master script component

		MyLine = GetComponent<LineRenderer> ();

		MyOriginalRotation = transform.localEulerAngles;
		MyOriginalLocalPosition = transform.localPosition;
			
		DiminishFactorInPercent = M.PublicValuesToBeAssignedInInspector [10];
		BoxMaxHeight = Screen.height / 2.5f;
		BoxHeight = BoxMaxHeight;

		MyLine.SetVertexCount (2); //works like an array, meaning that the first number needs to be zeron.
		MyLine.enabled = false;
	}

	void Update () {

		//If we are not paused, we can aim and shoot
		if (Time.timeScale == 1 && !Input.GetKey(KeyCode.L)) {

			//Use left mouse button pressed down to use the laser dot
			if (Input.GetMouseButton (0)) {
				RaycastHit Hit;
				Ray MyRay = BotCam.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (/*transform.position, */MyRay, out Hit)) {
					if (LaserInstance == null) {
						LaserInstance = Instantiate (M.GameobjectObjects [2], Hit.point + Hit.normal * OffSet, Quaternion.identity)as GameObject;
					}
					else {
						LaserInstance.transform.position = Hit.point + Hit.normal * OffSet;
					}
				}

				MyLine.enabled = true;

				MyLine.SetPosition (0, TheRobotPointIamAttacthedTo.transform.position);
				MyLine.SetPosition (1, LaserInstance.transform.position);

				//transform.Rotate (Input.GetAxis ("Mouse Y") * M.PublicValuesToBeAssignedInInspector [2] * Time.deltaTime * -1, Input.GetAxis ("Mouse X") * M.PublicValuesToBeAssignedInInspector [2] * Time.deltaTime, 0f);
			}
			//If releasing the mousebutton, the robot will shoot, and the dot will disappear.
			if (Input.GetMouseButtonUp (0)) {
				/*if (M.MyBooleans [2]) {
					CmdShootOverThere ();
				}
				else {
					M.Positions3D[2] = LaserInstance.transform.position;
				}*/
				if (!Input.GetKey (KeyCode.E)) {
					IsRecharging = true;
				}
				M.Positions3D[2] = LaserInstance.transform.position;
				transform.localEulerAngles = MyOriginalRotation;
				transform.localPosition = MyOriginalLocalPosition;
				MyLine.enabled = false;
				Destroy (LaserInstance);
			}
		}
	}

	/*[Command]
	void CmdShootOverThere(){
		M.Positions3D[2] = LaserInstance.transform.position;
	}*/

	void OnGUI(){
		if (IsRecharging) {
			/*
			Explanation for the following equation:
			We want the bar to get diminished by a certain percentage each second. We do this by setting the percentage value in the inspector e.g. "2".
			As the number 2 is not in itself a percentage, we need to divide it by 100 (2/100), in order to use it for the calculation correctley.

			As computers do not always share their height, we need to make sure that the screenheight (and width) is always accounted for.
			Therefore we set the GUI.Box's Rectangle values to be a specific percentage of the computer screen which the game is being played on (e.g. screen.height/20).

			Therefore we also need the screen.height in the equation below. As the box height is e.g. 30% of the screen.height, we do not want the box to loose 2% of
			its size each second, but 2% of the screen.height each second. In this way we always ensure that the time it will take for the box to reach a height of 0
			will always be the same.

			We end the equation by multiplying with Time.deltatime in order to make sure that the box will diminish with the assigned percentage each second, and not
			each frame, which may vary from computer to computer.
			*/
			BoxHeight -= (Screen.height*(DiminishFactorInPercent/100))*Time.deltaTime;
			//2% of screen width  				10% of screen height				Box width 3%
			GUI.Box (new Rect (Screen.width*0.02f, Screen.height - (Screen.height*0.1f)-BoxHeight, Screen.width*0.03f, BoxHeight),
				"R\n" +
				"E\n" +
				"C\n" +
				"H\n" +
				"A\n" +
				"R\n" +
				"G\n" +
				"I\n" +
				"N\n" +
				"G\n");
			if (BoxHeight <= 0) {
				BoxHeight = BoxMaxHeight;
				IsRecharging = false;
			}
		}
	}
}
