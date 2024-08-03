using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;
[Serializable]
public class TimingDescription
{
    public String description;
    public float timing;

    public TimingDescription(String description, float timing)
    {
        this.description = description;
        this.timing = timing;
    }

    public TimingDescription ShallowCopy()
    {
        return (TimingDescription)this.MemberwiseClone();
    }

    public void setDescription(String description) {
        this.description = description;
    }
    public void setTiming(float timing) {
        this.timing = timing;
    }

}
