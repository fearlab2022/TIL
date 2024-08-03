using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
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
    private float lavaUpdateInterval = 5f;

    private List<Tile> surroundingLavaTiles = new List<Tile>();

    public int Width => width;
    public int Height => height;

    // Ensure this method is called when changes are made in the editor
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            GenerateGridInEditor();
        }
    }

    public void GenerateGrid()
    {
        // Clean up any existing tiles
        if (tiles != null)
        {
            foreach (var tile in tiles)
            {
                if (tile != null)
                {
                    Destroy(tile.gameObject);
                }
            }
        }

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

                DontDestroyOnLoad(spawnedTile.gameObject);
            }
        }

        pathfinding = new Pathfinding(this);

        if (cam != null)
        {
            cam.transform.position = new Vector3((width - 1) * (1 + gapSize) / 2, (height - 1) * (1 + gapSize) / 2, -10);
            Camera.main.orthographicSize = Mathf.Max(width * (1 + gapSize), height * (1 + gapSize)) / 2f + 1;
        }

        Debug.Log("Grid generated.");
    }

    private void GenerateGridInEditor()
    {
        // Clean up any existing tiles
        if (tiles != null)
        {
            foreach (var tile in tiles)
            {
                if (tile != null)
                {
                    DestroyImmediate(tile.gameObject);
                }
            }
        }

        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                spawnedTile.name = $"Tile {x} {y}";
                var tileComponent = spawnedTile.GetComponent<Tile>();
                tileComponent.x = x;
                tileComponent.y = y;
                tiles[x, y] = tileComponent;

                #if UNITY_EDITOR
                EditorUtility.SetDirty(spawnedTile);
                #endif
            }
        }

        #if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
        #endif

        if (cam != null)
        {
            cam.transform.position = new Vector3((width - 1) * (1 + gapSize) / 2, (height - 1) * (1 + gapSize) / 2, -10);
            Camera.main.orthographicSize = Mathf.Max(width * (1 + gapSize), height * (1 + gapSize)) / 2f + 1;
        }

        Debug.Log("Grid generated in editor.");
    }

    public void SetNewLavaTile()
    {
        foreach (Tile tile in tiles)
        {
            tile.SetMaterial(normalTileMaterial);
        }

        surroundingLavaTiles.Clear();

        Tile newLavaTile;
        do
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            newLavaTile = tiles[x, y];
        } while (Vector3.Distance(newLavaTile.transform.position, Vector3.zero) < 4f);

        currentLavaTile = newLavaTile;
        currentLavaTile.SetMaterial(lavaMaterial);

        SetSurroundingTilesMaterial(currentLavaTile.x, currentLavaTile.y, 1, nearLavaMaterial);
        SetSurroundingTilesMaterial(currentLavaTile.x, currentLavaTile.y, 2, secondNearLavaMaterial);
    }

    private void SetSurroundingTilesMaterial(int centerX, int centerY, int radius, Material material)
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
                        surroundingLavaTiles.Add(adjacentTile);
                    }
                }
            }
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if (tiles == null)
        {
            Debug.LogError("Tiles array not initialized!");
            return null;
        }

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

    public List<Tile> GetSurroundingLavaTiles()
    {
        return surroundingLavaTiles;
    }
}
