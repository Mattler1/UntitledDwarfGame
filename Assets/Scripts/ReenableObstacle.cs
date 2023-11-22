using UnityEngine;
using UnityEngine.AI;

public class ReenableObstacle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            gameObject.GetComponent<NavMeshObstacle>().enabled = true;
        }
    }
}
