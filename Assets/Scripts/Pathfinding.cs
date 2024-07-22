using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private GridManager gridManager;

    public Pathfinding(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    public List<Tile> FindPath(Tile startTile, Tile targetTile)
    {
        List<Tile> openList = new List<Tile>();
        HashSet<Tile> closedList = new HashSet<Tile>();

        openList.Add(startTile);

        while (openList.Count > 0)
        {
            Tile currentTile = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentTile.FCost || openList[i].FCost == currentTile.FCost && openList[i].HCost < currentTile.HCost)
                {
                    currentTile = openList[i];
                }
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == targetTile)
            {
                return RetracePath(startTile, targetTile);
            }

            foreach (Tile neighbor in GetNeighbors(currentTile))
            {
                if (!neighbor.isWalkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = currentTile.GCost + GetDistance(currentTile, neighbor);
                if (newCostToNeighbor < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = newCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, targetTile);
                    neighbor.Parent = currentTile;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private List<Tile> RetracePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.Parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(Tile tileA, Tile tileB)
    {
        int dstX = Mathf.Abs(tileA.x - tileB.x);
        int dstY = Mathf.Abs(tileA.y - tileB.y);
        return dstX + dstY;
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int checkX = tile.x + dx[i];
            int checkY = tile.y + dy[i];

            if (checkX >= 0 && checkX < gridManager.Width && checkY >= 0 && checkY < gridManager.Height)
            {
                neighbors.Add(gridManager.GetTileAt(checkX, checkY));
            }
        }

        return neighbors;
    }
}
