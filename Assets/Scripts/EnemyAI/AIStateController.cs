using Assets.Scripts.EnemyAI.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Weapon;
using Assets.Scripts.EnemyAI.Actions;

namespace Assets.Scripts.EnemyAI
{
    class AIStateController : MonoBehaviour
    {
        public AIState currentState;

        [HideInInspector]
        public NavMeshAgent nav;
        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public IWeapon attack;
        [HideInInspector]
        public GameObject player;

        float stateSwitchTime;
        bool aiActive = true;
        GameController gameController;

        // Use this for initialization
        void Start()
        {
            gameController = GameObject.Find("GameController").GetComponent<GameController>();
            player = GameObject.FindGameObjectWithTag("Player");
            nav = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            attack = GetComponent<IWeapon>();

            stateSwitchTime = Time.time;

            gameController.OnPause += () => aiActive = false;
            gameController.OnResume += () => aiActive = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!gameController.IsPaused)
            {
                currentState.UpdateState(this);
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

        public float TimeSinceStateSwitch()
        {
            return Time.time - stateSwitchTime;
        }

        private void OnDrawGizmos()
        {
            if (currentState != null)
            {
                Gizmos.color = currentState.sceneGizmoColor;
                Gizmos.DrawWireSphere(transform.position, 1);
            }
        }
    }
}
