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
        if (Physics.Raycast(transform.position, (playerTransform.position - transform.position).normalized, out RaycastHit hit, Mathf.Infinity) && !properties.canBeGrabbed)
        {
            if (hit.transform.gameObject.CompareTag("Player")) {
                agent.SetDestination(playerTransform.position);
                lastRememberedPosition = playerTransform.position;
            } else if (lastRememberedPosition != Vector3.zero) {
                agent.SetDestination(lastRememberedPosition);
                lastRememberedPosition = Vector3.zero;
            }
        }
        if (!agent.enabled)
        {
            StartCoroutine(ReenableCharacter());
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
        }
    }
    private IEnumerator ReenableCharacter()
    {
        if (!properties.isGrabbed && Physics.Raycast(transform.position, Vector3.down, 1f, LayerMask.GetMask("Default")))
        {
            yield return new WaitForSecondsRealtime(6.5f);
            agent.enabled = true;
            properties.canBeGrabbed = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            StopCoroutine(ReenableCharacter());
        }
        else
        {
            yield return null;
        }
    }
}