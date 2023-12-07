using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerActions : MonoBehaviour
{
    [HideInInspector]
    public float moveSpeed = 5f;
    [HideInInspector]
    public float rotationSpeed = 2f;
    public LayerMask layermask;
    private Camera playerCamera;
    private Rigidbody rb;
    private Transform playerTransform;
    private bool canJump = false;
    private readonly float throwForce = 20f;

    private int hitsTaken = 0;
    private readonly int maxHits = 5;
    private float mouseX = 0f;
    private float mouseY = 0f;

    private GameObject victim;
    [HideInInspector]
    public Transform holdPosition;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        playerTransform = GetComponent<Transform>();
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);

        playerTransform.rotation = Quaternion.Euler(0f, mouseX, 0f);
        playerCamera.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0f);

        Vector3 moveVelocity = transform.TransformDirection(movementDirection) * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

        if (Input.GetKeyDown(KeyCode.Space) && canJump) {
            rb.velocity = new Vector3(rb.velocity.x, 5f, rb.velocity.z);
            canJump = false;
        }

        if (victim != null)
        {
            victim.transform.SetPositionAndRotation(holdPosition.transform.position, playerTransform.rotation);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ThrowObject();
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUpObject();
            }
        }
    }

    private void PickUpObject() {
        if (victim == null)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, 5f, layermask))
            {
                victim = hit.collider.gameObject;
                if (victim.transform != playerTransform)
                {
                    if (victim.CompareTag("Throwable"))
                    {
                        Grab();
                    }
                    else if (victim.CompareTag("Enemy"))
                    {
                        if (victim.TryGetComponent(out MeleeEnemy meleeScript))
                        {
                            if (meleeScript.properties.canBeGrabbed)
                            {
                                meleeScript.properties.isGrabbed = true;
                                Grab();
                            }
                        }
                        else if (victim.TryGetComponent(out RangedEnemy rangedScript))
                        {
                            if (rangedScript.properties.canBeGrabbed)
                            {
                                rangedScript.properties.isGrabbed = true;
                                Grab();
                            }
                        }
                    }
                }
            }
        }
    }

    private void Grab()
    {
        victim.transform.parent = holdPosition.transform;
        victim.transform.rotation = playerTransform.rotation;
        victim.GetComponent<Collider>().enabled = false;
    }

    private void ThrowObject()
    {
        if (victim != null)
        {
            victim.GetComponent<Collider>().enabled = true;
            victim.transform.parent = null;
            if (victim.TryGetComponent(out NavMeshObstacle obstacle))
            {
                obstacle.enabled = false;
            }
            if (victim.TryGetComponent(out MeleeEnemy meleeScript))
            {
                meleeScript.properties.isGrabbed = false;
                meleeScript.properties.toDestroy = true;
            }
            else if (victim.TryGetComponent(out RangedEnemy rangedScript))
            {
                rangedScript.properties.isGrabbed = false;
                rangedScript.properties.toDestroy = true;
            }
            victim.GetComponent<Rigidbody>().velocity = playerCamera.transform.forward * throwForce;
            victim = null;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Throwable")) 
        {
            canJump = true;
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.TryGetComponent(out MeleeEnemy meleeScript))
            {
                if (meleeScript.properties.canBeGrabbed)
                {
                    canJump = true;
                }
            }
            else if (other.gameObject.TryGetComponent(out RangedEnemy rangedScript))
            {
                if (rangedScript.properties.canBeGrabbed)
                {
                    canJump = true;
                }
            }
        }
        else if (other.gameObject.CompareTag("Projectile"))
        {
            TakeHit();
            Object.Destroy(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hitbox"))
        {
            TakeHit();
        }
    }

    private void TakeHit()
    {
        hitsTaken += 1;
        Debug.Log("Hit taken");

        if (hitsTaken >= maxHits)
        {
            Debug.Log("YOU HAVE NOW FACED SAM WATKINS' FATE. GOODBYE.");
            //We need some way to end the player's life.
        }
    }
}
