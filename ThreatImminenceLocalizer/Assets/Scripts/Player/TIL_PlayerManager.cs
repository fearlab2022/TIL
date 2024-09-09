using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIL_PlayerManager : MonoBehaviour
{
    public string playerPosition;

    // Update is called once per frame
    void Update()
    {
        // You can add any additional player logic here if needed
    }

    public void enableMovement()
    {

    }

    public void resetPlayer()
    {
        
    }

    // Detect when the player enters a trigger collider
    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     // Check if the collided object has the tag "position"
    //     if (other.CompareTag("Position"))
    //     {
    //         // Set the player's position to the name of the collided object
    //         playerPosition = other.gameObject.name;
    //         float time = Time.realtimeSinceStartup;
    //         Debug.Log($"Pos: {playerPosition}, Time: {time}");
    //     }
    // }
}
