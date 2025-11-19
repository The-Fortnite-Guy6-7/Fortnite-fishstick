using UnityEngine;
using System.Collections;
using System; // Make sure this is present if you use Action delegates elsewhere

namespace Assets.FPS.Scripts
{
    public class NPCSpawner : MonoBehaviour
    {
        public GameObject npcPrefab;
        public Transform spawnPoint;
        public float respawnTime = 5f;

        public void StartRespawn()
        {
            _ = StartCoroutine(RespawnRoutine());
        }

        IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnTime);
            SpawnNPC();
        }

        void SpawnNPC()
        {
            // Check if references are assigned before trying to use them
            if (npcPrefab != null && spawnPoint != null)
            {
                GameObject newNPC = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
                if (newNPC.TryGetComponent<NPCHealth>(out NPCHealth newNPCHealth))
                {
                    newNPCHealth.spawner = this;
                }
            }
            else
            {
                // This will stop the game from potentially crashing with a NullReferenceException
                // and display a clear error message in the console.
                Debug.LogError("NPC Prefab or Spawn Point not assigned in the Inspector!");

                // Optional: Stop the script from running further if a critical error occurs
                // this.enabled = false; 
            }
        }

        void Start()
        {
            SpawnNPC();
        }
    }
}
