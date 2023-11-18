using UnityEngine;

public class Health : MonoBehaviour
{
    private float health;
    public float maxHealth;
    private bool isPlayer;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        if (this.gameObject.CompareTag("Player"))
        {
            isPlayer = true;
        }
        else
        {
            isPlayer = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isPlayer)
        {
            if (collision.collider.CompareTag("Projectile") || collision.collider.CompareTag("Enemy"))
            {
                //health -= collision.collider.GetComponent<Damage>().damage;
                return;
            }
        }
    }
}
