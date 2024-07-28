using UnityEngine;
using System.Collections.Generic;

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
    private float moveTimer;
    private bool canMove = false;

    private List<JoystickInputHandler> joystickInputsList = new List<JoystickInputHandler>();

    public GameObject cage;
    private CageRenderer cageRenderer;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
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

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (!isMoving && (horizontalInput != 0 || verticalInput != 0))
        {
            Vector3 moveDirection = Vector3.zero;

            if (Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput))
            {
                moveDirection = horizontalInput > 0 ? Vector3.right : Vector3.left;
            }
            else
            {
                moveDirection = verticalInput > 0 ? Vector3.up : Vector3.down;
            }

            targetPosition += moveDirection;
            targetPosition = new Vector3(
                Mathf.Clamp(targetPosition.x, minX, maxX),
                Mathf.Clamp(targetPosition.y, minY, maxY),
                targetPosition.z
            );

            isMoving = true;

            StartCoroutine(MoveCooldown());
        }
    }

    private System.Collections.IEnumerator MoveCooldown()
    {
        canMove = false;
        yield return new WaitForSeconds(moveDelay);
        canMove = true;
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
        transform.position = new Vector3(x, y, transform.position.z);
        targetPosition = transform.position;
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

public class JoystickInputHandler
{
    public Vector2 movementInput;
    public float time;
}
