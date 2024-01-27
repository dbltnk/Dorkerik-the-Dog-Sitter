using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector3 draggingScale = new Vector3(1.5f, 1.5f, 1.5f);
    private float draggingZ = -1f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                if (!isDragging)
                {
                    Debug.Log("Started dragging");
                    isDragging = true;
                    originalPosition = transform.position;
                    originalScale = transform.localScale;
                    Debug.Log("Original position: " + originalPosition);
                    Debug.Log("Original scale: " + originalScale);
                    transform.position = new Vector3(transform.position.x, transform.position.y, draggingZ);
                    transform.localScale = draggingScale;
                    Debug.Log("New position: " + transform.position);
                    Debug.Log("New scale: " + transform.localScale);
                }
                else
                {
                    Debug.Log("Stopped dragging");
                    isDragging = false;
                    transform.position = originalPosition;
                    transform.localScale = originalScale;
                    Debug.Log("Restored position: " + transform.position);
                    Debug.Log("Restored scale: " + transform.localScale);
                }
            }
        }

        if (isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = worldPos;
            Debug.Log("Dragging position: " + transform.position);
        }
    }
}