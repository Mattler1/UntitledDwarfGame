using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    /// <summary>
    /// roomEnemies and enemySpawns must be the same length.
    /// 
    /// Each object in roomEnemies will be spawned at the corresponding transform in enemySpawns.
    /// 
    /// WARNING: You will notice objects disappearing while using this script. Don't worry about it, they return
    /// after reloading the scene.
    /// </summary>
    public List<GameObject> roomEnemies;
    public List<Transform> enemySpawns;
    public List<GameObject> roomDoors;
    public Collider triggerObject;
    private GameObject player;
    private readonly Dictionary<GameObject, Transform> enemies;
    private readonly List<GameObject> livingEnemies;
    private bool encounterStarted = false;
    private bool encounterFinished = false;
    // Start is called before the first frame update
    void Start()
    {
        if (roomEnemies.Count != enemySpawns.Count)
        {
            Debug.LogError("roomEnemies and enemySpawns must be the same length!");
        }
        for (int i = 0; i < roomEnemies.Count; i++)
        {
            enemies.Add(roomEnemies[i], enemySpawns[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (enemies.Count <= 0)
        {
            Destroy(triggerObject);
            encounterStarted = true;
        }
        if (livingEnemies.Count <= 0)
        {
            encounterFinished = true;
        }
        if (!encounterStarted)
        {
            RaycastHit[] hits = player.GetComponent<Rigidbody>().SweepTestAll(player.transform.forward, 5f, QueryTriggerInteraction.Collide);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider == triggerObject)
                {
                    foreach (KeyValuePair<GameObject, Transform> kvp in enemies)
                    {
                        livingEnemies.Add(Instantiate(kvp.Key, kvp.Value.position, Quaternion.identity));
                        Destroy(kvp.Value.gameObject);
                    }
                    for (int j = 0; j < roomDoors.Count; j++)
                    {
                        roomDoors[j].SetActive(true);
                    }
                    enemies.Clear();
                }
            }
        }
        else if (encounterStarted)
        {
            for (int i = 0; i < livingEnemies.Count; i++)
            {
                if (!livingEnemies[i])
                {
                    livingEnemies.RemoveAt(i);
                    i--;
                }
            }
        }
        if (encounterFinished)
        {
            for (int i = 0; i < roomDoors.Count; i++)
            {
                roomDoors[i].SetActive(false);
            }
            Destroy(this);
        }
    }
}
