using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class JoystickInput
{
    public float horizontal;
    public float vertical;
    public String timestamp;

    public JoystickInput(float horizontal, float vertical, String timestamp)
    {
        this.horizontal = horizontal;
        this.vertical = vertical;
        this.timestamp = timestamp;
    }


}
