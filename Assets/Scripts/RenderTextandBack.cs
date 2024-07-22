using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextandBack : MonoBehaviour
{
    public bool middleScreen = false;
    public GameObject squareSprite;
    public GameObject messageText;

    void Start()
    {
        squareSprite.SetActive(false);
        messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (middleScreen)
        {
            RenderObjects();
        }
        else
        {
            HideObjects();
        }
    }

    void RenderObjects()
    {
        squareSprite.SetActive(true);
        messageText.gameObject.SetActive(true);

        
    }

    void HideObjects()
    {
        squareSprite.SetActive(false);
        messageText.gameObject.SetActive(false);
    }

    
}
