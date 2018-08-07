using Assets.Scripts.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatStats : MonoBehaviour, IDamageable {

    public int Health { get; private set; }
    public int MeleeDamage { get; private set; }
    public int ProjectileDamage { get; private set; }
    public float AttackSpeed { get; private set; }
    public float ProjectileSpeed { get; private set; }
    public float ProjectileSize { get; private set; }
    public float MovementSpeed { get; private set; }

    public EnemyCombatSkills skills;

    float nextAttack;

    Renderer render;
    public void SetStats()
    {
        //Health = skills.MaxHealth;
    }

    private void Start()
    {
        render = GetComponentInChildren<Renderer>();
        Health = 20;
        ProjectileSpeed = 10;
        AttackSpeed = 2.5f;
    }

    //public bool CanAttack()
    //{
    //    if(Time.time > AttackSpeed)
    //    {
    //        nextAttack = Time.time + AttackSpeed;
    //        return true;
    //    }
    //    return false;            
    //}

    public void TakeDamage(int damage)
    {
        Debug.Log(damage);
        this.Health -= damage;
        if (Health <= 0)
        {
            OnDeath();
        }
        else
        {
            OnDamage();
        }
    }

    private void OnDeath()
    {
        GameManagerScript.IncreaseCombo();
        this.gameObject.SetActive(false);
    }

    private void OnDamage()
    {
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        render.material.SetFloat("_FlashAmount", 0.5f);
        yield return new WaitForSeconds(0.1f);
        render.material.SetFloat("_FlashAmount", 0);
    }
}
