using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public Vector3 lastRememberedPosition = Vector3.zero;
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(this.GetComponent<Transform>().position, (playerTransform.position - this.GetComponent<Transform>().position).normalized, out RaycastHit hit, Mathf.Infinity))
        {
            agent.destination = Vector3.zero;
            if (hit.transform.gameObject.CompareTag("Player")) {
                agent.SetDestination(playerTransform.position);
                lastRememberedPosition = playerTransform.position;
            } else if (lastRememberedPosition != Vector3.zero) {
                agent.SetDestination(lastRememberedPosition);
                lastRememberedPosition = Vector3.zero;
            }
        }
    }
}