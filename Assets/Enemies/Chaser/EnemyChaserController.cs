using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum EnemyChaserState { Idling, Chasing, Searching}
public class EnemyChaserController : MonoBehaviour {

    public GameObject player;
    public float range = 6;

    EnemyChaserState state = EnemyChaserState.Idling;

    NavMeshAgent nav;
    Animator anim;
    EnemyHealth stats;
    Renderer render;
    Vector3 lastKnownPosition;

	// Use this for initialization
	void Start () {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        stats = GetComponent<EnemyHealth>();
        render = GetComponentInChildren<Renderer>();

        //nav.speed = stats.MovementSpeed;

        anim.Play("Idle");
	}

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case EnemyChaserState.Idling:
                if (CanSeePlayer())
                {
                    nav.SetDestination(player.transform.position);
                    state = EnemyChaserState.Chasing;
                    this.anim.SetTrigger("StartChasing");
                }
                break;
            case EnemyChaserState.Chasing:
                nav.SetDestination(player.transform.position);
                if (!PlayerIsInRange())
                {
                    lastKnownPosition = player.transform.position;
                    state = EnemyChaserState.Searching;
                    this.anim.SetTrigger("StartSearching");
                }
                break;
            case EnemyChaserState.Searching:
                if (!CanSeePlayer())
                {
                    if (transform.position == lastKnownPosition)
                    {
                        state = EnemyChaserState.Idling;
                        this.anim.SetTrigger("StartIdling");
                    }
                    else
                    {
                        nav.SetDestination(lastKnownPosition);
                    }
                }
                else
                {
                    state = EnemyChaserState.Chasing;
                    this.anim.SetTrigger("StartChasing");
                }
                break;
            default:
                break;
        }
	}

    bool PlayerIsInRange()
    {
        return ((transform.position - player.transform.position).sqrMagnitude < (range * range));
    }

    bool CanSeePlayer()
    {
        if (!PlayerIsInRange())
            return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, player.transform.position - transform.position, out hit, range))
        {
            if (hit.collider.gameObject == player)
                return true;
        }
        return false;
    }

    void TakeDamage()
    {
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        
        render.material.SetFloat("_FlashAmount", 0.5f);
        yield return new WaitForSeconds(0.1f);
        render.material.SetFloat("_FlashAmount", 0);
    }

    void Die()
    {
        this.gameObject.SetActive(false);
    }
}
