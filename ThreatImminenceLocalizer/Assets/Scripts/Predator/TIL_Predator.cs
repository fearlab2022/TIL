using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIL_Predator : MonoBehaviour
{
    string predatorPositon;
    // Start is called before the first frame update

     void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has the tag "position"
        if (other.CompareTag("Position"))
        {
            // Set the player's position to the name of the collided object
            predatorPositon = other.gameObject.name;
            float time = Time.realtimeSinceStartup;
            Debug.Log($"Pos: {predatorPositon}, Time: {time}");
        }
    }
}
