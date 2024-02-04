using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 initialPosition;
    private float zoomStepSize = 6f; // Adjust this value to control zoom speed

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");
        Zoom(scrollData);

        // Check if the right mouse button is pressed
        if (Input.GetMouseButton(1))
        {
            // Get the new position for the camera based on mouse movement
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");
            Vector3 direction = new Vector3(h, 0, v).normalized;
            MoveCamera(direction);
        }

        HandleKeyboardInput();
    }

    void HandleKeyboardInput()
    {
        // WASD and arrow keys for camera movement
        float h = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
        float v = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
        Vector3 direction = new Vector3(h, 0, v).normalized;
        MoveCamera(direction);

        // Plus and minus for zoom
        if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Plus)|| Input.GetKey(KeyCode.Slash))
        {
            Zoom(-1);
        }
        else if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus)|| Input.GetKey(KeyCode.RightBracket))
        {
            Zoom(1);
        }
    }

    void MoveCamera(Vector3 direction)
    {
        // Calculate the new position
        Vector3 newPosition = transform.position + direction * Time.deltaTime * 20f; // 20f is the speed of the camera movement

        // Subtract the initial position from the new position and clamp the result
        Vector3 relativePosition = newPosition - initialPosition;
        relativePosition.x = Mathf.Clamp(relativePosition.x, -10, 10);
        relativePosition.y = Mathf.Clamp(relativePosition.y, -10, 10);
        relativePosition.z = Mathf.Clamp(relativePosition.z, -10, 10);

        // Add the initial position back to the relative position
        newPosition = initialPosition + relativePosition;

        // Apply the new position to the camera
        transform.position = newPosition;
    }

    void Zoom(float increment)
    {
        Camera.main.fieldOfView -= increment * zoomStepSize;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 15, 90);
    }

    public void ZoomInButton()
    {
        Zoom(-1);
    }

    public void ZoomOutButton()
    {
        Zoom(1);
    }
}