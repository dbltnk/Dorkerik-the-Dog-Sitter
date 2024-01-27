using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector3 draggingScale = new Vector3(3f, 3f, 3f);
    private float draggingZ = 2f;
    private Plane groundPlane;
    private Collider objectCollider; // Reference to the object's collider
    private float someThreshold = 3f;

    void Start()
    {
        GameObject ground = GameObject.Find("Ground");
        Vector3 groundPoint = ground.GetComponent<Collider>().bounds.center;
        groundPoint.y = ground.GetComponent<Collider>().bounds.max.y;
        groundPlane = new Plane(Vector3.up, groundPoint);

        objectCollider = GetComponent<Collider>(); // Get the object's collider
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit) && hit.collider == objectCollider)
            {
                // Add a distance check here
                float distanceToCollider = Vector3.Distance(hit.point, objectCollider.transform.position);
                if (distanceToCollider <= someThreshold) // Replace someThreshold with a suitable value
                {
                    Debug.Log("Started dragging");
                    isDragging = true;
                    originalPosition = transform.position;
                    originalScale = transform.localScale;
                    transform.localScale = draggingScale;
                    objectCollider.enabled = false; // Disable the collider
                    Debug.Log("Original position: " + originalPosition);
                    Debug.Log("Original scale: " + originalScale);
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
            Debug.Log("Stopped dragging");
            isDragging = false;
            transform.localScale = originalScale;
            objectCollider.enabled = true; // Enable the collider
            Debug.Log("Restored position: " + transform.position);
            Debug.Log("Restored scale: " + transform.localScale);
            transform.position = new Vector3(transform.position.x, originalPosition.y, transform.position.z);
        }
    }
}