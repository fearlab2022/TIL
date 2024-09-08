using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PredatorAStar : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public List<Transform> graphNodes; // List of graph nodes
    public float moveSpeed = 1f; // Movement speed (units per second)
    public bool isDebugMode = true; // Toggle debug logs on or off

    private List<Transform> pathToPlayer = new List<Transform>(); // Holds the path to the player
    private int currentPathIndex = 0; // Current index on the path
    private Transform currentNode; // The node the predator is currently moving towards

    private Dictionary<Transform, Transform> cameFrom = new Dictionary<Transform, Transform>(); // Keeps track of the A* path
    private Dictionary<Transform, float> costSoFar = new Dictionary<Transform, float>(); // Cost to reach each node

    
    
    private void Attack()
    {
        // Start the search for the player
        StartCoroutine(SearchForPlayer());
    }

    private IEnumerator SearchForPlayer()
    {
        while (true)
        {
            if (isDebugMode)
                Debug.Log("Starting A* Search for the player...");

            // Perform A* search and generate a path to the player
            pathToPlayer = AStarSearch(transform.position, player.position);

            if (pathToPlayer != null && pathToPlayer.Count > 0)
            {
                if (isDebugMode)
                    Debug.Log("Path found! Moving predator towards the player...");
                currentPathIndex = 0;
                StartCoroutine(MoveAlongPath());
            }
            else
            {
                if (isDebugMode)
                    Debug.LogWarning("No path found to the player.");
            }

            // Recalculate path every second
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator MoveAlongPath()
    {
        while (currentPathIndex < pathToPlayer.Count)
        {
            currentNode = pathToPlayer[currentPathIndex];

            if (isDebugMode)
                Debug.Log($"Moving towards node: {currentNode.name} at position: {currentNode.position}");

            // Move towards the target node
            while (Vector3.Distance(transform.position, currentNode.position) > 0.1f)
            {
                // Move towards the current node at a speed of moveSpeed units per second
                transform.position = Vector3.MoveTowards(transform.position, currentNode.position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            if (isDebugMode)
                Debug.Log($"Reached node: {currentNode.name}");

            currentPathIndex++;
        }

        if (isDebugMode)
            Debug.Log("Completed moving along the path.");
    }

    private List<Transform> AStarSearch(Vector3 startPos, Vector3 goalPos)
    {
        Transform startNode = GetClosestNode(startPos); // Get the closest node to the predator's current position
        Transform goalNode = GetClosestNode(goalPos); // Get the closest node to the player's position

        if (isDebugMode)
            Debug.Log($"Starting A* search from node: {startNode.name} to node: {goalNode.name}");

        // Reset the A* dictionaries
        cameFrom.Clear();
        costSoFar.Clear();
        cameFrom[startNode] = null;
        costSoFar[startNode] = 0;

        // Create an open list (priority queue)
        List<Transform> openList = new List<Transform> { startNode };

        while (openList.Count > 0)
        {
            // Get the node with the lowest cost
            Transform currentNode = openList.OrderBy(n => costSoFar[n] + Heuristic(n.position, goalNode.position)).First();

            if (isDebugMode)
                Debug.Log($"Current node in A* search: {currentNode.name}");

            if (currentNode == goalNode)
            {
                if (isDebugMode)
                    Debug.Log("Goal node reached! Reconstructing path...");
                return ReconstructPath(currentNode);
            }

            openList.Remove(currentNode);

            // Get the neighbors of the current node
            foreach (Transform neighbor in GetNeighbors(currentNode))
            {
                float newCost = costSoFar[currentNode] + Vector3.Distance(currentNode.position, neighbor.position);

                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    cameFrom[neighbor] = currentNode;

                    if (isDebugMode)
                        Debug.Log($"Neighbor {neighbor.name} added to the open list with cost: {newCost}");

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        if (isDebugMode)
            Debug.LogError("A* search failed to find a path.");
        return null; // No path found
    }

    private List<Transform> ReconstructPath(Transform goalNode)
    {
        List<Transform> path = new List<Transform>();
        Transform current = goalNode;

        if (isDebugMode)
            Debug.Log("Reconstructing path...");

        while (current != null)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse(); // Reverse the path to get it from start to goal

        if (isDebugMode)
            Debug.Log("Path reconstruction complete.");
        return path;
    }

    private Transform GetClosestNode(Vector3 position)
    {
        // Find the node closest to the given position
        Transform closestNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform node in graphNodes)
        {
            float distance = Vector3.Distance(position, node.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        if (isDebugMode)
            Debug.Log($"Closest node to position {position} is {closestNode.name}");
        return closestNode;
    }

    private List<Transform> GetNeighbors(Transform node)
    {
        // Return all nodes as neighbors (adjust this as needed for your graph)
        if (isDebugMode)
            Debug.Log($"Getting neighbors for node: {node.name}");
        return graphNodes;
    }

    private float Heuristic(Vector3 a, Vector3 b)
    {
        // Use Euclidean distance as the heuristic
        return Vector3.Distance(a, b);
    }
}
