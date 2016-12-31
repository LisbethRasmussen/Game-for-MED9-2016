using UnityEngine;
using System.Collections;
using UnityEngine.UI; //Because we need to access the canvas, which is placed under the camera, which is placed under our player character.
using UnityEngine.Networking;

public class RobotScript : NetworkBehaviour {

	private GameObject MasterScriptObject; //two lines for accessing public variables in the Master script placed on the MasterScript object
	private Master M;

	//-----------------------------------Animation stuff
	public float animSpeed = 1f;				// a public setting for overall animator animation speed

	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;	// a reference to the current state of the animator, used for base layer

	private float AnimFloat = 0;
	private bool[] AnimBools = new bool[8];
	//-------------------Nope
	int WalkForwardHash = Animator.StringToHash("WalkForward");
	int WalkBackwardHash = Animator.StringToHash("WalkBackwards");
	int IdleHash = Animator.StringToHash("Idle");
	int RunHash = Animator.StringToHash("RuN");
	int RollHash = Animator.StringToHash("Unarmed-Roll-Forward");
	int JumpHash = Animator.StringToHash("JumP");
	int FallHash = Animator.StringToHash("Unarmed-Fall");
	int LandHash = Animator.StringToHash("Unarmed-Land");
	//-------------------
	private float RotationSpeedNormal = 0f;
	private float RotationSpeedRun = 0f;
	private float RotationSpeed = 0f;

	private bool OnGround = true;
	private float UpDownMaxValue = 0.7f;
	private float UpDownValue = 0f;
	private float DiminishingUpDownValue = 0.1f;

	//-----------------------------------

	//-----------------------------------Networking stuff that turns things off and/or delete them if needed
	private bool YouMayMove = false;
	public GameObject MyCamera;
	public GameObject ShooterInitiater; //I think this is the same as LaserPointer, but I don't care at the moment.
	public GameObject BigLaserRenderObject;
	private LineRenderer MyLineRend;
	private Vector3 Position1Laser;
	private Light MyBigLaserNetworkShootIndicator; //This light will switch on and off as the big laser is fired. This is done since the line renderer hates networking apparently.
	private bool BossIsSpawnedOnNetwork = false;
	//We will also remove the mobspawners on the client side, but I made a function for that further down.
	//-----------------------------------

	//-----------------------------------Shooting stuff
	private bool IsRecharging = false;
	//these are the positions we soot from
	public Transform RightEye;
	public Transform LeftEye;
	//The GUI effect
	private float DiminishFactorInPercent = 0f;
	private float BoxMaxHeight = 0f;
	private float BoxHeight = 0f;
	//-----------------------------------

	//-----------------------------------Things used for death
	//We need to know whether or not the bot is dead in order to activate and deactivate features.
	private bool Dead = false;
	private Vector3 SaftyKeepPlayerInPlaceWhereTheyDied;
	//This bool is used by the enemies to kill the robot when the shield id de-activated.
	public bool InitiateDeaht = false;
	//When a bot dies, it will go off with a boom!
	private Transform Explosion;
	//This shows the player the shield's "life force" and the shield it self.
	public GameObject MyShield;
	private float MaxShieldSliderValue = 0f;
	public Slider ShieldHealthSlider;
	//When dying, we need to set these things invisible
	public GameObject TheRobot;
	public GameObject SpecialWeapon; //I will use this variable to help structuring the big laser script, by giving it a name depending on which character is holding it. Edit, as everything fucked up that was not necessary, however, now I use it for other things.
	private SkinnedMeshRenderer RobotRender;
	private MeshRenderer SpecialWeaponRender;
	public GameObject LaserPoint;
	//Lastley, we need to respawn again, but we only want this to be called one every respawn. We also nee a certain location to spawn at.
	private bool RespawnOnce = false;
	private Vector3 RespawnSpot;
	//-----------------------------------

	//-----------------------------------Boss health, because we need the canvas attached to the robot.
	private float MaxBossHealth = 0f;
	public Slider BossHealthSlider;
	public GameObject BossSliderObject;
	//In order to avoid some networking problems, we will spawn the boss from this script
	private GameObject TheBossPrefab;
	private Transform ThePlaceToSpawnTheBoos;

	private GameObject FireballsFromTheBoss;
	private Vector3[] FireballSpawningPositions = new Vector3[8];
	private int TempForArrays = 0;
	private float TimeBetweenFireballShots = 0f;
	//For some reason another prefab of the boss spawns, so this is to help remove it.
	private string BossCloneName;
	//-----------------------------------

	//-----------------------------------LevelUp scenario
	public GameObject LvlUpSliderObject;
	private float MaxLvlSliderValue = 0f;
	public Slider LvlUpSlider;
	public Text LvlUpText;
	private bool RunOnce = true;
	//-----------------------------------

	//-----------------------------------For the achivements scenario
	private float[] DifferenceInPositions = new float[9];
	private float PositionLogTime = 0f;
	private bool CalculateDistance = false;
	//-----------------------------------

	private float BugCounter = 0f;

	//-----------------------------------Minimap
	public GameObject Minimap;
	public GameObject MinimapMarker;
	private bool MapIsShown = false;
	private bool ThisCanRun = true;
	//-----------------------------------Test stuff for debug
	//Trololo

	//-----------------------------------

