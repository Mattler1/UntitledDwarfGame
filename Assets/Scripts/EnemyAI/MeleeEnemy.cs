using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class MeleeEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public Vector3 lastRememberedPosition = Vector3.zero;
    private Transform playerTransform;
    public EnemyProperties properties;
    private Rigidbody rb;
    private Collider hitbox;
    // Start is called before the first frame update
    void Start()
    {
        properties.canBeGrabbed = false;
        properties.isGrabbed = false;
        properties.toDestroy = false;
        hitbox = transform.GetChild(1).gameObject.GetComponent<Collider>();
        hitbox.enabled = false;
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

            if (hit.distance <= 1.5f)
            {
                hitbox.enabled = true;
            }
            else
            {
                hitbox.enabled = false;
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
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f))
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