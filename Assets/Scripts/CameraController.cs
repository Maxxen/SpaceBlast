using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

   
    public float distance;
    Vector3 offset;
    GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        var angle = (90 - this.transform.rotation.eulerAngles.x) * Mathf.Deg2Rad;
        offset = new Vector3(0, Mathf.Cos(angle) * distance, Mathf.Sin(angle) * -distance);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = player.transform.position + offset;
	}
}
