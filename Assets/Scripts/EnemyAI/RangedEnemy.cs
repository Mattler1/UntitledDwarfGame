using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    public float attackRange;
    private readonly Transform playerTransform = GameObject.Find("PlayerPrefab").GetComponent<Transform>();
    private List<Vector3> destinations = new();
    private readonly int timeBetweenShots = 5;
    private bool isRunningAway = false;
    private bool bulletReady = false;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        StartCoroutine(PrepareBullet());
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, (playerTransform.position - transform.position).normalized, out RaycastHit hit, attackRange))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                // I need a model for our chosen projectile.
                FireAtPlayer();
            }
        }
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            if (destinations.Count <= 0)
            {
                SetNewDestinations();
                ChooseNewDestination();
            }
            else
            {
                ChooseNewDestination();
            }
        }
    }

    private void SetNewDestinations()
    {
        
        if (Vector3.Distance(transform.position, playerTransform.position) <= 10f)
        {
            destinations.Add(transform.position - ((Quaternion.AngleAxis(Random.Range(0, 179), Vector3.up) * (playerTransform.position - transform.position).normalized)) * 10f);
        }
        else
        {
            destinations.Add(Vector3.Reflect(transform.position, (Vector3.left * 2f)));
            destinations.Add(Vector3.Reflect(transform.position, (Vector3.right * 2f)));
        }
    }

    private void ChooseNewDestination()
    {
        Vector3 chosenDestination = destinations[0];
        agent.destination = chosenDestination;
        destinations.Remove(chosenDestination);
    }

    private void FireAtPlayer()
    {
        Vector3 predictedPosition = playerTransform.position;
        float velocity = playerTransform.GetComponent<Rigidbody>().velocity;

        velocity *= Time.deltaTime;
        predictedPosition += velocity;

        if (bulletReady)
        {
            //Wait for a projectile object to be available to fire before finishing this.
            return;
        }
    }

    private IEnumerator PrepareBullet()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (!bulletReady)
            {
                yield return new WaitForSecondsRealtime(5f);
                bulletReady = true;
            }
        }
    }
}