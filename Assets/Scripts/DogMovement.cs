using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogMovement : MonoBehaviour
{
    public float minMoveRange = 2f;
    public float maxMoveRange = 6f;
    private float moveRange;

    public float moveInterval = 3f;
    public float moveRandInterval = 1.5f;
    public float minMoveSpeed = 4f;
    public float maxMoveSpeed = 6f;
    public float acceleration = 0.5f;

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

    public float ValueMultiplier;

    private Draggable draggable;

    private Transform meshesTransform;
    private GameObject currentMesh;

    private AudioSource audioSource;
    public AudioClip awooSound;
    public AudioClip barkSound;
    public AudioClip chomppSound;
    public AudioClip poopSound;
    public AudioClip tippyTapsSound;
    public AudioClip zoomiesSound;
    public AudioClip splootSound;
    public AudioClip angerySound;


    void Awake()
    {
        draggable = GetComponentInChildren<Draggable>();
        meshesTransform = transform.Find("Meshes");
        currentMesh = meshesTransform.Find("ween_medium_static").gameObject;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // find a RawImage component in any child
        rawImage = GetComponentInChildren<RawImage>();

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        ChangeVisual();
        InvokeRepeating("ChangeVisual", moveInterval, Random.Range(moveInterval - moveRandInterval, moveInterval + moveRandInterval));
        InvokeRepeating("Poop", Random.Range(10f, 15f), Random.Range(16f, 21f));
        InvokeRepeating("Bark", Random.Range(9f, 11f), Random.Range(9f, 11f));
    }

    void Update()
    {
        Transform meshesTransform = transform.Find("Meshes");

        if (draggable.isDragging)
        {
            for (int i = 0; i < meshesTransform.childCount; i++)
            {
                Transform child = meshesTransform.GetChild(i);

                // If the child's name is not "ween_medium_airjail", disable it
                if (child.name == "ween_medium_airjail")
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
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
            audioSource.PlayOneShot(chomppSound);
        }
        else if (other.gameObject.tag == "Treat" && !TreatsLiked.Contains(other.gameObject.GetComponentInChildren<Treat>().Name))
        {
            rawImage.enabled = true;
            rawImage.texture = angeryTexture;
            Happiness -= 25;
            StartCoroutine(DisableRawImageAfterSeconds(3));
            Destroy(other.gameObject); // destroy the treat
            audioSource.PlayOneShot(angerySound);
        }
    }

    public void ChangeVisual()
    {
        if (draggable.isDragging)
        {
            return;
        }

        int childCount = meshesTransform.childCount;
        System.Random rand = new System.Random();
        int randomIndex = rand.Next(childCount);

        bool isAnyChildActive = false;

        // i = 1 to skip airchail, the first child
        for (int i = 1; i < childCount; i++)
        {
            Transform child = meshesTransform.GetChild(i);
            if (i == randomIndex)
            {
                child.gameObject.SetActive(true);
                isAnyChildActive = true;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        // If no child is active, manually activate the first one (or any other you prefer)
        if (!isAnyChildActive && childCount > 1)
        {
            meshesTransform.GetChild(1).gameObject.SetActive(true);
        }

        // disable airjail
        meshesTransform.GetChild(0).gameObject.SetActive(false);

        // Pick a random y rotation for the object
        float randomYRotation = UnityEngine.Random.Range(0f, 360f);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, randomYRotation, transform.eulerAngles.z);
    }

    void Poop()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(0.5f, 0.75f));
        Instantiate(PoopPrefab, transform.position + randomOffset, Quaternion.identity);
        audioSource.PlayOneShot(poopSound);
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
                audioSource.PlayOneShot(awooSound);
                break;
            case float n when (n < -25):
                rawImage.enabled = true;
                rawImage.texture = borkBorkTexture;
                audioSource.PlayOneShot(barkSound);
                break;
            case float n when (n > 25):
                rawImage.enabled = true;
                rawImage.texture = tippyTapsTexture;
                audioSource.PlayOneShot(tippyTapsSound);
                break;
            case float n when (n > 75):
                rawImage.enabled = true;
                rawImage.texture = zoomiesTexture;
                audioSource.PlayOneShot(zoomiesSound);
                break;
            default:
                break;
        }

        StartCoroutine(DisableRawImageAfterSeconds(3));
    }
}