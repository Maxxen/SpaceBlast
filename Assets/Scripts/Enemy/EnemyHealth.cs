using Assets.Scripts.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable {

    public EnemyStats stats;
    public int Health { get; private set; }

    GameController gameController;
    Renderer render;

    private void Start()
    {
        gameController = GameObject.Find("UI").GetComponent<GameController>();
        render = GetComponentInChildren<Renderer>();
    }

    private void OnEnable()
    {
        Health = stats.MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log(damage);
        this.Health -= damage;
        if (Health <= 0)
        {
            //Die
            gameController.IncreaseCombo();
            gameController.AddScore(stats.ScoreReward);
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
