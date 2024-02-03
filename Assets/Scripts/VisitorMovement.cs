using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class VisitorMovement : MonoBehaviour
{
    private float minMoveRange = 8f;
    private float maxMoveRange = 24f;

    private float minMoveRangeNearDog = 3f;
    private float maxMoveRangeNearDog = 5f;
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

    private AudioSource audioSource;
    public AudioClip leaveSound;
    public AudioClip moneySound;

    private NavMeshAgent agent;
    private bool hasReachedPosition;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
    }

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
        hasReachedPosition = agent.remainingDistance <= 1.5f && !agent.pathPending;

        if (isExiting)
        {
            exitTimer += Time.deltaTime;
            if (exitTimer >= ForceExitAfter || Vector3.Distance(transform.position, DoorExit.transform.position) <= 2f)
            {
                audioSource.PlayOneShot(leaveSound);
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

        print("Moving towards: " + targetPosition);
        agent.SetDestination(targetPosition); // Move the agent towards the target

        // If the agent has arrived at the target, set animation active to false
        if (hasReachedPosition)
        {
            GetComponentInChildren<Animation>().enabled = false;
            // face the nearest GameObject with a DogMovement script
            GameObject[] dogs = GameObject.FindGameObjectsWithTag("Dog");
            if (dogs.Length == 0)
            {
                return;
            }
            GameObject nearestDog = GameObject.FindGameObjectsWithTag("Dog")[0];
            if (nearestDog)
            {
                float nearestDistance = Vector3.Distance(transform.position, nearestDog.transform.position);
                foreach (GameObject dog in GameObject.FindGameObjectsWithTag("Dog"))
                {
                    float distance = Vector3.Distance(transform.position, dog.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDog = dog;
                        nearestDistance = distance;
                    }
                }
                transform.LookAt(nearestDog.transform);
            }
        }
        else
        {
            GetComponentInChildren<Animation>().enabled = true;
        }
    }

    void PickNewTarget()
    {
        // Get all GameObjects with the DogMovement script
        DogMovement[] dogs = FindObjectsOfType<DogMovement>();

        // Check if there are any dogs in the scene
        if (dogs.Length == 0)
        {
            moveRange = Random.Range(minMoveRange, maxMoveRange); // Pick a random move range
            targetPosition = new Vector3(
                transform.position.x + Random.Range(-moveRange, moveRange),
                yPos,
                transform.position.z + Random.Range(-moveRange, moveRange)
            );
            return;
        }

        // Calculate the distances to the dogs
        List<(float distance, DogMovement dog)> distances = new List<(float distance, DogMovement dog)>();
        foreach (DogMovement dog in dogs)
        {
            float distance = Vector3.Distance(transform.position, dog.transform.position);
            distances.Add((distance, dog));
        }

        // Sort the list by distance
        distances.Sort((x, y) => x.distance.CompareTo(y.distance));

        // Take the 5 closest dogs, or all of them if there are less than 5
        List<DogMovement> closestDogs = distances.Take(Mathf.Min(5, distances.Count)).Select(x => x.dog).ToList();

        // Pick a random dog from the closest ones
        DogMovement targetDog = closestDogs[Random.Range(0, closestDogs.Count)];

        // Pick a random move range
        moveRange = Random.Range(minMoveRangeNearDog, maxMoveRangeNearDog);

        // Generate a random angle
        float angle = Random.Range(0, 2 * Mathf.PI);

        // Calculate the x and z coordinates using the random angle and move range
        float x = targetDog.transform.position.x + moveRange * Mathf.Cos(angle);
        float z = targetDog.transform.position.z + moveRange * Mathf.Sin(angle);

        // Set the target position
        targetPosition = new Vector3(x, yPos, z);
    }

    void CheckHappiness()
    {
        StartCoroutine(CoCheckHappiness());
    }

    private IEnumerator CoCheckHappiness()
    {
        if (!hasReachedPosition)
        {
            yield return null;
        }

        if (Scorer.Instance.GetMoney() < 10)
        {
            Instantiate(HeartPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            yield return null;
        }

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
                audioSource.PlayOneShot(moneySound);
                Instantiate(HeartPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
        else
        {
            //Debug.Log("No heart prefab instantiated due to non-positive total happiness.");
        }
    }
}