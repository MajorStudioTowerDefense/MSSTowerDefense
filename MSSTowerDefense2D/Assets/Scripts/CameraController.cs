using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public float cameraDragSpeed = 0.1f;
    public float maxMoveSpeed = 20f;
    public Vector2 moveLimit;

    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    private Vector2 moveVelocity;
    private Vector3 lastMousePosition;

    public float acceleration = 30f;
    public float deceleration = 30f;

    private bool isDisabled = false;

    void Update()
    {
        Vector3 pos = transform.position;

        if (!isDisabled)
        {
            // WASD Movement
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            // Middle Mouse Button Drag
            if (Input.GetMouseButtonDown(2))
            {
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                moveX -= delta.x * maxMoveSpeed * cameraDragSpeed;
                moveY -= delta.y * maxMoveSpeed * cameraDragSpeed;
                lastMousePosition = Input.mousePosition;
            }
        }

        // Apply movement
        moveVelocity.x = Mathf.Clamp(moveX, -1, 1) * maxMoveSpeed;
        moveVelocity.y = Mathf.Clamp(moveY, -1, 1) * maxMoveSpeed;

        pos.x += moveVelocity.x * Time.deltaTime;
        pos.y += moveVelocity.y * Time.deltaTime;

        // Zoom only if not over any UI element
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            float zoomChange = Input.GetAxis("Mouse ScrollWheel");
            Camera.main.orthographicSize -= zoomChange * zoomSpeed * 100f * Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }

        // Apply limits to prevent the camera from moving too far away
        pos.x = Mathf.Clamp(pos.x, -moveLimit.x, moveLimit.x);
        pos.y = Mathf.Clamp(pos.y, -moveLimit.y, moveLimit.y);

        transform.position = pos;
    }

    public void DisableCameraMovement()
    {
        isDisabled = true;
    }

    public void EnableCameraMovement()
    {
        isDisabled = false;
    }

}
