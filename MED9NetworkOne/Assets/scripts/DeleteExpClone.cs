using UnityEngine;
using System.Collections;

public class DeleteExpClone : MonoBehaviour {

	private float counter = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		counter += 1f * Time.deltaTime;
		if (counter >= 5) {
			Destroy (gameObject);
		}
	
	}
}
