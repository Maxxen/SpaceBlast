using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatStats : MonoBehaviour {

    public int HealthMax { get; private set; }
    public float HealthRegen { get; private set; }
    public int EnergyMax { get; private set; }
    public float EnergyRegen { get; private set; }

    public float MovementSpeed { get; private set; }

    public float ShootingEnergyCost { get; private set; }
    public int ShootingDamage { get; private set; }
    public float ShootingSpeed { get; private set; }
    public float ShootingVelocity { get; private set; }

    const int BASE_HEALTH = 100;
    const int HEALTH_PER_LVL = 25;

    const float BASE_HEALTH_REGEN = 1f;
    const float HEALTH_REGEN_PER_LVL = 1.1f;

    const int BASE_ENERGY = 100;
    const int ENERGY_PER_LVL = 25;

    const float BASE_ENERGY_REGEN = 0.2f;
    const float ENERGY_REGEN_PER_LVL = 1.25f;

    const float BASE_MOVEMENT_SPEED = 7f;
    const float MOVEMENT_SPEED_PER_LVL = 1.5f;

    //Shoot energy cost depends on damage and shoot speed.
    const float BASE_SHOOT_COST = 10f;

    const int BASE_SHOOT_DAMAGE = 10;
    const int SHOOT_DAMAGE_PER_LVL = 10;

    const float BASE_SHOOT_SPEED = 0.8f;
    const float SHOOT_SPEED_PER_LVL = 0.2f;

    const float BASE_BULLET_VELOCITY = 8f;
    const float BULLET_VELOCITY_PER_LVL = 2f;

    int healthSkill = 0;
    int healthRegenSkill = 0;
    int energySkill = 0;
    int energyRegenSkill = 0;
    int movementSpeedSkill = 0;
    int shootingDamageSkill = 0;
    int shootingSpeedSkill = 0;
    int bulletVelocitySkill = 0;

    public float Health { get; private set; } 
    public float Energy { get { return energy; } private set { energy = value; EnergySlider.value = value; } }
    private float energy;


    Slider HealthSlider;
    Slider EnergySlider;

    public void RecalculateStats()
    {
        HealthMax = BASE_HEALTH + (HEALTH_PER_LVL * healthSkill);
        HealthRegen = BASE_HEALTH_REGEN * (HEALTH_REGEN_PER_LVL * healthRegenSkill);
        EnergyMax = BASE_ENERGY + (ENERGY_PER_LVL * energySkill);
        EnergyRegen = BASE_ENERGY_REGEN + (energyRegenSkill * ENERGY_PER_LVL);
        MovementSpeed = Mathf.Log(BASE_MOVEMENT_SPEED, MOVEMENT_SPEED_PER_LVL * (movementSpeedSkill + 1));
        ShootingDamage = BASE_SHOOT_DAMAGE + (SHOOT_DAMAGE_PER_LVL * shootingDamageSkill);
        ShootingSpeed = BASE_SHOOT_SPEED - (SHOOT_SPEED_PER_LVL * shootingSpeedSkill);
        ShootingVelocity = BASE_BULLET_VELOCITY + (BULLET_VELOCITY_PER_LVL * bulletVelocitySkill);

        ShootingEnergyCost = BASE_SHOOT_COST * (1 + shootingSpeedSkill + shootingDamageSkill + (0.5f * bulletVelocitySkill));

        HealthSlider = GameObject.Find("/UI/GameHUD/ResourceSliders/HealthSlider").GetComponent<Slider>();
        EnergySlider = GameObject.Find("/UI/GameHUD/ResourceSliders/EnergySlider").GetComponent<Slider>();

        HealthSlider.maxValue = HealthMax;
        EnergySlider.maxValue = EnergyMax;

        Energy = EnergyMax;
        Health = HealthMax;

    }

    public delegate void OnDamageCallback();
    public delegate void OnDeathCallback();

    public OnDamageCallback onDamage;
    public OnDeathCallback onDeath;

    private void Start()
    {
        HealthSlider = GameObject.Find("/UI/GameHUD/ResourceSliders/HealthSlider").GetComponent<Slider>();
        EnergySlider = GameObject.Find("/UI/GameHUD/ResourceSliders/EnergySlider").GetComponent<Slider>();

        RecalculateStats();
    }

    private void Update()
    {
        StartCoroutine(Regeneration());
    }

    bool regenCooldown = true;

    private IEnumerator Regeneration()
    {
        yield return new WaitForSeconds(1);
        if (Health != HealthMax)
            Health += HealthRegen;
        if(Energy != EnergyMax)
            Energy += EnergyRegen;
        yield break;
    }

    float nextAttack;
    public bool CanAttack()
    {
        if(Time.time > nextAttack && Energy > ShootingEnergyCost)
        {
            nextAttack = Time.time + ShootingSpeed;
            Energy -= ShootingEnergyCost;
            Energy = Mathf.Clamp(Energy, 0, EnergyMax);

            return true;
        }
        return false;            
    }

    public void TakeDamage(int damage)
    {
        this.Health -= damage;

        HealthSlider.value = Health;

        if(Health <= 0)
        {
            onDeath();
        }
        else
        {
            onDamage();
        }
    }
}
