using UnityEngine;
using System.Collections;

public class Floating : MonoBehaviour
{
    /*    public float floatStrength;               //Float Strength
        private Vector3 tempPos;          //Temporary position   
        float originalY; */
    float originalY;
    public float floatStrength = 1;

    void Start()
    {
        this.originalY = this.transform.position.y;
    }

    void Update()
    {
        /*      tempPos = transform.position;
              tempPos.y = originalY + (Mathf.Sin(Time.time) * floatStrength);
              transform.position = tempPos; */
        transform.position = new Vector3(transform.position.x,
        originalY + ((float)Mathf.Sin(Time.time) * floatStrength),
        transform.position.z);
    }
}