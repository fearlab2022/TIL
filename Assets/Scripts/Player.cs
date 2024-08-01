using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections;
using System;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving;
    private int minX = 0;
    private int maxX = 9;
    private int minY = 0;
    private int maxY = 9;

    public float moveDelay = 0.2f;
    private bool canMove = false;

    private List<JoystickInputHandler> joystickInputsList = new List<JoystickInputHandler>();
    private Dictionary<String, Vector2> inputRecords = new Dictionary<String, Vector2>();

    public GameObject cage;
    private CageRenderer cageRenderer;
    private Rigidbody2D rb; 

    private Coroutine recordPositionCoroutine;
    private void Start()
    {
        targetPosition = transform.position;
        rb = GetComponent<Rigidbody2D>(); 

        if (cage != null)
        {
            cageRenderer = cage.GetComponent<CageRenderer>();
        }
    }

    #region Move Logic

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
        if(!canMove) {
            rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator RecordVectorandTime(List<PlayerVector> playerVectorList, List<JoystickInput> joyStickInputList)
    {
        while (true)
        {
            RecordPositionData(playerVectorList);
            RecordJoyStickInput(joyStickInputList);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void RecordPositionData(List<PlayerVector> playerVectorList)
    {
        Vector3 currentPosition = transform.position;
        String currentTime = DateTime.Now.ToString();
        playerVectorList.Add(new PlayerVector(currentPosition, currentTime));
    }
    
    private void RecordJoyStickInput(List<JoystickInput> joyStickInputList)
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        String currentTime = DateTime.Now.ToString();
        joyStickInputList.Add(new JoystickInput(horizontalInput, verticalInput, currentTime));
    }

    public void startRecording(List<PlayerVector> playerVectorList, List<JoystickInput> joyStickInputList)
        {
            recordPositionCoroutine = StartCoroutine(RecordVectorandTime(playerVectorList, joyStickInputList));
        }

        public void StopRecording()
        {
            if (recordPositionCoroutine != null)
            {
                StopCoroutine(recordPositionCoroutine);
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

            Vector3 nextPosition = targetPosition + moveDirection;

            if (cageRenderer != null && cageRenderer.render)
            {
                if (nextPosition.x >= 4 && nextPosition.x <= 5 && nextPosition.y >= 4 && nextPosition.y <= 5)
                {
                    return;
                }
            }

            targetPosition = new Vector3(
                Mathf.Clamp(nextPosition.x, minX, maxX),
                Mathf.Clamp(nextPosition.y, minY, maxY),
                nextPosition.z
            );

            isMoving = true;
            RecordInput(new Vector2(horizontalInput, verticalInput));
            StartCoroutine(MoveCooldown());
        }
    }

    #endregion

    #region Helper Functions

    private void RecordInput(Vector2 input)
    {
        String timestamp = DateTime.Now.ToString();
        inputRecords[timestamp] = input;
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
        if (cage != null)
        {
            cageRenderer = cage.GetComponent<CageRenderer>();
        }

        if (cageRenderer == null)
        {
            Debug.LogError("CageRenderer component not found on the cage GameObject!");
        }
    }

    public void ActivatePlayer()
    {
        gameObject.SetActive(true);
    }
    public void DeactivatePlayer()
    {
        gameObject.SetActive(false);
    }

    public void SetInitialPosition(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
        targetPosition = transform.position;
    }

    
    

    public void EnableMovement()
    {
        Debug.Log("EnableMovement called");
        canMove = true;
    }

    public void DisableMovement()
    {
        Debug.Log("DisableMovement called");
        canMove = false;
        isMoving = false;
        
        rb.velocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Chaser"))
        {
            DisableMovement();
            Debug.Log("Player caught by chaser, movement disabled.");
        }
    }

    #endregion
}

public class JoystickInputHandler
{
    public Vector2 movementInput;
    public float time;
}



