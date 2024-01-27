using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorMovement : MonoBehaviour
{
    public float moveRange = 20f;
    public float moveInterval = 10f;
    public float moveRandInterval = 2f;
    public float minMoveSpeed = 3f;
    public float maxMoveSpeed = 7f;
    public float acceleration = 0.5f;

    private float moveSpeed;
    private float currentSpeed = 0f;
    private float yPos;
    private Vector3 targetPosition;
    private Rigidbody rb;

    void Start()
    {
        yPos = transform.position.y;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        PickNewTarget();
        InvokeRepeating("PickNewTarget", moveInterval, Random.Range(moveInterval - moveRandInterval, moveInterval + moveRandInterval));

        // Set a random color
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(Random.value, Random.value, Random.value);
        }
    }

    void Update()
    {
        float step = currentSpeed * Time.deltaTime;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        RaycastHit hit;
        // Add a buffer distance to stop before the actual point of collision
        float bufferDistance = Random.Range(0.5f, 1f);
        if (Physics.Raycast(transform.position, moveDirection, out hit, step + bufferDistance))
        {
            PickNewTarget();
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // Accelerate when moving towards the target, decelerate when reached the target
        if (transform.position != targetPosition)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, moveSpeed);
        }
        else
        {
            currentSpeed = Mathf.Max(currentSpeed - acceleration * Time.deltaTime, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        print("Triggered with " + other.gameObject.name);
        PickNewTarget();
    }

    void PickNewTarget()
    {
        print("Picking new target");
        targetPosition = new Vector3(
            transform.position.x + Random.Range(-moveRange, moveRange),
            yPos,
            transform.position.z + Random.Range(-moveRange, moveRange)
        );
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed); // Pick a random move speed
        currentSpeed = 0; // Reset speed when a new target is picked
    }
}