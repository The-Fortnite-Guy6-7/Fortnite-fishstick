using UnityEngine;
using System; // Added for the Action delegate

namespace Assets.FPS.Scripts
{
    public class NPCHealth : MonoBehaviour
    {
        // Renamed 'health' from original script for clarity in this example
        public float currentHealth = 10f;

        // This reference is set by the NPCSpawner when the NPC is created.
        // Using 'internal set' is fine, or revert to the original public field with [HideInInspector]
        public NPCSpawner spawner { get; internal set; }

        public void TakeDamage(float amount)
        {
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

            // Destroy the current NPC instance
            Destroy(gameObject);
        }

        /* 
         * The StartRespawn(), RespawnRoutine(), SpawnNPC(), Start(), 
         * npcPrefab, spawnPoint, and respawnTime variables were removed from here 
         * because they belong in the NPCSpawner script, not here.
         */
    }
}
