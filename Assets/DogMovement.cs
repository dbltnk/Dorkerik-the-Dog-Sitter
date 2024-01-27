using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMovement : MonoBehaviour
{
    public float minMoveRange = 2f;
    public float maxMoveRange = 6f;
    private float moveRange;

    public float moveInterval = 5f;
    public float moveRandInterval = 1f;
    public float minMoveSpeed = 4f;
    public float maxMoveSpeed = 6f;
    public float acceleration = 0.5f;

    private float moveSpeed;
    private float currentSpeed = 0f;
    private float yPos;
    private Vector3 targetPosition;
    private Rigidbody rb;

    public GameObject PoopPrefab;

    void Start()
    {
        yPos = transform.position.y;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        PickNewTarget();
        InvokeRepeating("PickNewTarget", moveInterval, Random.Range(moveInterval - moveRandInterval, moveInterval + moveRandInterval));

        // Scale the object randomly
        //float scaleX = Random.Range(0.4f, 0.6f);
        //float scaleY = Random.Range(0.4f, 0.6f);
        //float scaleZ = Random.Range(0.4f, 0.6f);
        //transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        // Set a random color
        Renderer renderer = transform.Find("Capsule").GetComponent<Renderer>();
        if (renderer != null)
        {
            float r = Random.Range(0.4f, 0.6f);
            float g = Random.Range(0.2f, 0.3f);
            float b = 0;
            renderer.material.color = new Color(r, g, b);
        }

        InvokeRepeating("CheckHappiness", Random.Range(3f, 9f), Random.Range(3f, 9f));
    }

    void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        RaycastHit hit;
        float bufferDistance = Random.Range(0.5f, 1f);
        if (Physics.Raycast(transform.position, moveDirection, out hit, currentSpeed * Time.deltaTime + bufferDistance))
        {
            PickNewTarget();
            return;
        }

        // Rotate towards the target direction
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 / 3 * Time.deltaTime);

        // Move towards the target position
        float step = currentSpeed * Time.deltaTime;
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
        PickNewTarget();
    }

    void PickNewTarget()
    {
        moveRange = Random.Range(minMoveRange, maxMoveRange); // Pick a random move range
        targetPosition = new Vector3(
            transform.position.x + Random.Range(-moveRange, moveRange),
            yPos,
            transform.position.z + Random.Range(-moveRange, moveRange)
        );
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed); // Pick a random move speed
        currentSpeed = 0; // Reset speed when a new target is picked
    }

    void CheckHappiness()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), Random.Range(0.25f, 0.5f));
        Instantiate(PoopPrefab, transform.position + randomOffset, Quaternion.identity);
    }
}