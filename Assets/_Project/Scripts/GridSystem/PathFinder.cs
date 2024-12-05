using System.Collections.Generic;
using UnityEngine;

namespace SMTD.Grid
{
    public static class PathFinder
    {
        private static List<GridCell> GetNeighbors(GridCell cell, GridSystem gridSystem)
        {
            List<GridCell> neighbors = new List<GridCell>();
            Vector3Int[] directions = new Vector3Int[]
            {
                new Vector3Int(1, 0, 0), // Sağ
                new Vector3Int(-1, 0, 0), // Sol
                new Vector3Int(0, 1, 0), // Yukarı
                new Vector3Int(0, -1, 0) // Aşağı
            };

            foreach (var direction in directions)
            {
                Vector3Int neighborPos = cell.CellPosition + direction;
                var neighbor = gridSystem.GetCell(neighborPos);
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }
        public static List<GridCell> FindPath(GridCell startCell, GridCell targetCell, GridSystem gridSystem)
        {
            List<GridCell> openList = new List<GridCell>();
            HashSet<GridCell> closedList = new HashSet<GridCell>();

            openList.Add(startCell);

            while (openList.Count > 0)
            {
                GridCell currentCell = openList[0];

                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].F < currentCell.F || 
                        (openList[i].F == currentCell.F && openList[i].H < currentCell.H))
                    {
                        currentCell = openList[i];
                    }
                }

                openList.Remove(currentCell);
                closedList.Add(currentCell);

                if (currentCell == targetCell)
                {
                    return RetracePath(startCell, targetCell);
                }

                foreach (var neighbor in GetNeighbors(currentCell, gridSystem))
                {
                    if (!neighbor.IsWalkable || closedList.Contains(neighbor))
                        continue;

                    int newCostToNeighbor = currentCell.G + GetDistance(currentCell, neighbor);
                    if (newCostToNeighbor < neighbor.G || !openList.Contains(neighbor))
                    {
                        neighbor.G = newCostToNeighbor;
                        neighbor.H = GetDistance(neighbor, targetCell);
                        neighbor.Parent = currentCell;

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }

            return null; // Yol bulunamadı
        }
        private static int GetDistance(GridCell a, GridCell b)
        {
            int distX = Mathf.Abs(a.CellPosition.x - b.CellPosition.x);
            int distZ = Mathf.Abs(a.CellPosition.y - b.CellPosition.y);
            return distX + distZ; // Manhattan distance
        }
        private static List<GridCell> RetracePath(GridCell startCell, GridCell endCell)
        {
            List<GridCell> path = new List<GridCell>();
            GridCell currentCell = endCell;

            while (currentCell != startCell)
            {
                path.Add(currentCell);
                currentCell = currentCell.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}