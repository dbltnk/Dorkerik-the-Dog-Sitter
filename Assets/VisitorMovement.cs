using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorMovement : MonoBehaviour
{
    private float minMoveRange = 8f;
    private float maxMoveRange = 24f;
    private float moveRange;

    private float moveInterval = 10f;
    private float moveRandInterval = 2f;
    private float minMoveSpeed = 4f;
    private float maxMoveSpeed = 6f;
    private float acceleration = 0.5f;

    private float moveSpeed;
    private float currentSpeed = 0f;
    private float yPos;
    private Vector3 targetPosition;
    private Rigidbody rb;

    public GameObject HeartPrefab;
    private float DogViewingRange = 7.5f;

    private float MinVisitDuration = 120f;
    private float MaxVisitDuration = 180f;
    private float ForceExitAfter = 60f;
    private float VisitDuration;
    public GameObject DoorExit;
    private float exitTimer = 0f;
    private bool isExiting = false;

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

        InvokeRepeating("CheckHappiness", 5f, Random.Range(4f, 6f));

        DoorExit = GameObject.Find("DoorExit");
        VisitDuration = Random.Range(MinVisitDuration, MaxVisitDuration);
    }

    void Update()
    {
        if (isExiting)
        {
            exitTimer += Time.deltaTime;
            if (exitTimer >= ForceExitAfter || Vector3.Distance(transform.position, DoorExit.transform.position) <= 2f)
            {
                Destroy(gameObject);
                return;
            }
            targetPosition = DoorExit.transform.position;
        }
        else
        {
            VisitDuration -= Time.deltaTime;
            if (VisitDuration <= 0f)
            {
                isExiting = true;
            }
        }

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
        StartCoroutine(CoCheckHappiness());
    }

    private IEnumerator CoCheckHappiness()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, DogViewingRange);
        //Debug.Log("Number of colliders detected: " + colliders.Length);
        float totalHappiness = 0;
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Dog"))
            {
                float happiness = collider.transform.parent.parent.GetComponent<DogMovement>().Happiness;
                //Debug.Log("Dog detected with happiness: " + happiness);
                totalHappiness += happiness * collider.transform.parent.parent.GetComponent<DogMovement>().ValueMultiplier;
            }
        }

        //Debug.Log("Total happiness: " + totalHappiness);

        if (totalHappiness > 0)
        {
            // for every 100 happiness, spawn a heart
            int numHeartsToSpawn = Mathf.CeilToInt(totalHappiness / 100);
            for (int i = 0; i < numHeartsToSpawn; i++)
            {
                yield return new WaitForSeconds(0.15f);
                //Debug.Log("Instantiating heart prefab due to positive total happiness.");
                Instantiate(HeartPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
        else
        {
            //Debug.Log("No heart prefab instantiated due to non-positive total happiness.");
        }
    }
}