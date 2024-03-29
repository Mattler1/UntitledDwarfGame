using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class RangedEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private readonly float runDistance = 2f;
    private Transform playerTransform;
    private readonly List<Vector3> destinations = new();
    private readonly float timeToReload = 5f;
    private bool isRunningAway = false;
    private bool bulletReady = false;
    public GameObject projectileToFire;
    private Transform firePosition;
    private Rigidbody rb;
    public EnemyProperties properties;
    // Start is called before the first frame update
    void Start()
    {
        properties.isGrabbed = false;
        properties.canBeGrabbed = false;
        properties.toDestroy = false;
        firePosition = transform.Find("FirePosition");
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        StartCoroutine(PrepareBullet());
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY;
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
                if (transform.rotation.y < 0)
                {
                    predictedPosition.x += 0.65f;
                    predictedPosition.z += 1f;
                }
                else
                {
                    predictedPosition.x -= 0.65f;
                    predictedPosition.z -= 1f;
                }
                predictedPosition += playerVelocity;

                transform.LookAt(predictedPosition);
            }
        }
    }

    private void SetNewDestinations()
    {
        
        if (Vector3.Distance(transform.position, playerTransform.position) <= runDistance)
        {
            isRunningAway = true;
            destinations.Clear();
            destinations.Add(transform.position - ((Quaternion.AngleAxis(Random.Range(0, 179), Vector3.up) * (playerTransform.position - transform.position).normalized)) * 10f);
        }
        else
        {
            isRunningAway = false;
            destinations.Add(Vector3.Reflect(transform.position, (transform.right * -10f)));
            destinations.Add(Vector3.Reflect(transform.position, (transform.right * 10f)));
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
                    StartCoroutine(KillProjectile(firedProjectile));
                }
            }
            yield return null;
        }
    }

    private IEnumerator KillProjectile(GameObject projectile)
    {
        yield return new WaitForSecondsRealtime(8f);
        if (projectile)
        {
            Object.Destroy(projectile);
        }
        StopCoroutine(KillProjectile(projectile));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (properties.toDestroy)
        {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Throwable") && other.rigidbody.velocity != Vector3.zero)
        {
            TakeHit();
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.TryGetComponent(out MeleeEnemy meleeScript))
            {
                if (meleeScript.properties.toDestroy)
                {
                    TakeHit();
                }
            }
            else if (other.gameObject.TryGetComponent(out RangedEnemy rangedScript))
            {
                if (rangedScript.properties.toDestroy)
                {
                    TakeHit();
                }
            }
        }
    }

    private void TakeHit()
    {
        agent.enabled = false;
        properties.canBeGrabbed = true;
        rb.constraints = RigidbodyConstraints.None;
        StartCoroutine(ReenableCharacter());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hitbox"))
        {
            if (other.gameObject.name == "Grab")
            {
                agent.enabled = false;
                transform.parent = other.gameObject.transform;
                properties.toDestroy = true;
            }
        }
    }

    private IEnumerator ReenableCharacter()
    {
        yield return new WaitForSecondsRealtime(6.5f);
        yield return new WaitUntil(() => !properties.isGrabbed);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f))
        {
            if (hit.transform.gameObject.CompareTag("Floor"))
            {
                properties.canBeGrabbed = false;
                agent.enabled = true;
                rb.constraints = RigidbodyConstraints.FreezePositionY;
                StopCoroutine(ReenableCharacter());
            }
        }
        else
        {
            yield return null;
        }
    }
}