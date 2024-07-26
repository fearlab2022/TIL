using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// test
public class CageRenderer : MonoBehaviour
{
    public bool render = true;

    // Reference to the Renderer component
    private Renderer cageRenderer;

    void Start()
    {
        cageRenderer = GetComponent<Renderer>();
        cageRenderer.enabled = render;
    }

    void Update()
    {
        if (cageRenderer.enabled != render)
        {
            cageRenderer.enabled = render;
        }
    }

    public void SetColor(Color color)
    {
        if (cageRenderer != null)
        {
            cageRenderer.material.color = color;
        }
    }
}
