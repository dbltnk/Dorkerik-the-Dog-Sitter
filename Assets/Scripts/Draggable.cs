using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Draggable : MonoBehaviour
{
    public bool isDragging = false;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector3 draggingScale = new Vector3(3f, 3f, 3f);
    private float draggingZ = 2f;
    private Plane groundPlane;
    private Collider objectCollider; // Reference to the object's collider
    private float someThreshold = 3f;
    private RectTransform trashPanel;
    private AudioSource audioSource;

    public AudioClip splootSound;
    public AudioClip liftSound;
    public AudioClip dropSound;
    public AudioClip trashSound;


    private void Awake()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }


    void Start()
    {
        GameObject ground = GameObject.Find("Ground");
        Vector3 groundPoint = ground.GetComponent<Collider>().bounds.center;
        groundPoint.y = ground.GetComponent<Collider>().bounds.max.y;
        groundPlane = new Plane(Vector3.up, groundPoint);

        objectCollider = GetComponent<Collider>(); // Get the object's collider
        trashPanel = GameObject.Find("PanelTrash").GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Get all colliders in the object
            Collider[] colliders = GetComponentsInChildren<Collider>();

            foreach (var objectCollider in colliders)
            {
                if (Physics.Raycast(mouseRay, out hit) && hit.collider == objectCollider)
                {
                    // Add a distance check here
                    float distanceToCollider = Vector3.Distance(hit.point, objectCollider.transform.position);
                    if (distanceToCollider <= someThreshold) // Replace someThreshold with a suitable value
                    {
                        //Debug.Log("Started dragging");
                        isDragging = true;
                        originalPosition = transform.position;
                        originalScale = transform.localScale;
                        transform.localScale = draggingScale;
                        objectCollider.enabled = false; // Disable the collider
                        //Debug.Log("Original position: " + originalPosition);
                        //Debug.Log("Original scale: " + originalScale);
                        audioSource.PlayOneShot(liftSound);
                        break; // Exit the loop once a collider is found
                    }
                }
            }
        }
        if (isDragging)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;
            if (groundPlane.Raycast(mouseRay, out enter))
            {
                Vector3 newPosition = mouseRay.GetPoint(enter);
                transform.position = newPosition + new Vector3(0, draggingZ, 0);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            //Debug.Log("Stopped dragging");
            isDragging = false;
            transform.localScale = originalScale;
            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            foreach (var objectCollider in colliders)
            {
                objectCollider.enabled = true; // Enable the collider
            }
            //Debug.Log("Restored position: " + transform.position);
            //Debug.Log("Restored scale: " + transform.localScale);
            transform.position = new Vector3(transform.position.x, originalPosition.y, transform.position.z);

            // get the DogMovement component and if it is not null then call the PickNewTarget function
            DogMovement dogMovement = GetComponent<DogMovement>();
            if (dogMovement != null)
            {
                dogMovement.ChangeVisual();
                audioSource.PlayOneShot(splootSound);
            }
            else
            {
                audioSource.PlayOneShot(dropSound);
            }

            if (IsMouseOverTrashPanel())
            {
                StartCoroutine(CoDestroyButWithSound());
            }
        }
    }

    IEnumerator CoDestroyButWithSound()
    {
        gameObject.transform.position = new Vector3(-100f, -100f, -100f);
        audioSource.PlayOneShot(trashSound);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private bool IsMouseOverTrashPanel()
    {
        Vector2 localMousePosition = trashPanel.InverseTransformPoint(Input.mousePosition);
        return trashPanel.rect.Contains(localMousePosition);
    }
}