	void Start () {

		Minimap.SetActive(false);

		MasterScriptObject = GameObject.Find ("MasterScript"); //Looking for the original MasterScript object
		M = MasterScriptObject.GetComponent<Master> ();			//Retrieving the Master script component

		//-----------------------------------Animation stuff
		anim = GetComponent<Animator>();

		for (int i = 0; i < AnimBools.Length; i++) {
			AnimBools [i] = false;
		}

		RotationSpeedNormal = M.PublicValuesToBeAssignedInInspector [13];
		RotationSpeedRun = RotationSpeedNormal * 0.8f;

		UpDownMaxValue = M.PublicValuesToBeAssignedInInspector [12];
		UpDownValue = UpDownMaxValue;
		//-----------------------------------

		//-----------------------------------Giving the name Hostering if no object with that name is on the scene, and if Hostering is on the scene, name the object Locally.
											// This is basically in order to figure out in code and on the scene, whether or not our active player is the server or the client.
											//But since it is not dependent in any way on the network being active or not, this also helps with other code, and will run at every game version.
			if (GameObject.Find ("Hostering") != null) {
				name = "Locally";
				SpecialWeapon.tag = "LocallyWeapon";
			} else {
				name = "Hostering";
				SpecialWeapon.tag = "HosteringWeapon";
			}

		MyBigLaserNetworkShootIndicator = SpecialWeapon.GetComponent<Light> ();
		MyBigLaserNetworkShootIndicator.enabled = false;
		//-----------------------------------

		//-----------------------------------Death stuff
		MyShield.tag = gameObject.name;
		//Setting up the shield
		MaxShieldSliderValue = M.PublicValuesToBeAssignedInInspector [3];
		ShieldHealthSlider.maxValue = MaxShieldSliderValue;
		ShieldHealthSlider.value = MaxShieldSliderValue;
		//Setting up the things that should become invisible, and the explosion that will happen when the bot dies
		RobotRender = TheRobot.GetComponent<SkinnedMeshRenderer> ();
		SpecialWeaponRender = SpecialWeapon.GetComponent<MeshRenderer> ();
		Explosion = M.TransformObjects [1];
		//-----------------------------------

		//-----------------------------------Networking stuff
		if (M.MyBooleans [2]) {
			/*if (GameObject.Find ("NonPlayerCamera") != null) {
				Destroy (GameObject.Find ("NonPlayerCamera"));
			}*/
			if (isLocalPlayer) {
				YouMayMove = true;
				M.NeededNamesForNetwork [0] = name;
				if (name == "Hostering") {
					var BiggiLaser = (GameObject)Instantiate (BigLaserRenderObject, new Vector3 (0,0,0), Quaternion.identity);
					NetworkServer.Spawn (BiggiLaser);
				}
				if (name == "Locally") {
					DestroyMobbieSpawner (); //Because We only want the server to spawn the smal mobs
				}
				MyLineRend = GameObject.Find("SpecialWeaponEffects(Clone)").GetComponent<LineRenderer> ();
				MyLineRend.SetVertexCount (2);
			}
			if (!isLocalPlayer) { //When the bot in the window is not the player, then we need to stop it from taking input, destroy the camera to avoid odd effects and prevent it from shooting on its own.
				YouMayMove = false;
				Destroy (MyCamera);
				Destroy (ShooterInitiater);
				//Because we still need to access the weapon of the other player
				if (name == "Locally") {
					SpecialWeapon.tag = "LocallyWeapon";
				}
				if (name == "Hostering") {
					SpecialWeapon.tag = "HosteringWeapon";
				}
				RobotRender.material = M.materials [1];
				SpecialWeaponRender.material = M.materials [2];

			}
		}
		if (!M.MyBooleans [2]) {
			YouMayMove = true;
		}
		//-----------------------------------

		//-----------------------------------Shooting stuff, this is basically synchronizing the shooting recharge timer with the gui effect in the LaserPointer script.
		DiminishFactorInPercent = M.PublicValuesToBeAssignedInInspector [10];
		BoxMaxHeight = Screen.height / 2.5f;
		BoxHeight = BoxMaxHeight;
		//-----------------------------------

		//-----------------------------------Boss stuff
		//We only want the boss to exist in scenario Competent (array number 0) and scenario Relatedness (array number 2)
		//When we don't care about networking we run this
		if (M.MyBooleans [0]) {
			var Bosse = (GameObject)Instantiate (M.GameobjectObjects [0], M.TransformObjects [3].position, Quaternion.identity);
			Bosse.name = "TheBoss(Clone)"; //To avoid confusion in the code, we name the boss "TheBoss(Clone)"
		}

		//We only want the boos slider health to be shown when the boss is actually on the map.
		if (GameObject.Find ("TheBoss(Clone)")) {
			BossSliderObject.SetActive (true);
			MaxBossHealth = M.PublicValuesToBeAssignedInInspector [14];
			BossHealthSlider.maxValue = MaxBossHealth;
		}
		else {
			BossSliderObject.SetActive (false);
		}
		if (M.MyBooleans [2] && name == "Locally") {
			BossSliderObject.SetActive (true);
			MaxBossHealth = M.PublicValuesToBeAssignedInInspector [14];
			BossHealthSlider.maxValue = MaxBossHealth;
		}
		FireballsFromTheBoss = M.GameobjectObjects [13];

		FireballSpawningPositions[0] = new Vector3 (0,0,2);
		FireballSpawningPositions[1] = new Vector3 (0,2,2);
		FireballSpawningPositions[2] = new Vector3 (0,2,0);
		FireballSpawningPositions[3] = new Vector3 (0,0,-2);
		FireballSpawningPositions[4] = new Vector3 (0,-2,-2);
		FireballSpawningPositions[5] = new Vector3 (0,-2,0);
		FireballSpawningPositions[6] = new Vector3 (0,2,-2);
		FireballSpawningPositions[7] = new Vector3 (0,-2,2);
		//-----------------------------------

		//-----------------------------------LevelUp scenario
		MaxLvlSliderValue = 2 * Mathf.Sqrt (M.FloatsToAccessInOtherScripts[3]) + (7 * M.FloatsToAccessInOtherScripts[3]);//ax^2+bx+c
		LvlUpSlider.maxValue = MaxLvlSliderValue;
		LvlUpSlider.value = 0f;
		LvlUpText.text = "Level " + M.FloatsToAccessInOtherScripts[3];
		LvlUpSliderObject.SetActive (false);
		//-----------------------------------
	
	}

