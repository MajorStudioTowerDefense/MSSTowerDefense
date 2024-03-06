using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float maxMoveSpeed = 20f;
    public float edgeBorderThickness = 10f;
    public Vector2 moveLimit;

    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    private Vector2 moveVelocity;

    public float acceleration = 30f;
    public float deceleration = 30f;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        // Mouse Movement
        float moveX = 0f;
        float moveY = 0f;

        if (Input.mousePosition.x >= Screen.width - edgeBorderThickness)
        {
            moveX = 1f;
        }
        else if (Input.mousePosition.x <= edgeBorderThickness)
        {
            moveX = -1f;
        }

        if (Input.mousePosition.y >= Screen.height - edgeBorderThickness)
        {
            moveY = 1f;
        }
        else if (Input.mousePosition.y <= edgeBorderThickness)
        {
            moveY = -1f;
        }

        // Apply acceleration
        if (moveX != 0)
        {
            moveVelocity.x = Mathf.MoveTowards(moveVelocity.x, maxMoveSpeed * moveX, acceleration * Time.deltaTime);
        }
        else // Apply deceleration
        {
            moveVelocity.x = Mathf.MoveTowards(moveVelocity.x, 0, deceleration * Time.deltaTime);
        }

        if (moveY != 0)
        {
            moveVelocity.y = Mathf.MoveTowards(moveVelocity.y, maxMoveSpeed * moveY, acceleration * Time.deltaTime);
        }
        else // Apply deceleration
        {
            moveVelocity.y = Mathf.MoveTowards(moveVelocity.y, 0, deceleration * Time.deltaTime);
        }

        pos.x += moveVelocity.x * Time.deltaTime;
        pos.y += moveVelocity.y * Time.deltaTime;

        // Zoom
        float zoomChange = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize -= zoomChange * zoomSpeed * 100f * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);

        // Apply limits to prevent the camera from moving too far away
        pos.x = Mathf.Clamp(pos.x, -moveLimit.x, moveLimit.x);
        pos.y = Mathf.Clamp(pos.y, -moveLimit.y, moveLimit.y);

        transform.position = pos;
    }
}
