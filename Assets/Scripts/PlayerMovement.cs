using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving;

    private int minX = 0;
    private int maxX = 9;
    private int minY = 0;
    private int maxY = 9;

    public float moveDelay = 0.2f;

    public GameObject cage;

    private CageRenderer cageRenderer;

    void Start()
    {
        targetPosition = transform.position;
        cageRenderer = cage.GetComponent<CageRenderer>();
        StartCoroutine(HandleMovement());
    }

    IEnumerator HandleMovement()
    {
        while (true)
        {
            yield return null;

            if (isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    transform.position = targetPosition;
                    isMoving = false;
                }
            }
            else
            {
                Vector3 direction = Vector3.zero;

                if (Input.GetKey(KeyCode.UpArrow) && targetPosition.y < maxY)
                {
                    if (cageRenderer.render && (targetPosition.x == 4 || targetPosition.x == 5) && (targetPosition.y + 1 == 4))
                    {
                        continue;
                    }
                    direction = Vector3.up;
                }
                else if (Input.GetKey(KeyCode.DownArrow) && targetPosition.y > minY)
                {
                    if (cageRenderer.render && (targetPosition.x == 4 || targetPosition.x == 5) && (targetPosition.y - 1 == 5))
                    {
                        continue;
                    }
                    direction = Vector3.down;
                }
                else if (Input.GetKey(KeyCode.LeftArrow) && targetPosition.x > minX)
                {
                    if (cageRenderer.render && (targetPosition.y == 4 || targetPosition.y == 5) && targetPosition.x - 1 == 5)
                    {
                        continue;
                    }
                    direction = Vector3.left;
                }
                else if (Input.GetKey(KeyCode.RightArrow) && targetPosition.x < maxX)
                {
                    if (cageRenderer.render && (targetPosition.y == 4 || targetPosition.y == 5) && targetPosition.x + 1 == 4)
                    {
                        continue;
                    }
                    direction = Vector3.right;
                }

                if (direction != Vector3.zero)
                {
                    targetPosition += direction;
                    isMoving = true;
                    yield return new WaitForSeconds(moveDelay);
                }
            }
        }
    }
}
