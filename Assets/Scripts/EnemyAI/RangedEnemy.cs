using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    public float attackRange;
    private readonly Transform playerTransform = GameObject.Find("PlayerPrefab").GetComponent<Transform>();
    private List<Vector3> destinations = new();
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(this.GetComponent<Transform>().position, (playerTransform.position - this.GetComponent<Transform>().position).normalized, out RaycastHit hit, attackRange))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                
            }
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNewDestination();
        }
    }

    private void SetNewDestination()
    {
        destinations.Add(this.GetComponent<Transform>().position - new Vector3(Random.Range(1f, 3f), 0f, Random.Range(1f, 3f)));
        destinations.Add(this.GetComponent<Transform>().position - new Vector3(Random.Range(1f, 3f), 0f, Random.Range(1f, 3f)));
        destinations.Add(this.GetComponent<Transform>().position + new Vector3(Random.Range(1f, 3f), 0f, Random.Range(1f, 3f)));
        destinations.Add(this.GetComponent<Transform>().position + new Vector3(Random.Range(1f, 3f), 0f, Random.Range(1f, 3f)));
    }
}