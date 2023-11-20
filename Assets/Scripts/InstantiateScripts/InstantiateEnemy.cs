using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform enemySpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(enemyPrefab, enemySpawnPosition.position, Quaternion.identity);
    }
}
