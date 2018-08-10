using Assets.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    class PlayerController : MonoBehaviour
    {

        public float animationBaseSpeed = 4;
        public Transform chestBone;
        public Transform gunBone;
        public GameObject bulletPrefab;

        ObjectPool bulletPool;

        CharacterController control;
        Animator anim;
        PlayerStats stats;

        Vector3 move;
        Vector3 lookDirection;

        // Use this for initialization
        void Start()
        {
            control = GetComponent<CharacterController>();
            anim = GetComponent<Animator>();
            stats = GetComponent<PlayerStats>();
            stats.UpdateStats();

            bulletPool = new ObjectPool(
                bulletPrefab,
                (b) => { var bullet = b.GetComponent<Bullet>(); bullet.shooterTag = "Player"; },
                30
                );
        }

        // Update is called once per frame
        void Update()
        {
            HandleMovement();
            HandleShooting();
        }

        void HandleMovement()
        {
            move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (move.sqrMagnitude > 1)
            {
                //This ensures that diagonal movement is just as fast as orthogonal
                move.Normalize();
            }

            control.Move(move * stats.attributes.MovementSpeed * Time.deltaTime);

            if (move.sqrMagnitude != 0)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move, Vector3.up), Time.deltaTime * stats.attributes.MovementSpeed);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, chestBone.rotation, Time.deltaTime * stats.attributes.MovementSpeed);
            }

            Vector3 localMove = transform.InverseTransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));

            anim.speed = stats.attributes.MovementSpeed / animationBaseSpeed;
            anim.SetFloat("X", localMove.x, 0, Time.deltaTime);
            anim.SetFloat("Y", localMove.z, 0, Time.deltaTime);
        }


        void HandleShooting()
        {
            Plane playerPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float hitdist = 0.0f;
            if (playerPlane.Raycast(ray, out hitdist))
            {
                Vector3 hitPos = ray.GetPoint(hitdist);
                lookDirection = hitPos - chestBone.transform.position;
                lookDirection.y = 0;
            }

            if (Input.GetMouseButton(0))
            {
                anim.SetBool("Attack", true);
                anim.SetFloat("AttackSpeed", stats.attributes.AttackSpeed);
                
            }
            else
            {
                anim.SetBool("Attack", false);
            }
        }

        //This gets called by animation event.
        void Attack()
        {
            if (!anim.IsInTransition(2) && stats.CanAttack())
            {
                stats.Energy -= stats.attributes.AttackEnergyCost;

                var bullet = bulletPool.Spawn(gunBone.position, lookDirection + gunBone.position);
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * stats.attributes.BulletVelocity;
                bullet.GetComponent<Bullet>().damage = stats.attributes.AttackDamage;
            }
        }

        void LateUpdate()
        {   //This has to be done in lateUpdate to properly ovverride currently playing animation
            chestBone.transform.LookAt(lookDirection + chestBone.transform.position, Vector3.up);
        }
    }
}