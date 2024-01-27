using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public float Happiness;

    public Texture awooTexture;
    public Texture borkBorkTexture;
    public Texture tippyTapsTexture;
    public Texture zoomiesTexture;
    public Texture angeryTexture;
    public Texture splootTexture;
    public Texture chomppTexture;

    private RawImage rawImage;

    public string Name;
    public List<string> TreatsLiked;


    void Start()
    {
        // find a RawImage component in any child
        rawImage = GetComponentInChildren<RawImage>();

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

        InvokeRepeating("Poop", Random.Range(3f, 9f), Random.Range(3f, 9f));
        InvokeRepeating("Bark", Random.Range(9f, 11f), Random.Range(9f, 11f));
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

        CalculateHappiness();
    }

    private void CalculateHappiness()
    {
        // Happiness decreases for every poop and other dog or hooman that is within a radius of 5f
        int numDogsWithinRadius = 0;
        int numPoopsWithinRadius = 0;
        int numHoomansWithinRadius = 0;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Dog")
            {
                if (collider.gameObject == gameObject) continue; // Skip self
                numDogsWithinRadius++;
            }
            else if (collider.gameObject.tag == "Poop")
            {
                numPoopsWithinRadius++;
            }
            else if (collider.gameObject.tag == "Human")
            {
                numHoomansWithinRadius++;
            }
        }

        float weightPoops = 0.5f;
        float weightDogs = 1f;
        float weightHoomans = 0.1f;

        float happinessDecreasePerSecond = weightPoops * numPoopsWithinRadius + weightDogs * numDogsWithinRadius + weightHoomans * numHoomansWithinRadius;
        Happiness -= happinessDecreasePerSecond * Time.deltaTime;

        // Clamp happiness between 0 and 1
        Happiness = Mathf.Clamp(Happiness, -100, 100);
        //print("Happiness: " + Happiness);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Treat" && TreatsLiked.Contains(other.gameObject.GetComponentInChildren<Treat>().Name))
        {
            rawImage.enabled = true;
            rawImage.texture = chomppTexture;
            Happiness += 50;
            StartCoroutine(DisableRawImageAfterSeconds(3));
            Destroy(other.gameObject); // destroy the treat
        }
        else if (other.gameObject.tag == "Treat" && !TreatsLiked.Contains(other.gameObject.GetComponentInChildren<Treat>().Name))
        {
            rawImage.enabled = true;
            rawImage.texture = angeryTexture;
            Happiness -= 25;
            StartCoroutine(DisableRawImageAfterSeconds(3));
            Destroy(other.gameObject); // destroy the treat
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

    void Poop()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(0.5f, 0.75f));
        Instantiate(PoopPrefab, transform.position + randomOffset, Quaternion.identity);
    }

    IEnumerator DisableRawImageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        rawImage.enabled = false;
    }

    void Bark()
    {
        switch (Happiness)
        {
            case float n when (n < -75):
                rawImage.enabled = true;
                rawImage.texture = awooTexture;
                break;
            case float n when (n < -25):
                rawImage.enabled = true;
                rawImage.texture = borkBorkTexture;
                break;
            case float n when (n > 25):
                rawImage.enabled = true;
                rawImage.texture = tippyTapsTexture;
                break;
            case float n when (n > 75):
                rawImage.enabled = true;
                rawImage.texture = zoomiesTexture;
                break;
            default:
                break;
        }

        StartCoroutine(DisableRawImageAfterSeconds(3));
    }
}