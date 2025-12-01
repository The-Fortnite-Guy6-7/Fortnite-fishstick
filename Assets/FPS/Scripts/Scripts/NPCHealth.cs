using UnityEngine;
using System; // Added for the Action delegate

namespace Assets.FPS.Scripts
{
    public class NPCHealth : MonoBehaviour
    {
        // Renamed 'health' from original script for clarity in this example
        // Using int type for consistency with the initial code snippet provided
        public int maxHealth = 100;
        private int currentHealth;

        // This reference is set by the NPCSpawner when the NPC is created.
        public NPCSpawner spawner { get; internal set; }

        void Start()
        {
            // Initialize health when the NPC spawns
            currentHealth = maxHealth;
        }

        // Changed parameter type to int for consistency with the provided code snippet
        public void TakeDamage(int amount)
        {
            // Optional: Prevent processing damage if already dead
            if (currentHealth <= 0)
            {
                return;
            }

            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                // Pass the notification method to the Die function
                Die(NotifySpawnerOfDeath);
            }
        }

        // Method that the Die function will invoke
        private void NotifySpawnerOfDeath()
        {
            // We call StartRespawn on the assigned spawner instance
            if (spawner != null)
            {
                spawner.StartRespawn();
            }
            else
            {
                // Log an error if spawner wasn't assigned properly
                Debug.LogError("Spawner reference missing on this NPC!");
            }
        }

        // The 'Die' method accepts an Action delegate
        void Die(Action respawnAction)
        {
            // Invoke the passed action (which is NotifySpawnerOfDeath)
            respawnAction?.Invoke();

            // Add death effects here (e.g., animations, particle effects, sounds)
            Debug.Log(gameObject.name + " has died!");

            // Destroy the current NPC instance
            Destroy(gameObject);
        }
    }
}
