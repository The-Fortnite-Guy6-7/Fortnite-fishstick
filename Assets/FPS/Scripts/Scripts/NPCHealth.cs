using UnityEngine;
using System;

namespace Assets.FPS.Scripts
{
    public class NPCHealth : MonoBehaviour
    {
        public int maxHealth = 100;
        private int currentHealth;
        public NPCSpawner spawner { get; internal set; }

        void Start()
        {
            currentHealth = maxHealth;
        }

        // Method to apply a specific amount of damage
        public void TakeDamage(int amount)
        {
            if (currentHealth <= 0)
            {
                return;
            }

            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                Die(NotifySpawnerOfDeath);
            }
        }

        // NEW METHOD: Apply exactly 10 HP of damage
        public void Take10Damage()
        {
            // Call the existing TakeDamage method with a fixed value of 10
            TakeDamage(10);
        }

        private void NotifySpawnerOfDeath()
        {
            if (spawner != null)
            {
                spawner.NotifyDeath();
            }
            else
            {
                Debug.LogError("Spawner reference missing on this NPC!");
            }
        }

        void Die(Action respawnAction)
        {
            respawnAction?.Invoke();
            Debug.Log(gameObject.name + " has died!");
            Destroy(gameObject);
        }
    }
}
