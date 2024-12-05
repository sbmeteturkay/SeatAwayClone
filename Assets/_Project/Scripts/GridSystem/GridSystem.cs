using System.Collections;
using System.Collections.Generic;
using Sisus.Init;
using UnityEngine;

namespace SMTD.Grid
{
    public class GridSystem : MonoBehaviour
    {
        //sprite to render tiles which is independent of unity's grid components
        [SerializeField] SpriteRenderer gridRenderer;
        [SerializeField] UnityEngine.Grid grid;
        private Vector2Int _gridSize;
        private readonly Dictionary<Vector3Int, GridCell> _cells = new Dictionary<Vector3Int, GridCell>();
        public Vector2Int GridSize => _gridSize;
        public Vector3 CellSize => grid.cellSize;
        public UnityEngine.Grid Grid => grid;
        readonly GridCell _emptyCell= new(false);
        public void InitGrid(Vector2Int gridSize, Vector3 cellSize)
        {
            _gridSize = gridSize;
            grid.cellSize = cellSize;
            InitGridRenderer(_gridSize);
            CreateGridCells();
        }


        private void CreateGridCells()
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int z = 0; z < _gridSize.y; z++)
                {
                    Vector3Int cellPosition = new Vector3Int(x, z, 0);
                    Vector3 worldPosition = grid.CellToWorld(cellPosition)+new Vector3(CellSize.x/2,0,CellSize.y/2);
                    GridCell cell = new GridCell
                    {
                        CellPosition = cellPosition,
                        WorldPosition = worldPosition,
                        IsWalkable = true // Varsayılan olarak geçilebilir
                    };

                    _cells[cellPosition] = cell;
                }
            }
        }
        public GridCell GetCell(Vector3Int cellPosition)
        {
            return _cells.GetValueOrDefault(cellPosition);
        }

        private void InitGridRenderer(Vector2 size)
        {
            gridRenderer.size = size;
            gridRenderer.transform.localScale = new Vector3(grid.cellSize.x, grid.cellSize.y, 1);
            //fit grid render to grid components grid tiles
            gridRenderer.transform.position = new Vector3(
                gridRenderer.transform.position.x + ((size.x / 2)*grid.cellSize.x),
                gridRenderer.transform.position.y,
                gridRenderer.transform.position.z + (size.y / 2)*grid.cellSize.y);;
        }
        public MovementLimitations CheckGridObjectMovementLimitations(GridCell selectedGridCell,GridCell ignoreCell)
        {
            var currentGridPosition = selectedGridCell.CellPosition;
            var leftCell =  GetCellFromGridPosition(currentGridPosition+ Vector3Int.left);
            var rightCell =  GetCellFromGridPosition(currentGridPosition+ Vector3Int.right);
            var upCell = GetCellFromGridPosition(currentGridPosition+ Vector3Int.up);
            var downCell = GetCellFromGridPosition( currentGridPosition+ Vector3Int.down);
            return new MovementLimitations(
                     leftCell==ignoreCell||leftCell.IsWalkable,
                    rightCell==ignoreCell||rightCell.IsWalkable,
                     upCell==ignoreCell||upCell.IsWalkable,
                     downCell==ignoreCell||downCell.IsWalkable
                );
        }
        public GridCell GetCellFromGridPosition(Vector3Int gridPosition)
        {
            return _cells.GetValueOrDefault(gridPosition,_emptyCell);
        }
    }
}
