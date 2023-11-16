using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    public Vector3 lastRememberedPosition = Vector3.zero;
    private Transform playerTransform = GameObject.Find("PlayerPrefab").GetComponent<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.GetComponent<Transform>().position, (playerTransform.position - this.GetComponent<Transform>().position).normalized, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.CompareTag("Player")) {
                agent.destination = playerTransform.position;
                lastRememberedPosition = playerTransform.position;
            } else if (lastRememberedPosition != Vector3.zero) {
                agent.destination = lastRememberedPosition;
                lastRememberedPosition = Vector3.zero;
            }
        }
    }
}