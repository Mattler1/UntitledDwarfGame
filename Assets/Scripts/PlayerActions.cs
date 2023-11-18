using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;
    public LayerMask layermask;
    private Camera playerCamera;
    private Rigidbody rb;
    private Transform playerTransform;
    private bool canJump = false;
    private readonly float throwForce = 20f;

    private float mouseX = 0f;
    private float mouseY = 0f;

    private GameObject victim;
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
                if (hit.collider.gameObject.CompareTag("Throwable"))
                {
                    victim = hit.collider.gameObject;
                    if (victim.transform != playerTransform)
                    {
                        victim.transform.parent = holdPosition.transform;
                        victim.transform.rotation = playerTransform.rotation;
                        Physics.IgnoreCollision(victim.GetComponent<Collider>(), playerTransform.GetComponentInParent<Collider>(), true);
                    }
                }
            }
        }
    }

    private void ThrowObject()
    {
        if (victim != null)
        {
            Physics.IgnoreCollision(victim.GetComponent<Collider>(), playerTransform.GetComponentInParent<Collider>(), false);
            victim.transform.parent = null;
            victim.GetComponent<Rigidbody>().velocity = playerCamera.transform.forward * throwForce;
            victim = null;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Throwable")) {
            canJump = true;
        }
    }
}
