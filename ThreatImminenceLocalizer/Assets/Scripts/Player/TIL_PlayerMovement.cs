using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class TIL_PlayerMovement : MonoBehaviour
{
    private Vector2 movementInput;
    private Vector2 lastValue;
    private Vector2 unweighted_input;

    public Vector3 direction;
    public GameObject light;
    public bool canMove;
    public bool publicCanMove;
    public float threshold60;
    public float threshold120;
    public float threshold180;
    public float threshold240;
    public float threshold300;
    public float threshold360;
    public bool tutorialMode;

    public GameObject arrowUp;
    public GameObject arrowUpRight;
    public GameObject arrowUpLeft;
    public GameObject arrowDownRight;
    public GameObject arrowDownLeft;
    public GameObject arrowDown;
    public GameObject gridUp;
    public GameObject gridUpRight;
    public GameObject gridUpLeft;
    public GameObject gridDownRight;
    public GameObject gridDownLeft;
    public GameObject gridDown;
    public LayerMask Bounds; // Layer mask to specify the wall layer

    public float angle;

    public List<JoystickInputHandler> joyStickInputsList;
    public List<JoystickInputHandler> movementVectorList;
    public List<PositionHandler> playerPositionsList; 

    public List<LightEvent> lightEventList;


    public PlayerInput input;
    bool lightOn;
    bool firstMove;
    bool stochasticMovementIsRunning;
    float lastMoveTime;
    float movementDuration;


    void Start()
    {
        StartMovement();
    }
    
    public void StartMovement()
    {
        // Set default values in case PlayerPrefs keys do not exist
        threshold60 = PlayerPrefs.GetFloat("UpRight", 4.1f);   // Default value is 4.1f
        threshold120 = PlayerPrefs.GetFloat("Up", 4.1f);      // Default value is 4.1f
        threshold180 = PlayerPrefs.GetFloat("UpLeft", 4.1f);  // Default value is 4.1f
        threshold240 = PlayerPrefs.GetFloat("DownLeft", 4.1f);// Default value is 4.1f
        threshold300 = PlayerPrefs.GetFloat("Down", 4.1f);    // Default value is 4.1f
        threshold360 = PlayerPrefs.GetFloat("DownRight", 4.1f);// Default value is 4.1f

        // Set the last movement time to the current time
        lastMoveTime = Time.realtimeSinceStartup;

        // Clear input and movement lists for the new movement session
        joyStickInputsList.Clear();
        lightEventList.Clear();
        movementVectorList.Clear();
        playerPositionsList.Clear();

        // Initialize movement
        movementInput = Vector2.zero;
        stochasticMovementIsRunning = true;

        // Log the initial position of the player
        logPlayerMovementData();

        // Start the stochastic movement coroutine
        StartCoroutine(StochasticMovement());
    }

    
    private void Update() {
        if(canMove)
        {
            vizMovementDirection();
            movementContinousJoystick();
        }
    }

    private IEnumerator StochasticMovement()
    {
        while (stochasticMovementIsRunning)
        {
            // Determine the random movement duration
            movementDuration = UnityEngine.Random.Range(0.8f, 1.2f);
            lastMoveTime = Time.realtimeSinceStartup;
            canMove = true;

            // Calculate the duration when lights are on (75%) and off (25%)
            float lightsOnDuration = movementDuration * 0.75f;
            float lightsOffDuration = movementDuration * 0.25f;

            // Turn lights on and wait for the lights on duration
            lightsOn();
            yield return new WaitForSeconds(lightsOnDuration);
            if (!stochasticMovementIsRunning) yield break; // Stop coroutine immediately if flagged to stop

        
            // Turn lights off and wait for the lights off duration
            lightsOff();
            yield return new WaitForSeconds(lightsOffDuration);
            if (!stochasticMovementIsRunning) yield break; // Stop coroutine immediately if flagged to stop

            // At the end of movement, determine the step direction
            direction = DetermineStepDirection(movementInput);
            step(direction);
            // Reset movement input
            movementInput = Vector2.zero;
            canMove = false;
        }
    }

    public void movementContinousJoystick()
    {
          
        float elapsedTime  = Time.realtimeSinceStartup-lastMoveTime;
        
        
        // Calculate weight based on quadratic function
        float normalizedTime = elapsedTime / movementDuration;
        float weight;

        if (normalizedTime < 0.5f)
        {
            weight = Mathf.Pow(normalizedTime * 2, 2); // Quadratic increase
        }
        else
        {
            weight = Mathf.Pow(2 - normalizedTime * 2, 2); // Quadratic decrease
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 movementValue = new Vector2(horizontalInput, verticalInput);
        movementInput += movementValue * weight;
        if(firstMove) DetermineStepDirection(movementValue);
    }


    private Vector3 DetermineStepDirection(Vector2 movement)
    {
        angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        angle = (angle + 360f) % 360f; // Normalize angle to [0, 360] degrees
        JoystickInputHandler movementValue = new JoystickInputHandler();
        movementValue.time = Time.realtimeSinceStartup;
        movementValue.movementInput = movement;
        
        
        if(firstMove)
        {
            joyStickInputsList.Add(movementValue);
            firstMove = false;

        }
        else
        {
            movementVectorList.Add(movementValue);
        }

        if (angle < 60f && movement.magnitude > threshold60)
            direction = new Vector3(0.85f, 0.43f); // Up right
        else if (angle < 120f && movement.magnitude > threshold120)
            direction = new Vector3(0, 0.85f, 0); // Up
        else if (angle < 180f && movement.magnitude > threshold180)
            direction = new Vector3(-0.85f, 0.43f); // Up left
        else if (angle < 240f && movement.magnitude > threshold240)
            direction = new Vector3(-0.85f, -0.43f); // Down left
        else if (angle < 300f && movement.magnitude > threshold300)
            direction = new Vector3(0, -0.85f, 0); // Down
        else if (angle < 360f && movement.magnitude > threshold360)
            direction = new Vector3(0.85f, -0.43f); // Down right
        else
            direction = Vector3.zero; // No movement if magnitude is below the threshold

        return direction;
    }


    public void SetArrows()
    {
         if(tutorialMode)
            {
                arrowUpRight.SetActive(false);
                arrowUp.SetActive(false);
                arrowUpLeft.SetActive(false); 
                arrowDownLeft.SetActive(false); 
                arrowDown.SetActive(false); 
                arrowDownRight.SetActive(false);

                if (angle < 60f && angle>0f )
                    arrowUpRight.SetActive(true); // Up right
                else if (angle < 120f  && angle>0f )
                    arrowUp.SetActive(true); // Up
                else if (angle < 180f  && angle>0f )
                     arrowUpLeft.SetActive(true); // Up left
                else if (angle < 240f  && angle>0f )
                     arrowDownLeft.SetActive(true); // Down left
                else if (angle < 300f  && angle>0f  )
                    arrowDown.SetActive(true); // Down
                else if (angle < 360f  && angle>0f )
                     arrowDownRight.SetActive(true); // Down right
            }
    }

    public void step(Vector3 direction)
    {

        // Calculate the new position
        Vector3 newPosition = transform.position + (Vector3)direction;

        // Check if the new position is overlapping with any collider
        Collider2D overlap = Physics2D.OverlapCircle(newPosition, 0.1f);

            if (overlap == null || !overlap.CompareTag("Bounds"))
            {
                // If there's no collision with a wall, move the character
                transform.position = newPosition;


            }
            else
            {
                // Handle the case where the new position overlaps with a wall
                Debug.Log("Cannot move, position blocked by a wall!");
            }

                // Optionally reset movement input and other grid settings
                movementInput = Vector2.zero;
                gridUpRight.SetActive(false);
                gridUp.SetActive(false);
                gridUpLeft.SetActive(false);
                gridDownLeft.SetActive(false);
                gridDown.SetActive(false);
                gridDownRight.SetActive(false);
                SetArrows();
    }



    
    void lightsOff()
    {
        LightEvent lightEvent = new LightEvent();
        lightEvent.lightStatus = 0;
        lightEvent.time = Time.realtimeSinceStartup;
        lightEventList.Add(lightEvent);
        light.SetActive(false);

    }

    void lightsOn()
    {
        LightEvent lightEvent = new LightEvent();
        lightEvent.lightStatus = 1;
        lightEvent.time = Time.realtimeSinceStartup;
        lightEventList.Add(lightEvent);
        light.SetActive(true);
        firstMove = true;
    }

    




    public void vizMovementDirection()
    {
        if(canMove)
        {
            angle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
            angle = (angle + 360f) % 360f; // Normalize angle to [0, 360] degrees
            
            gridUpRight.SetActive(false);
            gridUp.SetActive(false);
            gridUpLeft.SetActive(false); 
            gridDownLeft.SetActive(false); 
            gridDown.SetActive(false); 
            gridDownRight.SetActive(false);

            if (angle < 60f && movementInput.magnitude > threshold60)
                gridUpRight.SetActive(true); // Up right
            else if (angle < 120f && movementInput.magnitude > threshold120)
                gridUp.SetActive(true); // Up
            else if (angle < 180f && movementInput.magnitude > threshold180)
                gridUpLeft.SetActive(true); // Up left
            else if (angle < 240f && movementInput.magnitude > threshold240)
                gridDownLeft.SetActive(true); // Down left
            else if (angle < 300f && movementInput.magnitude > threshold300)
                gridDown.SetActive(true); // Down
            else if (angle < 360f && movementInput.magnitude > threshold360)
                gridDownRight.SetActive(true); // Down right
            else{
                gridUpRight.SetActive(false);
                gridUp.SetActive(false);
                gridUpLeft.SetActive(false); 
                gridDownLeft.SetActive(false); 
                gridDown.SetActive(false); 
                gridDownRight.SetActive(false);
            }
        }
        
    }



    public void StopMovement()
    {
        stochasticMovementIsRunning = false;
        StopCoroutine(StochasticMovement());
    }

    void OnDisable()
    {
        light.SetActive(false);
        gridUpRight.SetActive(false);
        gridUp.SetActive(false);
        gridUpLeft.SetActive(false);
        gridDownLeft.SetActive(false);
        gridDown.SetActive(false);
        gridDownRight.SetActive(false);
    }

    // public void logEndPosition()
    // {
    //     this.GetComponent<GameController_Player>().endPosition.time = Time.realtimeSinceStartup;
    //     this.GetComponent<GameController_Player>().endPosition.position = this.GetComponent<GameController_Player>().player_position;
    // }
    public void logPlayerMovementData()
    {
        PositionHandler playerPos = new PositionHandler();
        playerPos.time = Time.realtimeSinceStartup;
        playerPos.position  = this.GetComponent<TIL_PlayerManager>().playerPosition;
        playerPositionsList.Add(playerPos);
    }
}
