using Assets.Scripts.EnemyAI.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Enemy;
using Assets.Scripts.EnemyAI.Actions;

namespace Assets.Scripts.EnemyAI
{
    class AIStateController : MonoBehaviour
    {

        public AIState currentState;
        public bool AIActive;
        
        public NavMeshAgent nav;
        public Animator anim;
        public IWeapon attack;
        public IDamageable health;

        public GameObject player;

        // Use this for initialization
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            nav = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            attack = GetComponent<IWeapon>();
            health = GetComponent<IDamageable>();
        }

        // Update is called once per frame
        void Update()
        {
            if (AIActive)
            {
                currentState.UpdateState(this);
            }
        }

        private void OnDrawGizmos()
        {
            if (currentState != null)
            {
                Gizmos.color = currentState.sceneGizmoColor;
                Gizmos.DrawWireSphere(transform.position, 1);
            }
        }

        public void TransitionToState(AIState nextState)
        {
            currentState = nextState;
            anim.SetTrigger(currentState.animationTrigger);
            stateSwitchTime = Time.time;
            foreach(AIAction action in currentState.actions)
            {
                if(action != null)
                    action.StartAction(this);
            }
        }

        float stateSwitchTime;
        public float TimeSinceStateSwitch()
        {
            return Time.time - stateSwitchTime;
        }
    }
}
