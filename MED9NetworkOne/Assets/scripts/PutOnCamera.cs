using UnityEngine;
using System.Collections;

public class PutOnCamera : MonoBehaviour {

	public Transform MyBot; //What we need to look at
	private float OrbitSpeed = 4.0f;
	private Vector3 offset; //This will help set the distance away from the bot
	private float AccumulatedMouseY = 0f;

	void Start () {

		//Making sure that we start of by looking at the player
		transform.LookAt (MyBot.transform);
		//Setting the initial position of the camera
		offset = new Vector3 (transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
	
	}

	void Update () {

		//if (Input.GetMouseButton (1)){
			AccumulatedMouseY += Input.GetAxis ("Mouse Y")*-1f;
			offset = Quaternion.AngleAxis (Input.GetAxis ("Mouse X") * OrbitSpeed, Vector3.up) * offset;
			transform.position = new Vector3 (MyBot.position.x + (offset.x * 2.7f),
											  MyBot.position.y + (offset.y * 2.7f) + AccumulatedMouseY,
											  MyBot.position.z + (offset.z * 2.7f));
			transform.LookAt (MyBot);
		//}
		/*if (!Input.GetMouseButton (1)){
			transform.position = new Vector3 (MyBot.position.x + (offset.x * 2.7f),
											  MyBot.position.y + (offset.y * 2.7f) + AccumulatedMouseY,
											  MyBot.position.z + (offset.z * 2.7f));
			transform.LookAt (MyBot);
		}*/
	}
}
	/*
		EXTRA NOTES
		the "Mouse (X/Y)" variable are dependent on how fast the mouse is moving.
		Vector3.up refers to the Y axis, where the rotating will occur around this axis, not to be mistaken for moving up and down the y axis.
		Vector3.right refers to the x axis, which is the axis that lets the player rotate the camera over and below the character.
	*/
