using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 initialPosition;
    private const float ZoomStepSize = 6f;
    private const float CameraSpeed = 20f;
    private const float MaxCameraMove = 10f;
    private const float MinFOV = 15f;
    private const float MaxFOV = 90f;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        ProcessScrollWheelZoom();
        ProcessMouseDrag();
        ProcessKeyboardInput();
    }

    private void ProcessScrollWheelZoom()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        AdjustZoom(scrollData);
    }

    private void ProcessMouseDrag()
    {
        if (Input.GetMouseButton(1))
        {
            MoveCamera(new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y")).normalized);
        }
    }

    private void ProcessKeyboardInput()
    {
        Vector3 direction = new Vector3(
            (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1 : 0),
            0,
            (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1 : 0)
        ).normalized;
        MoveCamera(direction);

        if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Slash))
        {
            AdjustZoom(-1);
        }
        else if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.RightBracket))
        {
            AdjustZoom(1);
        }
    }

    private void MoveCamera(Vector3 direction)
    {
        Vector3 newPosition = transform.position + direction * Time.deltaTime * CameraSpeed;
        Vector3 clampedPosition = Vector3.ClampMagnitude(newPosition - initialPosition, MaxCameraMove);
        transform.position = initialPosition + clampedPosition;
    }

    private void AdjustZoom(float increment)
    {
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - increment * ZoomStepSize, MinFOV, MaxFOV);
    }

    public void ZoomInButton()
    {
        AdjustZoom(-1);
    }

    public void ZoomOutButton()
    {
        AdjustZoom(1);
    }
}