	void Update () {
		
		if (M.MyBooleans [2]) { //just to be sure
			if (!isLocalPlayer) {
				return;
			}
		}
		gameObject.GetComponent <Rigidbody> ().WakeUp();

		if (Input.GetKeyDown (KeyCode.G)) {
			GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
		}
		if (RunOnce && M.MyBooleans [5]) {
			LvlUpSliderObject.SetActive (true);
			MaxLvlSliderValue = 2 * Mathf.Sqrt (M.FloatsToAccessInOtherScripts[3]) + (7 * M.FloatsToAccessInOtherScripts[3]);//ax^2+bx+c
			LvlUpSlider.maxValue = MaxLvlSliderValue;
			LvlUpSlider.value = M.FloatsToAccessInOtherScripts[4];
			LvlUpText.text = "Level " + M.FloatsToAccessInOtherScripts[3];
			RunOnce = false;
		}
		if (M.MyBooleans [5]) {
			LvlUpSlider.value = M.FloatsToAccessInOtherScripts[4];
			if (LvlUpSlider.value >= MaxLvlSliderValue) {
				M.FloatsToAccessInOtherScripts [3] += 1;
				M.FloatsToAccessInOtherScripts [4] = 0;
				RunOnce = true;
			}
		}
		//Shooting fireballs from the server robot
		if (M.MyBooleans [2]) {
			if (YouMayMove && name == "Hostering") {
				if (GameObject.Find ("TheBoss(Clone)") != null) {
					TimeBetweenFireballShots += 1 * Time.deltaTime;
					if (TimeBetweenFireballShots >= M.PublicValuesToBeAssignedInInspector [23]) {

						CmdSpawnFireball ();

						TimeBetweenFireballShots = 0f;
					}
				}
			}
		}

		if (YouMayMove) { //YouMayMove and do stuff
			//Initate the function which makes us move the bot

			if (Input.GetKeyDown (KeyCode.M)) {
				ThisCanRun = true;
				if (MapIsShown && ThisCanRun){
					Minimap.SetActive(false);
					MapIsShown = false;
					ThisCanRun = false;
				}
				if (!MapIsShown && ThisCanRun){
					Minimap.SetActive(true);
					MapIsShown = true;
					ThisCanRun = false;
				}
			}
			if (MapIsShown) {
				MinimapMarker.transform.localPosition = new Vector3 (transform.position.x, transform.position.z, 0f);
			}


			if (!Dead) {
				Moving ();

				//-----------------------------------Shooting stuff
				if (Time.timeScale == 1) {
					if (PositionLogTime == 0) {
						DifferenceInPositions [0] = transform.position.x;
						DifferenceInPositions [1] = transform.position.y;
						DifferenceInPositions [2] = transform.position.z;
					}
					PositionLogTime += 1 * Time.deltaTime;
					if (PositionLogTime > 1) {
						DifferenceInPositions [3] = transform.position.x;
						DifferenceInPositions [4] = transform.position.y;
						DifferenceInPositions [5] = transform.position.z;
						CalculateDistance = true;
					}
					if (CalculateDistance) {
						DifferenceInPositions [6] = Mathf.Abs(DifferenceInPositions [0]-DifferenceInPositions [3]);
						DifferenceInPositions [7] = Mathf.Abs(DifferenceInPositions [1]-DifferenceInPositions [4]);
						DifferenceInPositions [8] = Mathf.Abs(DifferenceInPositions [2]-DifferenceInPositions [5]);
						M.AchivementFloats [0] += (DifferenceInPositions [6] + DifferenceInPositions [7] + DifferenceInPositions [7])*0.08f;
						PositionLogTime = 0f;
						CalculateDistance = false;
					}
					if (Input.GetMouseButtonUp (0) && !IsRecharging && !Input.GetKey(KeyCode.L)) {
						if (!Input.GetKey (KeyCode.E)) { //Abort shooting if we are pressing E
							//If we are multiplaying
							if (M.MyBooleans [2]) {
								CmdShooting ();
							}
							//If we are not multiplaying
							else {
								Instantiate (M.GameobjectObjects [3], RightEye.position, Quaternion.identity);
								Instantiate (M.GameobjectObjects [3], LeftEye.position, Quaternion.identity);
								IsRecharging = true;
							}
						}//whether or not we press E
					}//Mousebutton up
					if (M.MyBooleans[2] && GameObject.Find ("TheBoss(Clone)") != null){ //If we are not dead the big laser in scenario 3 is working.
						MyLineRend.SetPosition (0, GameObject.Find ("TheBoss(Clone)").transform.position);
						MyLineRend.SetPosition (1, Position1Laser);

						if (Input.GetMouseButton (1) && GameObject.Find("TheBoss(Clone)")) {
							if (M.PublicValuesToBeAssignedInInspector [25] >= Vector3.Distance (transform.position, GameObject.Find ("TheBoss(Clone)").transform.position)) {
								CmdLaserActivityOn ();
								if (name == "Hostering") {
									CmdSetDamHost ();
								}
								if (name == "Locally") {
									CmdSetDamClient ();
								}
							}
							if (M.PublicValuesToBeAssignedInInspector [25] <= Vector3.Distance (transform.position, GameObject.Find ("TheBoss(Clone)").transform.position)) {
								CmdLaserActivityOn ();
								if (name == "Hostering") {
									CmdSetDamHostToZero ();
								}
								if (name == "Locally") {
									CmdSetDamClientToZero ();
								}
							}

						}
						if (!Input.GetMouseButton (1) || GameObject.Find("TheBoss(Clone)") == null) {
							CmdLaserActivityOff ();
							if (name == "Hostering") {
								CmdSetDamHostToZero ();
							}
							if (name == "Locally") {
								CmdSetDamClientToZero ();
							}
						}
					}
					//In case the player gets stuck and need to respawn, the deathbutton has been put back into place.
					if (Input.GetKeyDown(KeyCode.K)){
						if (anim.enabled) {
							anim.Stop ();
							anim.enabled = false;
							MyShield.SetActive (false);
							InitiateDeaht = true;
						}
					}
				}//Time scale
				if (IsRecharging) { //We don't want the player to keep spam shooting.
					BoxHeight -= (Screen.height*(DiminishFactorInPercent/100))*Time.deltaTime;
					if (BoxHeight <= 0) {
						BoxHeight = BoxMaxHeight;
						IsRecharging = false;
					}
				}
			}//Dead
			//----------------------------------- End of shooting stuff
		}//YouMayMove

		//-----------------------------------Death and respawn stuff
		//The shield that protects the robot
		if (MyShield.activeSelf) {
			if (ShieldHealthSlider.value <= 0) {
				ShieldHealthSlider.value = 0;
				if (!M.MyBooleans [2]) {
					MyShield.SetActive(false); //If not networking
				}
				if (M.MyBooleans [2]) {
					CmdShield (); //If networking
				}
			}
		}
		//The shield is gone and now bot will die!
		if ((InitiateDeaht && !MyShield.activeSelf)){
			SaftyKeepPlayerInPlaceWhereTheyDied = transform.position;
			if (!M.MyBooleans[2]){ //A function for not networking
				RobotRender.enabled = false; //robot body goes invisible
				SpecialWeaponRender.enabled = false; //weapon goes invisible, this is needed cause the weapon is not part of the original robot body.
				Instantiate (Explosion, transform.position, Quaternion.identity); //Make an explosion
				RespawnSpot = M.Positions3D [0]; //Log where we need to spawn upon respawn
				LaserPoint.SetActive (false); //We should not be able to shoot while we are dead.
				Dead = true; //letting the rest of the script aware that we are dead now.
				InitiateDeaht = false;
				if (gameObject.name == "Hostering") {//we want small mobs to delete themselves when no player is alive.
					M.MyBooleans [7] = true;
				}
				else {
					M.MyBooleans [8] = false;
				}
			}
			if (M.MyBooleans[2]){ //A function for networking
				CmdDeath();
				/*For some reason the shield Cmd and Rpc works, but this does not apply for the death? So inputtet this copy paste code. Time is an issue here.*/
				/*It may have something to do with the network identity assigned in the inspecter on the parent robot object. But honestly, I cannot keep playing around with this.*/
				/*BECAUSE FUCK LOGIC???*/RobotRender.enabled = false; //robot body goes invisible
				/*BECAUSE FUCK LOGIC???*/SpecialWeaponRender.enabled = false; //weapon goes invisible, this is needed cause the weapon is not part of the original robot body.
				/*BECAUSE FUCK LOGIC???*/Instantiate (Explosion, transform.position, Quaternion.identity); //Make an explosion
				/*BECAUSE FUCK LOGIC???*/RespawnSpot = M.Positions3D [0]; //Log where we need to spawn upon respawn
				/*BECAUSE FUCK LOGIC???*/LaserPoint.SetActive (false); //We should not be able to shoot while we are dead.
				InitiateDeaht = false;
				/*BECAUSE FUCK LOGIC???*/Dead = true; //letting the rest of the script aware that we are dead now.
				/*BECAUSE FUCK LOGIC???*/if (gameObject.name == "Hostering") {//we want small mobs to delete themselves when no player is alive.
					/*BECAUSE FUCK LOGIC???*/M.MyBooleans [7] = true;
					/*BECAUSE FUCK LOGIC???*/}
				/*BECAUSE FUCK LOGIC???*/else {
					/*BECAUSE FUCK LOGIC???*/M.MyBooleans [8] = false;
					/*BECAUSE FUCK LOGIC???*/}
			}
		}
		if (RespawnOnce) {
			if (!M.MyBooleans[2]){
				if (!anim.enabled) {
					anim.enabled = true;
				}
				ShieldHealthSlider.value = MaxShieldSliderValue;
				MyShield.SetActive (true);
				RobotRender.enabled = true;
				SpecialWeaponRender.enabled = true;
				//Destroy (GameObject.FindWithTag("Clone")); //We don't want to keep the explosion instances on the scene, as they will just pile up if the player dies multipla times.
				transform.position = RespawnSpot;
				LaserPoint.SetActive (true);
				if (gameObject.name == "Hostering") {
					M.MyBooleans [7] = false;
				} else {
					M.MyBooleans [8] = false;
				}
				Dead = false;
				GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
				RespawnOnce = false;
			}
			if (M.MyBooleans [2]) {
				CmdRespawn ();
				if (!anim.enabled) {
					anim.enabled = true;
				}
				//#"!%#"%&/()/¤(&/%¤&#"%#¤%"#&%/¤(/¤¤%&#"%!"#¤%"/&%/(#%%&#¤%"%/&
				/*BECAUSE FUCK LOGIC???*/ShieldHealthSlider.value = MaxShieldSliderValue;
				/*BECAUSE FUCK LOGIC???*/MyShield.SetActive (true);
				/*BECAUSE FUCK LOGIC???*/RobotRender.enabled = true;
				/*BECAUSE FUCK LOGIC???*/SpecialWeaponRender.enabled = true;
				/*BECAUSE FUCK LOGIC???*/Destroy (GameObject.FindWithTag("Clone")); //We don't want to keep the explosion instances on the scene, as they will just pile up if the player dies multipla times.
				/*BECAUSE FUCK LOGIC???*/transform.position = RespawnSpot;
				/*BECAUSE FUCK LOGIC???*/LaserPoint.SetActive (true);
				/*BECAUSE FUCK LOGIC???*/if (gameObject.name == "Hostering") {
					/*BECAUSE FUCK LOGIC???*/M.MyBooleans [7] = false;
				/*BECAUSE FUCK LOGIC???*/} else {
					/*BECAUSE FUCK LOGIC???*/M.MyBooleans [8] = false;
					/*BECAUSE FUCK LOGIC???*/}
				/*BECAUSE FUCK LOGIC???*/Dead = false;
				GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
				/*BECAUSE FUCK LOGIC???*/RespawnOnce = false;
			}
		}
		//-----------------------------------End of death and respawn stuff

		//-----------------------------------Enemy stuff
		//Keep an eye on the boss health
		if (GameObject.Find ("TheBoss(Clone)") != null) {
			//Explanation for the following line of code:
			//As we need both bots to shoot at the same time, we have the equation: (damage to boss)*(Bot Host active)*(Bot Client Active)
			//Or in other words, I am using the method of multiplying with 0, which just needs to be added once in order for the entire thing to be zero.
			//e.g. If no bots shoots, the equation will look like this: (damage to boss)*0*0 = 0;
			//and if only one of the shoots: (damage to boss)*1*0 = 0;
			//And if both of them shoots: (damage to boss)*1*1 = (damage to boss)
			M.PublicValuesToBeAssignedInInspector[14] -= M.PublicValuesToBeAssignedInInspector[16]*M.PublicValuesToBeAssignedInInspector[17]*M.PublicValuesToBeAssignedInInspector[18];
			BossHealthSlider.value = M.PublicValuesToBeAssignedInInspector [14];
			if (M.PublicValuesToBeAssignedInInspector [14] == 0) { //The boss dies and explodes, the small mobs stops getting spawned.
				Instantiate (Explosion, GameObject.Find ("TheBoss(Clone)").transform.position, Quaternion.identity);
				Destroy (GameObject.Find ("TheBoss(Clone)"));
				Destroy (GameObject.Find("SpecialWeaponEffects(Clone)"));
				DestroyMobbieSpawner ();
			}
		}

		//Spawn small mobs from server always
		if (name == "Hostering"){
			if (!M.MyBooleans[2] && MobSpawner.GetSpawnSmallMob()) {
				var SmallMobi = (GameObject)Instantiate (M.GameobjectObjects [5], M.Positions3D[1], Quaternion.identity);
				MobSpawner.SetSpawnSmallMob (false);
			}
			if (M.MyBooleans [2]) {
				CmdSpawnMob ();
				if (GameObject.Find ("Locally") != null && !BossIsSpawnedOnNetwork) { //333
					CmdSpawnBossNetwork();
					BossSliderObject.SetActive (true);
					MaxBossHealth = M.PublicValuesToBeAssignedInInspector [14];
					BossHealthSlider.maxValue = MaxBossHealth;
					BossIsSpawnedOnNetwork = true;
				}
			}
		}
		//-----------------------------------End of enemy stuff

	}

