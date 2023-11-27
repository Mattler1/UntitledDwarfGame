using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public float runDistance;
    private Transform playerTransform;
    private readonly List<Vector3> destinations = new();
    private readonly float timeToReload = 5f;
    private bool isRunningAway = false;
    private bool bulletReady = false;
    public GameObject projectileToFire;
    private Transform firePosition;
    private Rigidbody rb;
    private EnemyProperties properties;
    // Start is called before the first frame update
    void Start()
    {
        firePosition = transform.Find("FirePosition");
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        StartCoroutine(PrepareBullet());
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        properties = GetComponent<EnemyProperties>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.enabled)
        {
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
        else
        {
            StartCoroutine(ReenableCharacter());
        }
    }

    private void SetNewDestinations()
    {
        
        if (Vector3.Distance(transform.position, playerTransform.position) <= runDistance * 1.5f)
        {
            isRunningAway = true;
            destinations.Clear();
            destinations.Add(transform.position - ((Quaternion.AngleAxis(Random.Range(0, 179), Vector3.up) * (playerTransform.position - transform.position).normalized)) * 10f);
        }
        else
        {
            isRunningAway = false;
            destinations.Add(Vector3.Reflect(transform.position, (transform.right * -2f)));
            destinations.Add(Vector3.Reflect(transform.position, (transform.right * 2f)));
        }
    }

    private void ChooseNewDestination()
    {
        Vector3 chosenDestination = destinations[0];
        agent.destination = chosenDestination;
        destinations.Remove(chosenDestination);
    }

    private IEnumerator PrepareBullet()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (agent.enabled)
            {
                if (!bulletReady)
                {
                    yield return new WaitForSecondsRealtime(timeToReload);
                    bulletReady = true;
                }
                else if (!isRunningAway)
                {
                    bulletReady = false;
                    GameObject firedProjectile = Instantiate(projectileToFire, firePosition.position, transform.rotation);
                    firedProjectile.GetComponent<Rigidbody>().velocity = transform.forward * 10f;
                    yield return new WaitForSecondsRealtime(8f);
                    //This gives an error that IsDestroyed() doesn't exist, find a replacement for finding if the projectile is already destroyed.
                    //if (!firedProjectile.IsDestroyed())
                    //{
                    //    Object.Destroy(firedProjectile);
                    //}
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Throwable") && other.gameObject.GetComponent<Rigidbody>().velocity != Vector3.zero)
        {
            agent.enabled = false;
            properties.canBeGrabbed = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    private IEnumerator ReenableCharacter()
    {
        if (!properties.isGrabbed)
        {
            yield return new WaitForSecondsRealtime(6.5f);
            agent.enabled = true;
            properties.canBeGrabbed = false;
            StopCoroutine(ReenableCharacter());
        }
        else
        {
            yield return null;
        }
    }
}