using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateEnemy : MonoBehaviour
{
    public List<GameObject> enemiesToSpawn;
    public List<Transform> enemySpawnPositions;
    public Rigidbody triggerObject;
    private Rigidbody playerBody;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
    }

    void Update()
    {
        RaycastHit[] hits = playerBody.SweepTestAll(playerBody.transform.forward, 5f, QueryTriggerInteraction.Collide);

        for (int i = 0; i < hits.Length; i++)
        {
            break;
        }
    }
}
