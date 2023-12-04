using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateEnemy : MonoBehaviour
{
    public List<GameObject> enemiesToSpawn;
    public List<Transform> enemySpawnPositions;
    private Rigidbody playerBody;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
    }
}
