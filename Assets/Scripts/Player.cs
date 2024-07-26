using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving;

    private int minX = 0;
    private int maxX = 9;
    private int minY = 0;
    private int maxY = 9;

    public float moveDelay = 0.2f;
    public GameObject cage;
    private CageRenderer cageRenderer;

    private bool canMove = false;
    private float moveTimer;

    void Update()
    {
        if (canMove)
        {
            if (isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    transform.position = targetPosition;
                    isMoving = false;
                }
            }
            else
            {
                HandleMovement();
            }
        }
    }

    void HandleMovement()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow) && targetPosition.y < maxY)
        {
            if (cageRenderer != null && cageRenderer.render && (targetPosition.x == 4 || targetPosition.x == 5) && (targetPosition.y + 1 == 4))
            {
                return;
            }
            direction = Vector3.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && targetPosition.y > minY)
        {
            if (cageRenderer != null && cageRenderer.render && (targetPosition.x == 4 || targetPosition.x == 5) && (targetPosition.y - 1 == 5))
            {
                return;
            }
            direction = Vector3.down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && targetPosition.x > minX)
        {
            if (cageRenderer != null && cageRenderer.render && (targetPosition.y == 4 || targetPosition.y == 5) && targetPosition.x - 1 == 5)
            {
                return;
            }
            direction = Vector3.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && targetPosition.x < maxX)
        {
            if (cageRenderer != null && cageRenderer.render && (targetPosition.y == 4 || targetPosition.y == 5) && targetPosition.x + 1 == 4)
            {
                return;
            }
            direction = Vector3.right;
        }

        if (direction != Vector3.zero)
        {
            if (moveTimer <= 0f)
            {
                Vector3 newPosition = targetPosition + direction;
                if (newPosition.x >= minX && newPosition.x <= maxX && newPosition.y >= minY && newPosition.y <= maxY)
                {
                    targetPosition = newPosition;
                    isMoving = true;
                    moveTimer = moveDelay;  
                }
            }
            else
            {
                moveTimer -= Time.deltaTime;  
            }
        }
    }

    public void Initialize(GameObject cage)
    {
        this.cage = cage;
        targetPosition = transform.position;
        cageRenderer = cage.GetComponent<CageRenderer>();

        if (cageRenderer == null)
        {
            Debug.LogError("CageRenderer component not found on the cage GameObject!");
        }
    }

    public void SetInitialPosition(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z); // Set player's position
        targetPosition = transform.position; // Ensure targetPosition is in sync with the initial position
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
    }
}
