using UnityEngine;

public class InstantiatePlayer : MonoBehaviour
{
    public GameObject player;
    public Transform spawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(player, spawnPosition.position, Quaternion.identity);
    }
}
