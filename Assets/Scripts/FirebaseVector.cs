using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class FirebaseVector
{
    public float x;
    public float y;
    public float z;

    public FirebaseVector(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
