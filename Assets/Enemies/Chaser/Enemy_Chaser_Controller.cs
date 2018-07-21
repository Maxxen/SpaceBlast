using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Chaser_Controller : MonoBehaviour {

    public GameObject Player;
    public float range;
    public float maxTurnSpeed = 20f;
    public float movementSpeed = 5f;

    CharacterController control;
    Animator anim;

	// Use this for initialization
	void Start () {
        control = gameObject.GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        anim.Play("Idle");
	}
	
	// Update is called once per frame
	void Update ()
    {
        var distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(Player.transform.position.x, Player.transform.position.z));

        if (distance < range)
        {
            //var turnFactor = Mathf.InverseLerp(0, range, distance);
            //var turnSpeed = maxTurnSpeed * turnFactor;
            var lookdir = Player.transform.position - transform.position;
            var rot = Quaternion.LookRotation(lookdir);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);

            transform.rotation = rot;
            control.Move(movementSpeed * Time.deltaTime * lookdir.normalized);
            this.anim.SetTrigger("Chase");
        }
        else
        {
            this.anim.SetTrigger("StopChase");
        }
	}
}
