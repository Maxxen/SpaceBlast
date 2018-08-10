using Assets.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    class PlayerStats : MonoBehaviour, IDamageable
    {

        public float Health { get { return health; } set { health = value; healthSlider.value = value; } }
        public float Energy { get { return energy; } set { energy = value; energySlider.value = value; } }

        public GameController gameController;
        public PlayerAttributes attributes;

        public Slider healthSlider;
        public Slider energySlider;

        float health;
        float energy;

        Renderer render;

        void Start()
        {
            attributes.RecalculateStats();
            attributes.AttributeChangeHandler += OnAttributeChange;
            render = GetComponentInChildren<Renderer>();
        }

        void Update()
        {
            StartCoroutine(Regeneration());
        }

        private void OnAttributeChange()
        {
            healthSlider.maxValue = attributes.MaxHealth;
            energySlider.maxValue = attributes.MaxEnergy;

            Health = attributes.MaxHealth;
            Energy = attributes.MaxEnergy;
        }

        private IEnumerator Regeneration()
        {
            yield return new WaitForSeconds(1);
            if (Health != attributes.MaxHealth)
                Health += attributes.HealthRegen;
            if (Energy != attributes.MaxEnergy)
                Energy += attributes.EnergyRegen;
            yield break;
        }

        public bool CanAttack()
        {
            return Energy > attributes.AttackEnergyCost;
        }

        public void TakeDamage(int damage)
        {
            this.Health -= damage;

            if (Health <= 0)
            {
                gameController.GotoGameOver();
            }
            else
            {
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
}