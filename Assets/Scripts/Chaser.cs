using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float calculateTime = 0.05f;
    public bool chaserRender = true;
    public bool chase = true;

    private GridManager gridManager;
    private Transform playerTransform;
    private List<Tile> path;
    private SpriteRenderer spriteRenderer;
    public string persistentDataPath;

    private Coroutine chaseCoroutine;
    private Coroutine recordCoroutine;
    private List<PlayerVector> chaserPositionDataList = new List<PlayerVector>();

    public void Initialize(GridManager gridManager, Transform playerTransform)
    {
        this.gridManager = gridManager;
        this.playerTransform = playerTransform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found!");
            return;
        }

        if (gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        // Set the initial visibility based on chaserRender
        spriteRenderer.enabled = chaserRender;

        StartChasing();
    }

    void Update()
    {
        // Update the visibility based on chaserRender
        spriteRenderer.enabled = chaserRender;

        // Restart coroutine if chase value changes
        if (chaseCoroutine != null && !chase)
        {
            StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;

            // Stop recording positions
            if (recordCoroutine != null)
            {
                StopCoroutine(recordCoroutine);
                recordCoroutine = null;
            }
        }
        else if (chaseCoroutine == null && chase)
        {
            StartChasing();
        }
    }

    void StartChasing()
    {
        if (chase)
        {
            chaseCoroutine = StartCoroutine(ChasePlayer());
            recordCoroutine = StartCoroutine(RecordChaserPosition());
        }
    }

    IEnumerator ChasePlayer()
    {
        while (chase)
        {
            if (playerTransform != null)
            {
                Tile startTile = gridManager.GetTileAt(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
                Tile targetTile = gridManager.GetTileAt(Mathf.RoundToInt(playerTransform.position.x), Mathf.RoundToInt(playerTransform.position.y));

                if (startTile == null || targetTile == null)
                {
                    Debug.LogError("Start tile or target tile is null");
                    yield break;
                }

                path = gridManager.FindPath(startTile, targetTile);

                if (path != null)
                {
                    foreach (Tile tile in path)
                    {
                        Vector3 targetPosition = new Vector3(tile.x, tile.y, 0);
                        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                        {
                            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                            yield return null;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(calculateTime);
        }
    }

    IEnumerator RecordChaserPosition()
    {
        while (chase)
        {
            chaserPositionDataList.Add(new PlayerVector(transform.position, Time.realtimeSinceStartup));
            yield return new WaitForSeconds(0.2f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            chase = false;

        }
    }

    public List<PlayerVector> GetChaserPositionData()
    {
        return new List<PlayerVector>(chaserPositionDataList);
    }

    public void ClearChaserPositionData()
    {
        chaserPositionDataList.Clear();
    }
}
