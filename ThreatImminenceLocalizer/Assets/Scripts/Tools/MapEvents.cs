using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEvents : MonoBehaviour
{
    public List<GameObject> tiles; // List of tile objects
    public float flashSpeed = 0.5f; // Speed of flashing (how often the tile turns on/off)

    private Coroutine activeCoroutine; // To store the active flashing coroutine
    private SpriteRenderer activeSpriteRenderer; // To store the currently flashing sprite

    // Start is called before the first frame update
    void Start()
    {
        // Initialize or validate that tiles are assigned in the inspector
        if (tiles == null || tiles.Count == 0)
        {
            Debug.LogError("No tiles assigned to the MapEvents script.");
        }
        ActivateRandomTile();
    }

    // Call this function to start the flashing of a random tile
    public void ActivateRandomTile()
    {
        // Ensure there are tiles in the list
        if (tiles == null || tiles.Count == 0)
        {
            Debug.LogWarning("Tile list is empty. Cannot activate a random tile.");
            return;
        }

        // Pick a random tile from the list
        int randomIndex = Random.Range(0, tiles.Count);
        GameObject selectedTile = tiles[randomIndex];

        // Ensure the tile has a SpriteRenderer component
        SpriteRenderer spriteRenderer = selectedTile.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Stop any previously active flashing coroutine
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }

            // Start the coroutine to make the tile flash
            activeSpriteRenderer = spriteRenderer;
            activeCoroutine = StartCoroutine(FlashTile(spriteRenderer));
        }
        else
        {
            Debug.LogWarning($"The selected tile {selectedTile.name} does not have a SpriteRenderer component.");
        }
    }

    // Call this function to stop the flashing of the current tile
    public void DeactivateTile()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeSpriteRenderer.enabled = false; // Ensure the sprite is turned off
            activeCoroutine = null;
            activeSpriteRenderer = null;
        }
        else
        {
            Debug.LogWarning("No tile is currently flashing.");
        }
    }

    // Coroutine to make the sprite flash indefinitely until deactivated
    private IEnumerator FlashTile(SpriteRenderer spriteRenderer)
    {
        bool isVisible = true;

        // Flash indefinitely until DeactivateTile is called
        while (true)
        {
            // Toggle the visibility of the sprite
            spriteRenderer.enabled = isVisible;
            isVisible = !isVisible;

            // Wait for the flashSpeed duration before toggling again
            yield return new WaitForSeconds(flashSpeed);
        }
    }
}
