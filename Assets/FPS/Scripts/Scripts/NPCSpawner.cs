using UnityEngine;
using System.Collections;
using System;

namespace Assets.FPS.Scripts
{
    public class NPCSpawner : MonoBehaviour
    {
        public GameObject npcPrefab;
        public Transform spawnPoint;
        // This is now the time to wait AFTER a death has occurred
        public float respawnDelay = 5.0f;

        // Removed the Coroutine reference variable as we use a one-shot approach now

        void Start()
        {
            // Spawn the very first NPC immediately when the game starts
            SpawnNPC();
        }

        // This method is called by the NPC's death script (NPCHealth.cs)
        // It starts the single wait-and-spawn cycle.
        public void NotifyDeath()
        {
            // Start the waiting routine upon receiving the death notification
            StartCoroutine(RespawnRoutine());
        }

        // The one-shot routine: waits then spawns one NPC
        IEnumerator RespawnRoutine()
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(respawnDelay);

            // Once the wait is over, spawn the single replacement NPC
            SpawnNPC();
        }

        void SpawnNPC()
        {
            if (npcPrefab != null && spawnPoint != null)
            {
                GameObject newNPC = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
                if (newNPC.TryGetComponent<NPCHealth>(out NPCHealth newNPCHealth))
                {
                    // Pass the spawner reference to the new NPC
                    newNPCHealth.spawner = this;
                }
            }
            else
            {
                Debug.LogError("NPC Prefab or Spawn Point not assigned in the Inspector!");
                this.enabled = false; // Disable the spawner if setup is wrong
            }
        }
    }
}
