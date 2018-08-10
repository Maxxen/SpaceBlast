using Assets.Scripts.Enemy;
using Assets.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour, IDamageable {

    public EnemyAttributes attributes;
    public int Health { get; private set; }

    GameController gameController;
    Renderer render;

    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        render = GetComponentInChildren<Renderer>();
    }

    private void OnEnable()
    {
        attributes.RecalculateStats();

        Health = attributes.MaxHealth;
        GetComponent<NavMeshAgent>().speed = attributes.MovementSpeed;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log(damage);
        this.Health -= damage;
        if (Health <= 0)
        {
            //Die
            gameController.IncreaseCombo();
            gameController.AddExp(attributes.ExpReward);
            this.gameObject.SetActive(false);
        }
        else
        {
            //Animate flash effect
            StartCoroutine(Flash());
        }
    }

    IEnumerator Flash()
    {
        render.material.SetFloat("_FlashAmount", 0.5f);
        yield return new WaitForSeconds(0.1f);
        render.material.SetFloat("_FlashAmount", 0);
    }
}
