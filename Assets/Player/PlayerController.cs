using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 5;
    public float baseSpeed = 4;
    public Transform chestBone;

    CharacterController control;
    Animator anim;
    Vector3 move;
    Vector3 lookPos;

	// Use this for initialization
	void Start ()
    {
        control = GetComponent<CharacterController>();
        SetupAnimator();
   	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        HandleInput();
        HandleMovement();
        HandleAnimation();

    }

    void LateUpdate()
    {
        HandleAiming();
    }

    void SetupAnimator()
    {
        anim = GetComponent<Animator>();
    }

    void HandleInput()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if(move.sqrMagnitude > 1)
        {
            //This ensures that diagonal movement is just as fast as orthogonal
            move.Normalize();
        }
    }

    void HandleMovement()
    {
        control.Move(move * speed * Time.deltaTime);
    }

    void HandleAnimation()
    {
        if (move.sqrMagnitude != 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move, Vector3.up), Time.deltaTime * speed);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, chestBone.rotation, Time.deltaTime * speed);
        } 

        Vector3 localMove = transform.InverseTransformDirection(move);

        anim.speed = speed / baseSpeed;
        anim.SetFloat("X", localMove.x, 0.15f, Time.deltaTime);
        anim.SetFloat("Y", localMove.z, 0.15f, Time.deltaTime);

        if (Input.GetMouseButton(0))
        {
            anim.SetBool("Shoot", true);
        }
        else
        {
             anim.SetBool("Shoot", false);
        } 
    }

    void HandleAiming()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 hitPos = ray.GetPoint(hitdist);
            lookPos = hitPos - chestBone.transform.position;
            lookPos.y = 0;
        }
        
        chestBone.transform.LookAt(lookPos + chestBone.transform.position, Vector3.up); 
    }
}
