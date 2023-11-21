using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public Vector3 lastRememberedPosition = Vector3.zero;
    private Transform playerTransform;
    private EnemyProperties properties;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        properties = GetComponent<EnemyProperties>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, (playerTransform.position - transform.position).normalized, out RaycastHit hit, Mathf.Infinity) && properties.isActive)
        {
            if (hit.transform.gameObject.CompareTag("Player")) {
                agent.SetDestination(playerTransform.position);
                lastRememberedPosition = playerTransform.position;
            } else if (lastRememberedPosition != Vector3.zero) {
                agent.SetDestination(lastRememberedPosition);
                lastRememberedPosition = Vector3.zero;
            }
        }

        if (!properties.isActive)
        {
            agent.ResetPath();
            lastRememberedPosition = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Throwable") && other.gameObject.GetComponent<Rigidbody>().velocity != Vector3.zero)
        {
            properties.isActive = false;
        }
    }
}