using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Master : NetworkBehaviour {
	//---------------------------------------------------------------------------------------------------------
	[Tooltip(

		"0 = Bridge Widht\n" +
		"1 = Bridge Moving Speed\n" +
		"2 = Mouse look orbit speed\n" +
		"3 = Robot Health Points\n" +
		"4 = How fast the energy balls fly\n" +
		"5 = How long it takes for any object to disintegrate\n" +
		"6 = How many seconds between default small mob spaw time\n" +
		"7 = Maximum amount of small mobs existing at the same time\n" +
		"8 = Small Mobs Running speed\n" +
		"9 = Small Mobs attack damage\n" +
		"10= How many % the rechagebar will diminish each second\n" +
		"11= Robot Jump Forward force\n" +
		"12= Robot Jump Up Force\n" +
		"13= Robot rotation speed\n" +
		"14= Boss health\n" +
		"15= How many 'clockparts' the player should collect\n" +
		"16= Holdervalue, this should be set to 0\n" +
		"17= holdervalue, this should be set to 0\n" +
		"18= How much damage the Bots should make on the Boss\n" +
		"19= How much power the shield gets from the recharger objects\n" +
		"20= Boss normal speed\n" +
		"21= Boss fast speed\n" +
		"22= Fireball speed\n" +
		"23= Fireball spawn time\n" +
		"24= Fireball damage\n" +
		"25= Distance from bots to boss needed to fire laser")]
		public float[] PublicValuesToBeAssignedInInspector;

	//---------------------------------------------------------------------------------------------------------
	[Tooltip(
		"0 = None yet\n" +
		"1 = Place explosion, from prefabs here\n" +
		"2 = Place Camera here???\n" +
		"3 = put Boss Spawn object here\n" +
		"4 = None yet\n" +
		"5 = Place an empty game object which is not used for anything else\n" +
		"6 = Place flare, or other object, which replaces a destroyed energyball\n" + //not used yet
		"7 = Place the 'Root' part of the robot or something similar here???\n" +
		"\n")]
	public Transform[] TransformObjects;

	//---------------------------------------------------------------------------------------------------------
	[Tooltip(
		"0 = Put Boss Prefab here\n" +
		"1 = Place the particles, from the scene that shows which portal are active here\n" +
		"2 = Place the Laser dot, from prefabs here\n" +
		"3 = Place EnergyBall, from prefabs here\n" +
		"4 = Place Server Player prefab here\n" +
		"5 = Place prefab of small mob here\n" +
		"6 = Place Coin Collectable object here\n" +
		"7 = Empty object (0,0,0) with all Collectables from Style 1\n" +
		"8 = Empty object (0,0,0) with all Collectables from Style 2\n" +
		"9 = Empty object (0,0,0) with all Collectables from Style 3\n" +
		"10= The UI camera for setting up networking\n" +
		"11= The empty object on which the robots will spawn initially\n" +
		"12= The object holding the network script\n" +
		"13= The prefab for the boss' fireballs\n" +
		"\n")]
		public GameObject[] GameobjectObjects;

	//---------------------------------------------------------------------------------------------------------
	[Tooltip(
		"0 = Robot spawn position\n" +
		"1 = SmallMob spawn position\n" +
		"2 = Point to shoot at")]
		public Vector3[] Positions3D;

	//---------------------------------------------------------------------------------------------------------
	[Tooltip(
		"0 = place object disintegrate texture here\n" +
		"\n")]
	public Material[] materials;

	//---------------------------------------------------------------------------------------------------------
	[Tooltip(
		"As a general advice, stay away from these booleans\n\n" +
		"0 = Scene 1\n" +
		"1 = Scene 2\n" +
		"2 = Scene 3\n" +
		"3 = GameVariation 1 - no AGM\n" +
		"4 = GameVariation 2 - Achivements\n" +
		"5 = GameVariation 3 - Level Up\n" +
		"6 = GameVariation 4 - Randomness to amount of rewards\n" +
		"7 = ServerRobot dead\n" +
		"8 = ClientRobot dead\n" +
		"9 = Respawn necessary")]
	public bool[] MyBooleans;

	//---------------------------------------------------------------------------------------------------------
	[Tooltip(
		"DO NOT TOUCH THESE! *angry programmer face*\n\n" +
		"0 = Clock part style 1 gathered\n" +
		"1 = Clock part style 2 gathered\n" +
		"2 = Clock part style 3 gathered" +
		"3 = Player Level, must start at 1!\n" +
		"4 = Players Current Exp number\n")]
	public float[] FloatsToAccessInOtherScripts;

	//---------------------------------------------------------------------------------------------------------
	[Tooltip(
		"DO NOT TOUCH THESE! *angry programmer face*\n\n" +
		"0 = The name of the object that isLocalPlayer\n" +
		"1 = Dead bot name \n")]
	public string[] NeededNamesForNetwork;

	//---------------------------------------------------------------------------------------------------------
	[Tooltip(
		"DO NOT TOUCH THESE! *angry programmer face*\n\n" +
		"0 = 'Length' the player has run\n" +
		"1 = Amount of destroyed non-enemy objects\n" +
		"2 = How many foes have been killed\n" +
		"3 = How many recharge orbs the player has collected\n")]
	public float[] AchivementFloats;

	//---------------------------------------------------------------------------------------------------------
	private bool[] LoadOnce = new bool[4];
	private bool[] ShowMessage = new bool[16];
	private GUIStyle MyStyle;

	private float TestTimeSeconds = 0f;
	private float TestTimeMinuts = 0f;

	// Use this for initialization
	void Start () {

		//-------------------------------------------------safty settings if people mess shit up in the inspector
		for (int i = 0; i < LoadOnce.Length; i++) {
			LoadOnce [i] = false;
		}
		for (int i = 0; i < FloatsToAccessInOtherScripts.Length; i++) {
			FloatsToAccessInOtherScripts [i] = 0f;
		}
		FloatsToAccessInOtherScripts [3] = 1f;
		for (int i = 0; i < MyBooleans.Length; i++) {
			MyBooleans [i] = false;
		}

		MyBooleans [0] = true; //We need scene one to be true from the start of the game
		MyBooleans [3] = true; //In order to not fuck things up, we need the no AGM mode to be loaded first

		Time.timeScale = 0; //The game starts being paused

		//This is where the robot spawn in the beginning
		//GameobjectObjects [11].transform.position = Positions3D [0];

		//This sets the networking to not work from the beginning
		GameobjectObjects [12].SetActive (false);
		GameobjectObjects [10].SetActive (false);
		//Because we need to begin with one figure
		Instantiate (GameobjectObjects [4], GameobjectObjects [11].transform.position, Quaternion.identity);
	
	
	}

	void Update () {

		//Close game

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}

		//---------------------------------------------------------------------------- Change levels
		if (Input.GetKeyDown (KeyCode.F1)) {
			MyBooleans [0] = true;
			MyBooleans [1] = false;
			MyBooleans [2] = false;
		}
		if (Input.GetKeyDown (KeyCode.F2)) {
			MyBooleans [0] = false;
			MyBooleans [1] = true;
			MyBooleans [2] = false;
		}
		if (Input.GetKeyDown (KeyCode.F3)) {
			MyBooleans [0] = false;
			MyBooleans [1] = false;
			MyBooleans [2] = true;
		}
		//----------------------------------------------------------------------------
		//----------------------------------------------------------------------------Pause game by pressing P, notice that the update functions will still run
		if (Input.GetKeyDown (KeyCode.P)) {
			if (Time.timeScale == 1) {
				Time.timeScale = 0;
			}
			else {
				Time.timeScale = 1;
			}
		}
		//----------------------------------------------------------------------------Where to place the particles that shows which respawn portal is active
		GameobjectObjects[1].transform.position = Positions3D[0]; //particles on player spawn platform

		//----------------------------------------------------------------------------The different things that needs to be loaded or destroyed upon changing "levels"
		//Scenario Competence
		if (MyBooleans [0] && !LoadOnce [0]) {
			//---------------------------Reset Time
			TestTimeSeconds = 0f;
			TestTimeMinuts = 0f;
			//---------------------------Ending this
			LoadOnce [0] = true;
		}
		//Scenario Autonomy
		if (MyBooleans [1] && !LoadOnce[1]) {
			//---------------------------Reset Time
			TestTimeSeconds = 0f;
			TestTimeMinuts = 0f;
			//---------------------------Put in the collectable empty object, with the collectable objects, and naming them for other use.
			GameObject ClockStyle_One = (GameObject) Instantiate (GameobjectObjects[7],transform.position,Quaternion.identity);
			ClockStyle_One.name = "CS0";
			GameObject ClockStyle_Two = (GameObject) Instantiate (GameobjectObjects[8],transform.position,Quaternion.identity);
			ClockStyle_Two.name = "CS1";
			GameObject ClockStyle_Three = (GameObject) Instantiate (GameobjectObjects[9],transform.position,Quaternion.identity);
			ClockStyle_Three.name = "CS2";
			//---------------------------We don't want the boss to be present in this level.
			Destroy (GameObject.Find ("TheBoss(Clone)"));
			Destroy (GameObject.Find ("BossHealth"));
			//---------------------------Ending this
			LoadOnce[1] = true;
		}
		//Scenario Relatedness
		if (MyBooleans [2] && !LoadOnce [2]) {
			Destroy (GameObject.Find ("Hostering"));
			//Safty destroy
			if(GameObject.Find ("CS0") != null){
				Destroy (GameObject.Find ("CS0"));
			}
			if(GameObject.Find ("CS1") != null){
				Destroy (GameObject.Find ("CS1"));
			}
			if(GameObject.Find ("CS2") != null){
				Destroy (GameObject.Find ("CS2"));
			}
			GameobjectObjects [12].SetActive (true);
			GameobjectObjects [10].SetActive (true);
			//---------------------------Ending this
			LoadOnce [2] = true;
		}
	
	}

	void OnGUI(){


		MyStyle = new GUIStyle(GUI.skin.box);
		MyStyle.alignment = TextAnchor.MiddleCenter;
		MyStyle.normal.textColor = Color.green;

		if (MyBooleans [1]) {
			//---------------------------Letting the player know what they have gathered and what they are missing yet.
			GUI.Box (new Rect (0f,0f, Screen.width*0.2f,Screen.height*0.1f),
				"Realistic area 1: " + FloatsToAccessInOtherScripts[0] + "/" + PublicValuesToBeAssignedInInspector[15] +"\n" +
				"Cartoonish area 2: " + FloatsToAccessInOtherScripts[1] + "/" + PublicValuesToBeAssignedInInspector[15] +"\n" +
				"Cubic area 3: " + FloatsToAccessInOtherScripts[2] + "/" + PublicValuesToBeAssignedInInspector[15] +"\n");
			//---------------------------When they have gathered enough of the objects, the parent holding all the rest of the objects will destroy it self.
			if (FloatsToAccessInOtherScripts[0] == PublicValuesToBeAssignedInInspector[15]){
				Destroy (GameObject.Find ("CS0"));
			}
			if (FloatsToAccessInOtherScripts[1] == PublicValuesToBeAssignedInInspector[15]){
				Destroy (GameObject.Find ("CS1"));
			}
			if (FloatsToAccessInOtherScripts[2] == PublicValuesToBeAssignedInInspector[15]){
				Destroy (GameObject.Find ("CS2"));
			}
		}
		//---------------------------Setting the game version with gui buttons
		if (!LoadOnce [3]) {
			if (GUI.Button (new Rect (0,0, Screen.width*0.25f, Screen.height), "No AGM")) {
				LoadOnce [3] = true;
			}
			if (GUI.Button (new Rect (Screen.width*0.25f, 0f, Screen.width*0.25f, Screen.height), "Achivements")) {
				MyBooleans [4] = true;
				LoadOnce [3] = true;
			}
			if (GUI.Button (new Rect (Screen.width*0.25f*2f, 0f, Screen.width*0.25f, Screen.height), "Level Up")) {
				MyBooleans [5] = true;
				LoadOnce [3] = true;
			}
			if (GUI.Button (new Rect (Screen.width*0.25f*3f, 0f, Screen.width*0.25f, Screen.height), "Randomness to rewards")) {
				MyBooleans [6] = true;
				LoadOnce [3] = true;
			}
		}
		//---------------------------Pause screen

		if (Time.timeScale == 0) {
			GUI.Box (new Rect (0f, 0f, Screen.width, Screen.height),
				"Game is paused");
		}
		//--------------------------- Achivements

		if(MyBooleans[4] && Input.GetKeyDown(KeyCode.L)){
			for (int i = 0; i < ShowMessage.Length; i++) {
				ShowMessage [i] = false;
			}
		}

		if (MyBooleans [4] && Input.GetKey (KeyCode.L)) {

			//---------------------------unearned achivement buttons
			//-------------------------------------------------------------------------------First Row
			if (AchivementFloats [0] < 5) {
				if (GUI.Button (new Rect (Screen.width * 0.14f, Screen.height * 0.25f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "First steps")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [0] = true;
				}
				if (ShowMessage [0]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Move 5 units\n Units moved: " + AchivementFloats [0])) {
						ShowMessage [0] = false;
					}
				}
			}
			if (AchivementFloats [1] < 1) {
				if (GUI.Button (new Rect (Screen.width * 0.33f, Screen.height * 0.25f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Irrelevant destruction")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [1] = true;
				}
				if (ShowMessage [1]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Destroy 1 (total)\n irrelevant object\n Destroyed: " + AchivementFloats [1])) {
						ShowMessage [1] = false;
					}
				}
			}
			if (AchivementFloats [2] < 1) {
				if (GUI.Button (new Rect (Screen.width * 0.52f, Screen.height * 0.25f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Killing first foe")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [2] = true;
				}
				if (ShowMessage [2]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Foes killed: " + AchivementFloats [2])) {
						ShowMessage [2] = false;
					}
				}
			}
			if (AchivementFloats [3] < 3) {
				if (GUI.Button (new Rect (Screen.width * 0.71f, Screen.height * 0.25f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Small Recharge")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [3] = true;
				}
				if (ShowMessage [3]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Collect 3 (total)\n recharge objects\n Collected: " + AchivementFloats [3])) {
						ShowMessage [3] = false;
					}
				}
			}
			//-------------------------------------------------------------------------------Second Row
			if (AchivementFloats [0] < 15) {
				if (GUI.Button (new Rect (Screen.width * 0.14f, Screen.height * 0.44f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "The running game")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [4] = true;
				}
				if (ShowMessage [4]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Move 15 units\n Units moved: " + AchivementFloats [0])) {
						ShowMessage [4] = false;
					}
				}
			}
			if (AchivementFloats [1] < 3) {
				if (GUI.Button (new Rect (Screen.width * 0.33f, Screen.height * 0.44f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Destroy for the lolz")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [5] = true;
				}
				if (ShowMessage [5]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Destroy 3 (total)\n irrelevant object\n Destroyed: " + AchivementFloats [1])) {
						ShowMessage [5] = false;
					}
				}
			}
			if (AchivementFloats [2] < 5) {
				if (GUI.Button (new Rect (Screen.width * 0.52f, Screen.height * 0.44f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Killing 5 foes")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [6] = true;
				}
				if (ShowMessage [6]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Foes killed: " + AchivementFloats [2])) {
						ShowMessage [6] = false;
					}
				}
			}
			if (AchivementFloats [3] < 6) {
				if (GUI.Button (new Rect (Screen.width * 0.71f, Screen.height * 0.44f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Medium Recharger")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [7] = true;
				}
				if (ShowMessage [7]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Collect 6 (total)\n recharge objects\n Collected: " + AchivementFloats [3])) {
						ShowMessage [7] = false;
					}
				}
			}
			//-------------------------------------------------------------------------------Third Row

			if (AchivementFloats [0] < 30) {
				if (GUI.Button (new Rect (Screen.width * 0.14f, Screen.height * 0.63f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Marathon")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [8] = true;
				}
				if (ShowMessage [8]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Move 30 units\n Units moved: " + AchivementFloats [0])) {
						ShowMessage [8] = false;
					}
				}
			}
			if (AchivementFloats [1] < 7) {
				if (GUI.Button (new Rect (Screen.width * 0.33f, Screen.height * 0.63f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "The incinerator")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [9] = true;
				}
				if (ShowMessage [9]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Destroy 7 (total)\n irrelevant object\n Destroyed: " + AchivementFloats [1])) {
						ShowMessage [9] = false;
					}
				}
			}
			if (AchivementFloats [2] < 10) {
				if (GUI.Button (new Rect (Screen.width * 0.52f, Screen.height * 0.63f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Killing 10 foes")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [10] = true;
				}
				if (ShowMessage [10]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Foes killed: " + AchivementFloats [2])) {
						ShowMessage [10] = false;
					}
				}
			}
			if (AchivementFloats [3] < 12) {
				if (GUI.Button (new Rect (Screen.width * 0.71f, Screen.height * 0.63f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Recharge to maximum")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [11] = true;
				}
				if (ShowMessage [11]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Collect 12 (total)\n recharge objects\n Collected: " + AchivementFloats [3])) {
						ShowMessage [11] = false;
					}
				}
			}
			//-------------------------------------------------------------------------------Fourth Row

			if (AchivementFloats [0] < 50) {
				if (GUI.Button (new Rect (Screen.width * 0.14f, Screen.height * 0.82f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Universal runner")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [12] = true;
				}
				if (ShowMessage [12]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Move 50 units\n Units moved: " + AchivementFloats [0])) {
						ShowMessage [12] = false;
					}
				}
			}
			if (AchivementFloats [1] < 12) {
				if (GUI.Button (new Rect (Screen.width * 0.33f, Screen.height * 0.82f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Destroyer of nature")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [13] = true;
				}
				if (ShowMessage [13]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Destroy 12 (total)\n irrelevant object\n Destroyed: " + AchivementFloats [1])) {
						ShowMessage [13] = false;
					}
				}
			}
			if (AchivementFloats [2] < 15) {
				if (GUI.Button (new Rect (Screen.width * 0.52f, Screen.height * 0.82f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Exterminator")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [14] = true;
				}
				if (ShowMessage [14]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Foes killed: " + AchivementFloats [2])) {
						ShowMessage [14] = false;
					}
				}
			}
			if (AchivementFloats [3] < 24) {
				if (GUI.Button (new Rect (Screen.width * 0.71f, Screen.height * 0.82f, Screen.width * 0.15f, Screen.height * 0.15f),
					     "Recharge addict")) {
					for (int i = 0; i < ShowMessage.Length; i++) {
						ShowMessage [i] = false;
					}
					ShowMessage [15] = true;
				}
				if (ShowMessage [15]) {
					if (GUI.Button (new Rect (0, 0, Screen.width * 0.15f, Screen.height * 0.15f), "Collect 24 (total)\n recharge objects\n Collected: " + AchivementFloats [3])) {
						ShowMessage [15] = false;
					}
				}
			}
			//---------------------------End of achievement buttons

			//---------------------------Achivements finished
			//-------------------------------------------------------------------------------First Row
			//GUI.color = Color.green;
			if (AchivementFloats [0] >= 5) {
				GUI.Box (new Rect (Screen.width * 0.14f, Screen.height * 0.25f, Screen.width * 0.15f, Screen.height * 0.15f),
					"First steps", MyStyle);
			}
			if (AchivementFloats [1] >= 1) {
				GUI.Box (new Rect (Screen.width * 0.33f, Screen.height * 0.25f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Irrelevant destruction", MyStyle);
			}
			if (AchivementFloats [2] >= 1) {
				GUI.Box (new Rect (Screen.width * 0.52f, Screen.height * 0.25f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Killing first foe", MyStyle);
			}
			if (AchivementFloats [3] >= 3) {
				GUI.Box (new Rect (Screen.width * 0.71f, Screen.height * 0.25f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Small Recharge", MyStyle);
			}
			//-------------------------------------------------------------------------------Second Row
			if (AchivementFloats [0] >= 15) {
				GUI.Box (new Rect (Screen.width * 0.14f, Screen.height * 0.44f, Screen.width * 0.15f, Screen.height * 0.15f),
					"The running game", MyStyle);
			}
			if (AchivementFloats [1] >= 3) {
				GUI.Box (new Rect (Screen.width * 0.33f, Screen.height * 0.44f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Destroy for the lolz", MyStyle);
			}
			if (AchivementFloats [2] >= 5) {
				GUI.Box (new Rect (Screen.width * 0.52f, Screen.height * 0.44f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Killing 5 foes", MyStyle);
			}
			if (AchivementFloats [3] >= 6) {
				GUI.Box (new Rect (Screen.width * 0.71f, Screen.height * 0.44f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Medium Recharger", MyStyle);
			}
			//-------------------------------------------------------------------------------Third Row
			if (AchivementFloats [0] >= 30) {
				GUI.Box (new Rect (Screen.width * 0.14f, Screen.height * 0.63f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Marathon", MyStyle);
			}
			if (AchivementFloats [1] >= 7) {
				GUI.Box (new Rect (Screen.width * 0.33f, Screen.height * 0.63f, Screen.width * 0.15f, Screen.height * 0.15f),
					"The incinerator", MyStyle);
			}
			if (AchivementFloats [2] >= 10) {
				GUI.Box (new Rect (Screen.width * 0.52f, Screen.height * 0.63f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Killing 10 foes", MyStyle);
			}
			if (AchivementFloats [3] >= 12) {
				GUI.Box (new Rect (Screen.width * 0.71f, Screen.height * 0.63f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Recharge to maximum", MyStyle);
			}
			//-------------------------------------------------------------------------------Fourth Row
			if (AchivementFloats [0] >= 50) {
				GUI.Box (new Rect (Screen.width * 0.14f, Screen.height * 0.82f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Universal runner", MyStyle);
			}
			if (AchivementFloats [1] >= 12) {
				GUI.Box (new Rect (Screen.width * 0.33f, Screen.height * 0.82f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Destroyer of nature", MyStyle);
			}
			if (AchivementFloats [2] >= 15) {
				GUI.Box (new Rect (Screen.width * 0.52f, Screen.height * 0.82f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Exterminator", MyStyle);
			}
			if (AchivementFloats [3] >= 24) {
				GUI.Box (new Rect (Screen.width * 0.71f, Screen.height * 0.82f, Screen.width * 0.15f, Screen.height * 0.15f),
					"Recharge addict", MyStyle);
			}
		}
	}
}
