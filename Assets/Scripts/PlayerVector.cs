using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;
[Serializable]
public class PlayerVector
{
    public float x;
    public float y;
    public float z;
    public String timestamp;

    public PlayerVector(Vector3 position, String timestamp)
    {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
        this.timestamp = timestamp;
    }

    // Optional: A method to convert back to Vector3 if needed
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
