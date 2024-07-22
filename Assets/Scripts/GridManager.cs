using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private float gapSize = 0.5f; 

    [SerializeField] private Transform cam;

    [SerializeField] private Material lavaMaterial;
    [SerializeField] private Material nearLavaMaterial;
    [SerializeField] private Material secondNearLavaMaterial;
    [SerializeField] private Material normalTileMaterial;

    private Tile[,] tiles;
    private Pathfinding pathfinding;
    private Tile currentLavaTile;
    private float lavaUpdateInterval = 15f;

    public int Width => width;
    public int Height => height;

    void Start()
    {
        pathfinding = new Pathfinding(this);
    }

    public void GenerateGrid()
    {
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.x = x;
                spawnedTile.y = y;
                tiles[x, y] = spawnedTile;
            }
        }

        cam.transform.position = new Vector3((width - 1) * (1 + gapSize) / 2, (height - 1) * (1 + gapSize) / 2, -10);
        Camera.main.orthographicSize = Mathf.Max(width * (1 + gapSize), height * (1 + gapSize)) / 2f + 1;

        SetNewLavaTile();
        StartCoroutine(UpdateLavaTile());
    }

    IEnumerator UpdateLavaTile()
    {
        while (true)
        {
            yield return new WaitForSeconds(lavaUpdateInterval);
            SetNewLavaTile();
        }
    }

    void SetNewLavaTile()
    {
        foreach (Tile tile in tiles)
        {
            tile.SetMaterial(normalTileMaterial);
        }

        Tile newLavaTile;
        do
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            newLavaTile = tiles[x, y];
        } while (Vector3.Distance(newLavaTile.transform.position, Vector3.zero) < 2f); // Ensure it's not too close to (0, 0)

        currentLavaTile = newLavaTile;
        currentLavaTile.SetMaterial(lavaMaterial);

        // Set materials for the tiles around the lava tile
        SetSurroundingTilesMaterial(currentLavaTile.x, currentLavaTile.y, 1, nearLavaMaterial);
        SetSurroundingTilesMaterial(currentLavaTile.x, currentLavaTile.y, 2, secondNearLavaMaterial);
    }

    void SetSurroundingTilesMaterial(int centerX, int centerY, int radius, Material material)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) == radius || Mathf.Abs(y) == radius) 
                {
                    Tile adjacentTile = GetTileAt(centerX + x, centerY + y);
                    if (adjacentTile != null)
                    {
                        adjacentTile.SetMaterial(material);
                    }
                }
            }
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return tiles[x, y];
        }
        return null;
    }

    public List<Tile> FindPath(Tile startTile, Tile targetTile)
    {
        return pathfinding.FindPath(startTile, targetTile);
    }
}
