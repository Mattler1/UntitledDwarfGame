using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class GrapplerFishBehaviour : MonoBehaviour
{
    private NavMeshAgent agent;
    public Vector3 lastRememberedPosition = Vector3.zero;
    private Transform playerTransform;
    public EnemyProperties properties;
    private Rigidbody rb;
    [HideInInspector]
    public Collider grabBox;
    // Start is called before the first frame update
    void Start()
    {
        properties.canBeGrabbed = false;
        properties.isGrabbed = false;
        properties.toDestroy = false;
        grabBox = transform.GetChild(3).gameObject.GetComponent<Collider>();
        grabBox.enabled = false;
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, (playerTransform.position - transform.position).normalized, out RaycastHit hit, Mathf.Infinity) && !properties.canBeGrabbed && agent.enabled)
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                agent.SetDestination(playerTransform.position);
                lastRememberedPosition = playerTransform.position;
            }
            else if (lastRememberedPosition != Vector3.zero)
            {
                agent.SetDestination(lastRememberedPosition);
                lastRememberedPosition = Vector3.zero;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (properties.toDestroy)
        {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Throwable") && other.gameObject.GetComponent<Rigidbody>().velocity != Vector3.zero && agent.enabled)
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
            else if (other.gameObject.TryGetComponent(out GrapplerFishBehaviour fishScript))
            {
                if (fishScript.properties.toDestroy)
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
        lastRememberedPosition = Vector3.zero;
        rb.constraints = RigidbodyConstraints.None;
        StartCoroutine(ReenableCharacter());
    }

    private IEnumerator ReenableCharacter()
    {
        yield return new WaitForSeconds(6.5f);
        yield return new WaitUntil(() => !properties.isGrabbed);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.3f))
        {
            if (hit.transform.gameObject.CompareTag("Floor"))
            {
                properties.canBeGrabbed = false;
                rb.constraints = RigidbodyConstraints.FreezePositionY;
                agent.enabled = true;
                StopCoroutine(ReenableCharacter());
            }
        }
        else
        {
            yield return null;
        }
    }
}
