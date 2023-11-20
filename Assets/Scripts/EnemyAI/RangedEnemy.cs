using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public float attackRange;
    private Transform playerTransform;
    private readonly List<Vector3> destinations = new();
    private readonly float timeToReload = 5f;
    private bool isRunningAway = false;
    private bool bulletReady = false;
    public GameObject projectileToFire;
    private Vector3 firePosition;
    // Start is called before the first frame update
    void Start()
    {
        firePosition = this.GetComponent<Transform>().Find("FirePosition").position;
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
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

        if (!isRunningAway)
        {
            Vector3 predictedPosition = playerTransform.position;
            Vector3 playerVelocity = playerTransform.GetComponent<Rigidbody>().velocity;

            playerVelocity *= Time.deltaTime;
            predictedPosition += playerVelocity;

            transform.LookAt(predictedPosition);
        }
    }

    private void SetNewDestinations()
    {
        
        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange * 1.5f)
        {
            isRunningAway = true;
            destinations.Add(transform.position - ((Quaternion.AngleAxis(Random.Range(0, 179), Vector3.up) * (playerTransform.position - transform.position).normalized)) * 10f);
        }
        else
        {
            isRunningAway = false;
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
        if (bulletReady && !isRunningAway)
        {
            GameObject firedProjectile = Instantiate(projectileToFire, firePosition, transform.rotation);
            firedProjectile.GetComponent<Rigidbody>().velocity = Vector3.forward * 2f;
        }
    }

    private IEnumerator PrepareBullet()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (!bulletReady)
            {
                yield return new WaitForSecondsRealtime(timeToReload);
                bulletReady = true;
            }
        }
    }
}