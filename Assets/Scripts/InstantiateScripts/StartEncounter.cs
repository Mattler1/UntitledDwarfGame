using System.Collections.Generic;
using UnityEngine;

public class StartEncounter : MonoBehaviour
{
    public List<GameObject> enemiesToSpawn;
    public List<Transform> enemySpawnPositions;
    public Collider triggerObject;
    private GameObject player;

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        RaycastHit[] hits = player.GetComponent<Rigidbody>().SweepTestAll(player.transform.forward, 5f, QueryTriggerInteraction.Collide);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.isTrigger == true && hits[i].collider == triggerObject)
            {
                for (int j = 0; j < enemiesToSpawn.Count; j++)
                {
                    Instantiate(enemiesToSpawn[j], enemySpawnPositions[j].position, Quaternion.identity);
                    Destroy(enemySpawnPositions[j].gameObject);
                    enemiesToSpawn.RemoveAt(j);
                    enemySpawnPositions.RemoveAt(j);
                    j--;
                }
            }
        }

        if (enemiesToSpawn.Count <= 0 && enemySpawnPositions.Count <= 0)
        {
            Destroy(this);
        }
    }
}
