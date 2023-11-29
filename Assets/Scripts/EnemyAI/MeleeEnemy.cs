using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public Vector3 lastRememberedPosition = Vector3.zero;
    private Transform playerTransform;
    private EnemyProperties properties;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        properties = GetComponent<EnemyProperties>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, (playerTransform.position - transform.position).normalized, out RaycastHit hit, Mathf.Infinity) && !properties.canBeGrabbed && agent.enabled)
        {
            if (hit.transform.gameObject.CompareTag("Player")) {
                agent.SetDestination(playerTransform.position);
                lastRememberedPosition = playerTransform.position;
            } else if (lastRememberedPosition != Vector3.zero) {
                agent.SetDestination(lastRememberedPosition);
                lastRememberedPosition = Vector3.zero;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Throwable") && other.gameObject.GetComponent<Rigidbody>().velocity != Vector3.zero)
        {
            agent.enabled = false;
            properties.canBeGrabbed = true;
            lastRememberedPosition = Vector3.zero;
            rb.constraints = RigidbodyConstraints.None;
            StartCoroutine(ReenableCharacter());
        }
    }
    private IEnumerator ReenableCharacter()
    {
        if (!properties.isGrabbed && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f))
        {
            if (hit.transform.gameObject.CompareTag("Floor"))
            {
                yield return new WaitForSecondsRealtime(6.5f);
                rb.constraints = RigidbodyConstraints.FreezePositionY;
                agent.enabled = true;
                properties.canBeGrabbed = false;
                StopCoroutine(ReenableCharacter());
            }
        }
        else
        {
            yield return null;
        }
    }
}