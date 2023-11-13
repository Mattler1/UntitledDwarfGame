using System.Collections;
using System.Collections.Generic;
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
    private float throwForce = 20f;

    private float mouseX = 0f;
    private float mouseY = 0f;
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
            rb.velocity = new Vector3(moveVelocity.x, 5f, moveVelocity.z);
            canJump = false;
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            UseObject();
        }
    }

    private void UseObject() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5f, layermask)) {
            Transform victim = hit.transform;
            if (victim.parent != playerTransform) {
                victim.SetParent(playerTransform, false);
                victim.GetComponent<Rigidbody>().useGravity = false;
                victim.position = playerTransform.position + playerTransform.forward * 3f;
            } else {
                victim.SetParent(null, true);
                victim.GetComponent<Rigidbody>().useGravity = true;
                victim.GetComponent<Rigidbody>().velocity = playerTransform.forward * throwForce;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor")) {
            canJump = true;
        }
    }
}