	void Moving (){

		/*while(anim.GetCurrentAnimatorStateInfo (0).IsName ("Unarmed-Jump")){
			print ("I am Jumping");
		}
		while(anim.GetCurrentAnimatorStateInfo (0).IsName ("Unarmed-Fall")){
			print ("I am falling");
		}
		while(anim.GetCurrentAnimatorStateInfo (0).IsName ("Unarmed-Land")){
			print ("I am Landing");
		}*/



		if (Input.GetKey (KeyCode.I) || AnimBools[7]) {
			for (int i = 0; i < AnimBools.Length; i++) {
				AnimBools [i] = false;
			}
			anim.SetBool ("Run", AnimBools [0]);
			anim.SetBool ("Jump", AnimBools [1]);
			anim.SetBool ("Falling", AnimBools [2]);
			anim.SetBool ("Land", AnimBools [3]);
			UpDownValue = UpDownMaxValue;
			BugCounter = 0f;

			anim.SetBool ("AnyToIdle", true);
			AnimBools [7] = false;
		}
		else {
			anim.SetBool ("AnyToIdle", false);
		}

		//-----------------------------------------------------------------------Turn rigth
		if (Input.GetKey (KeyCode.D)) {
			transform.Rotate (Vector3.up * RotationSpeed * Time.deltaTime);
		}
		//-----------------------------------------------------------------------Turn left
		if (Input.GetKey (KeyCode.A)) {
			transform.Rotate (Vector3.up * -RotationSpeed * Time.deltaTime);
		}
		//-----------------------------------------------------------------------Walk forward
		if (Input.GetKey (KeyCode.W) && !Input.GetKey (KeyCode.S)) {
			AnimFloat = 0.3f;
			anim.SetFloat ("Speed", AnimFloat);
			//anim.SetTrigger (WalkForwardHash);
		}
		//-----------------------------------------------------------------------Walk backward
		if (Input.GetKey (KeyCode.S) && !Input.GetKey (KeyCode.W)) {
			AnimFloat = -0.3f;
			anim.SetFloat ("Speed", AnimFloat);
			//anim.SetTrigger (WalkBackwardHash);
		}
		//-----------------------------------------------------------------------Stay idle
		if (!Input.GetKey (KeyCode.S) && !Input.GetKey (KeyCode.W)) {
			AnimFloat = 0f;
			anim.SetFloat ("Speed", AnimFloat);
			//anim.SetTrigger (IdleHash);
		}
		//-----------------------------------------------------------------------Run
		if (Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.LeftShift)) {
			AnimBools [0] = true;
			anim.SetBool ("Run", AnimBools [0]);
			RotationSpeed = RotationSpeedRun;
			if (Input.GetKeyDown (KeyCode.Space) && !AnimBools [2] && !AnimBools [3]) {
				AnimBools [1] = true;
				anim.SetBool ("Jump", AnimBools [1]);
				//anim.SetTrigger(JumpHash);
			}
		}
		//-----------------------------------------------------------------------Stop running, safty code
		if(!Input.GetKey (KeyCode.LeftShift)) {
			AnimBools [0] = false;
			anim.SetBool ("Run", AnimBools [0]);
			RotationSpeed = RotationSpeedNormal;
		}
		//-----------------------------------------------------------------------Jump
		if (AnimBools [1]) {
			BugCounter += 1 * Time.deltaTime;
			if (Input.GetKey(KeyCode.W)) {
				transform.localPosition += transform.forward * M.PublicValuesToBeAssignedInInspector [11] * Time.deltaTime;
				if (UpDownValue > 0f) {
					transform.localPosition += transform.up * M.PublicValuesToBeAssignedInInspector [11] * UpDownValue * Time.deltaTime;
					UpDownValue -= DiminishingUpDownValue; //as gravity works on us, we need the jumping up force to be diminished during the jump duration
				}
			}
			if (BugCounter >= 0.4f){
				if (OnGround) {
					AnimBools [1] = false;
					anim.SetBool ("Jump", AnimBools [1]);
					BugCounter = 0f;
					UpDownValue = UpDownMaxValue;
				}
			} //SCREW MECANIM
			/*//Force applied during jump and fall when we press W
			if (!AnimBools [3] && Input.GetKey(KeyCode.W)) {
				transform.localPosition += transform.forward * M.PublicValuesToBeAssignedInInspector [11] * Time.deltaTime;
				if (UpDownValue > 0f) {
					transform.localPosition += transform.up * M.PublicValuesToBeAssignedInInspector [11] * UpDownValue * Time.deltaTime;
					UpDownValue -= DiminishingUpDownValue; //as gravity works on us, we need the jumping up force to be diminished during the jump duration
				}
			}
			//-----
			BugCounter += 1*Time.deltaTime;
			if (BugCounter >= 0.4f && !AnimBools [2] && !AnimBools[3]) {
				AnimBools [2] = true;
				anim.SetBool("Falling", AnimBools [2]);
			}
			if (AnimBools [2] && OnGround && !AnimBools[3]){
				BugCounter = 0f; //maybe not.
				AnimBools [3] = true;
				anim.SetTrigger (LandHash);
			}
			if (AnimBools[3] && BugCounter >= 0.3f){
				for (int i = 1; i < AnimBools.Length; i++) {
					AnimBools [i] = false;
				}
				anim.SetBool("Falling",AnimBools [2]);
				BugCounter = 0f;
				UpDownValue = UpDownMaxValue;
				anim.SetTrigger (RunHash);
				//AnimBools [7] = true; //Debug
			}*/
			//When is jump done so we can fall
			/*BugCounter += 1 * Time.deltaTime;
			if (BugCounter >= 0.4f) {
				AnimBools [2] = true;
				anim.SetBool("Falling",AnimBools [2]);
			}
			//-----
			//Wen we stop falling and start landing
			if (AnimBools [2] && OnGround){
				AnimBools [3] = true;
				//anim.SetBool("Land",AnimBools [3]);
				anim.SetTrigger (LandHash);
				if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Unarmed-Land") && anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.9f) {
					for (int i = 0; i < AnimBools.Length; i++) {
						AnimBools [i] = false;
					}
					anim.SetBool ("Jump", AnimBools [1]);
					anim.SetBool("Falling",AnimBools [2]);
					anim.SetBool("Land",AnimBools [3]);
					UpDownValue = UpDownMaxValue;
					BugCounter = 0f;
				}
			}*/
		}
		//-----------------------------------------------------------------------Roll
		if (Input.GetKey (KeyCode.R) && AnimBools [0]) {
			AnimBools [4] = true;
			anim.SetBool ("Roll", AnimBools [4]);
		}
		if (!Input.GetKey (KeyCode.R)) {
			AnimBools [4] = false;
			anim.SetBool ("Roll", AnimBools [4]);
		}
	}

	void OnTriggerEnter (Collider other){ //debug
		if (other.gameObject.name == "Terrain" || other.gameObject.tag == "NotWater") {
			OnGround = true;
		}
		else{
			OnGround = false;
		}
	}

	void OnCollisionEnter (Collision other){
		if (other.gameObject.name == "FireBallFromBoss(Clone)" && MyShield.activeSelf) {
			ShieldHealthSlider.value -= M.PublicValuesToBeAssignedInInspector [24];
		}
		if (other.gameObject.name == "FireBallFromBoss(Clone)" && !MyShield.activeSelf) {
			InitiateDeaht = true;
		}
	}

	void OnCollisionStay (Collision other){
		if (M.MyBooleans[0]){
			if (other.gameObject.name == "FollowMeL1" || other.gameObject.name == "FollowMeL11") {
				transform.parent = other.transform.parent.gameObject.transform;
			}
			if (other.gameObject.name == "FollowMeR") {
				transform.parent = other.transform.parent.gameObject.transform;
			}
			if (other.gameObject.name == "FollowMeL2") {
				transform.parent = other.transform.parent.gameObject.transform;
			}
		}
	}
	void OnCollisionExit (Collision other){
		if (M.MyBooleans [0]) {
			if (other.gameObject.name == "FollowMeL1" || other.gameObject.name == "FollowMeL11" || other.gameObject.name == "FollowMeR" || other.gameObject.name == "FollowMeL2") {
				transform.parent = null;
			}
		}
	}

	[Command] //<- we use this in order for the client to be able to make things happens, as only the server can make some stuff happens. Command basically says to the server: run this thank you!
	void CmdShooting (){
		var bullet1 = (GameObject)Instantiate (M.GameobjectObjects [3], RightEye.position, Quaternion.identity);
		var bullet2 = (GameObject)Instantiate (M.GameobjectObjects [3], LeftEye.position, Quaternion.identity);
		NetworkServer.Spawn (bullet1);
		NetworkServer.Spawn (bullet2);
		IsRecharging = true;
	}

	[Command]
	void CmdShield(){
		MyShield.SetActive(false);
		RpcShield ();
	}
	[ClientRpc]
	void RpcShield(){ //It is apparently super important to start client functions with "Rpc" in the name, same as the "Cmd" in the server functions I suppose.
		MyShield.SetActive(false);
	}

	[Command]
	void CmdSuicide(){
		RespawnSpot = M.Positions3D [0];
		var NewBot = (GameObject)Instantiate (M.GameobjectObjects [14], RespawnSpot, Quaternion.identity);
		NetworkServer.Spawn (NewBot);
		M.NeededNamesForNetwork [1] = gameObject.name;
		M.MyBooleans [9] = true;
		Destroy (gameObject);
		RpcSuicide ();
	}
	[RPC]
	void RpcSuicide(){
		RespawnSpot = M.Positions3D [0];
		//var NewBot = (GameObject)Instantiate (M.GameobjectObjects [14], RespawnSpot, Quaternion.identity);
		M.NeededNamesForNetwork [1] = gameObject.name;
		M.MyBooleans [9] = true;
		Destroy (gameObject);
	}

	[Command]
	void CmdDeath(){
		RobotRender.enabled = false; //robot body goes invisible
		SpecialWeaponRender.enabled = false; //weapon goes invisible, this is needed cause the weapon is not part of the original robot body.
		Instantiate (Explosion, transform.position, Quaternion.identity); //Make an explosion
		RespawnSpot = M.Positions3D [0]; //Log where we need to spawn upon respawn
		LaserPoint.SetActive (false); //We should not be able to shoot while we are dead.
		InitiateDeaht = false;
		Dead = true; //letting the rest of the script aware that we are dead now.
		if (gameObject.name == "Hostering") {//we want small mobs to delete themselves when no player is alive.
			M.MyBooleans [7] = true;
		}
		else {
			M.MyBooleans [8] = false;
		}
		RpcDeath ();
	}
	[ClientRpc]
	void RpcDeath(){
		RobotRender.enabled = false; //robot body goes invisible
		SpecialWeaponRender.enabled = false; //weapon goes invisible, this is needed cause the weapon is not part of the original robot body.
		Instantiate (Explosion, transform.position, Quaternion.identity); //Make an explosion
		RespawnSpot = M.Positions3D [0]; //Log where we need to spawn upon respawn
		LaserPoint.SetActive (false); //We should not be able to shoot while we are dead.
		InitiateDeaht = false;
		Dead = true; //letting the rest of the script aware that we are dead now.
		if (gameObject.name == "Hostering") {//we want small mobs to delete themselves when no player is alive.
			M.MyBooleans [7] = true;
		}
		else {
			M.MyBooleans [8] = false;
		}
	}

	[Command]
	void CmdRespawn(){
		if (!anim.enabled) {
			anim.enabled = true;
		}
		ShieldHealthSlider.value = MaxShieldSliderValue;
		MyShield.SetActive (true);
		RobotRender.enabled = true;
		SpecialWeaponRender.enabled = true;
		Destroy (GameObject.FindWithTag("Clone")); //We don't want to keep the explosion instances on the scene, as they will just pile up if the player dies multipla times.
		transform.position = RespawnSpot;
		LaserPoint.SetActive (true);
		if (gameObject.name == "Hostering") {
			M.MyBooleans [7] = false;
		} else {
			M.MyBooleans [8] = false;
		}
		Dead = false;
		GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
		RespawnOnce = false;
		RpcRespawn ();
	}
	[ClientRpc]
	void RpcRespawn(){
		if (!anim.enabled) {
			anim.enabled = true;
		}
		ShieldHealthSlider.value = MaxShieldSliderValue;
		MyShield.SetActive (true);
		RobotRender.enabled = true;
		SpecialWeaponRender.enabled = true;
		Destroy (GameObject.FindWithTag("Clone")); //We don't want to keep the explosion instances on the scene, as they will just pile up if the player dies multipla times.
		transform.position = RespawnSpot;
		LaserPoint.SetActive (true);
		if (gameObject.name == "Hostering") {
			M.MyBooleans [7] = false;
		} else {
			M.MyBooleans [8] = false;
		}
		Dead = false;
		GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
		RespawnOnce = false;
	}

	void OnGUI(){
		if (Dead) {
			transform.position = SaftyKeepPlayerInPlaceWhereTheyDied;
			if (GUI.Button (new Rect (Screen.width / 2f - 50f, Screen.height / 2f - 25f, 100f, 50f), "Respawn")) {
				RespawnOnce = true;
			}
		}
	}

	[Command]
	void CmdSpawnMob(){
		if (MobSpawner.GetSpawnSmallMob()) {
			var SmallMobi = (GameObject)Instantiate (M.GameobjectObjects [5], M.Positions3D[1], Quaternion.identity);
			NetworkServer.Spawn (SmallMobi);
			MobSpawner.SetSpawnSmallMob (false);
		}
	}

	void DestroyMobbieSpawner(){
		GameObject[] MobgameObjects;

		MobgameObjects = GameObject.FindGameObjectsWithTag ("Mobbie"); //Remember the tag
		for (int i = 0; i < MobgameObjects.Length; i++) {
			Destroy (MobgameObjects [i]);
		}
	}

	[Command]
	void CmdSpawnBossNetwork(){
		var Bosse = (GameObject)Instantiate (M.GameobjectObjects [0], M.TransformObjects [3].position, Quaternion.identity);
		Bosse.name = "TheBoss(Clone)";
		NetworkServer.Spawn (Bosse);
	}

	[Command]
	void CmdSpawnFireball(){

		TempForArrays = Random.Range (0,7);
		var Fireballl = (GameObject)Instantiate (FireballsFromTheBoss,
			new Vector3 (GameObject.Find ("TheBoss(Clone)").transform.localPosition.x,
						 GameObject.Find ("TheBoss(Clone)").transform.localPosition.y + FireballSpawningPositions[TempForArrays].y,
						 GameObject.Find ("TheBoss(Clone)").transform.localPosition.z + FireballSpawningPositions[TempForArrays].z),
						 Quaternion.identity);
		NetworkServer.Spawn (Fireballl);
	}

	//Fire laser and set the light on the special weapon to true so that the other player can see that it is being fired.
	[Command]
	void CmdLaserActivityOn(){
		MyBigLaserNetworkShootIndicator.enabled = true;
		Position1Laser = SpecialWeapon.transform.position;
		RpcLaserActivityOn ();
	}
	[Command]
	void CmdLaserActivityOff(){
		MyBigLaserNetworkShootIndicator.enabled = false;
		if (GameObject.Find ("TheBoss(Clone)") != null) {
			Position1Laser = GameObject.Find ("TheBoss(Clone)").transform.position;
		}
		RpcLaserActivityOff ();
	}
	[ClientRpc]
	void RpcLaserActivityOn(){
		MyBigLaserNetworkShootIndicator.enabled = true;
		Position1Laser = SpecialWeapon.transform.position;
	}
	[ClientRpc]
	void RpcLaserActivityOff(){
		MyBigLaserNetworkShootIndicator.enabled = false;
		if (GameObject.Find ("TheBoss(Clone)") != null) {
			Position1Laser = GameObject.Find ("TheBoss(Clone)").transform.position;
		}
	}
	//Setting the damage to some value for the boss to get damaged by
	[Command]
	void CmdSetDamHost(){
		M.PublicValuesToBeAssignedInInspector [16] = 1f;
		RpcSetDamHost();
	}
	[Command]
	void CmdSetDamClient(){
		M.PublicValuesToBeAssignedInInspector [17] = 1f;
		RpcSetDamClient ();
	}
	[ClientRpc]
	void RpcSetDamHost(){
		M.PublicValuesToBeAssignedInInspector [16] = 1f;
	}
	[ClientRpc]
	void RpcSetDamClient(){
		M.PublicValuesToBeAssignedInInspector [17] = 1f;
	}
	//If both of the players are not shooting the boss should not take damage, and therefore we set the values to 0
	[Command]
	void CmdSetDamHostToZero(){
		M.PublicValuesToBeAssignedInInspector [16] = 0f;
		RpcSetDamHostToZero();
	}
	[Command]
	void CmdSetDamClientToZero(){
		M.PublicValuesToBeAssignedInInspector [17] = 0f;
		RpcSetDamClientToZero ();
	}
	[ClientRpc]
	void RpcSetDamHostToZero(){
		M.PublicValuesToBeAssignedInInspector [16] = 0f;
	}
	[ClientRpc]
	void RpcSetDamClientToZero(){
		M.PublicValuesToBeAssignedInInspector [17] = 0f;
	}
}
