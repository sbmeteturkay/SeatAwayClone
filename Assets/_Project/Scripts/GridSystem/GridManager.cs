using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMTD.GridSystem
{
    public class GridManager : MonoBehaviour
    {
        //sprite to render tiles which is independent of unity's grid components
        [SerializeField] SpriteRenderer gridRenderer;
        [SerializeField] Grid grid;
        [SerializeField] GridInput gridInput;
        
        private Vector2Int _gridSize;
        private IDraggable _selectedGridObject;
        
        private Dictionary<Vector3Int, GridCell> cells = new Dictionary<Vector3Int, GridCell>();
        
        //TODO: to be initialized from level builder
        [SerializeField] List<GridObject> gridObjects;
        public Vector2Int GridSize => _gridSize;
        public static event Action<IDraggable> OnDragObjectMoved;
        #region MonoBehaviour
        private void Start()
        {
            gridInput.GridInputDown += GridInputGridInputDown;
            gridInput.GridInputCancelled += GridInputOnGridInputCancelled;
        }

        private void OnDestroy()
        {
            gridInput.GridInputDown -= GridInputGridInputDown;
            gridInput.GridInputCancelled -= GridInputOnGridInputCancelled;
        }

        private void Update()
        {
            _selectedGridObject?.SetMovementLimitations(CheckGridObjectMovementLimitations());
            _selectedGridObject?.OnDrag();
        }

        #endregion
       
        #region GridInput

        private void GridInputGridInputDown()
        {
            var inputDownCellPosition = grid.GetCellCenterWorld(grid.WorldToCell(gridInput.GetInputMapPosition()));
            //check if clicked position has object 
            foreach (var gridObject in gridObjects)
            {
                if (gridObject.transform.position != inputDownCellPosition) continue;
                _selectedGridObject = gridObject;
                _selectedGridObject.OnGridChange+= SelectedGridObjectOnOnGridChange;
                gridObject.OnPick();
                break;
            }
        }

        private void SelectedGridObjectOnOnGridChange()
        {
            OnDragObjectMoved?.Invoke(_selectedGridObject);
        }

        private void GridInputOnGridInputCancelled()
        {
            if (_selectedGridObject != null)
            {
                _selectedGridObject?.OnRelease();
                _selectedGridObject.OnGridChange -= SelectedGridObjectOnOnGridChange;
                _selectedGridObject=null;
            }

        }

        #endregion
        
        public void InitGrid(Vector2Int gridSize, Vector3 cellSize)
        {
            _gridSize = gridSize;
            grid.cellSize = cellSize;
            InitGridRenderer(_gridSize);
            CreateGridCells();
            foreach (var gridObject in gridObjects)
            {
                gridObject.Init(grid,gridInput);
            }
        }

        public IEnumerator FollowPath(List<GridCell> path,GameObject target)
        {
            foreach (var cell in path)
            {
                Vector3 targetPosition = new Vector3(cell.WorldPosition.x+grid.cellSize.x/2, 0, cell.WorldPosition.z+grid.cellSize.y/2);
                while (Vector3.Distance(target.transform.position, targetPosition) > 0.1f)
                {
                   target.transform.position =
                        Vector3.MoveTowards(target.transform.position, targetPosition, 4f * Time.deltaTime);
                    yield return null;
                }
            }
        }

        private void CreateGridCells()
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int z = 0; z < _gridSize.y; z++)
                {
                    Vector3Int cellPosition = new Vector3Int(x, z, 0);
                    Vector3 worldPosition = grid.CellToWorld(cellPosition);
                    GridCell cell = new GridCell
                    {
                        CellPosition = cellPosition,
                        WorldPosition = worldPosition,
                        IsWalkable = true // Varsayılan olarak geçilebilir
                    };

                    cells[cellPosition] = cell;
                }
            }
        }
        public GridCell GetCell(Vector3Int cellPosition)
        {
            return cells.GetValueOrDefault(cellPosition);
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
        private MovementLimitations CheckGridObjectMovementLimitations()
        {
            // Hedef pozisyonu hesapla
            var currentGridPosition = _selectedGridObject.CurrentGridPosition();
            
            var onTopCell = currentGridPosition.y+1==_gridSize.y;
            var onBottomCell = currentGridPosition.y==0;
            
            var onFarRightCell = currentGridPosition.x+1==_gridSize.x;
            var onFarLeftCell = currentGridPosition.x==0;

            var leftGrid =  currentGridPosition+ Vector3Int.left;
            var rightGrid =  currentGridPosition+ Vector3Int.right;
            var upGrid =  currentGridPosition+ Vector3Int.up;
            var downGrid =  currentGridPosition+ Vector3Int.down;
            
            var leftCellOccupied = false;
            var rightCellOccupied = false;
            var upCellOccupied = false;
            var downCellOccupied = false;
            foreach (var gridObject in gridObjects)
            {
                leftCellOccupied = leftCellOccupied||grid.WorldToCell(gridObject.transform.position) == leftGrid;
                rightCellOccupied = rightCellOccupied||grid.WorldToCell(gridObject.transform.position) == rightGrid;
                upCellOccupied = upCellOccupied||grid.WorldToCell(gridObject.transform.position) == upGrid;
                downCellOccupied = downCellOccupied||grid.WorldToCell(gridObject.transform.position) == downGrid;
            }

            return new MovementLimitations(
                    !onFarLeftCell && !leftCellOccupied,
                    !onFarRightCell && !rightCellOccupied,
                    !onTopCell && !upCellOccupied,
                    !onBottomCell && !downCellOccupied
                );

        }

        public Vector3 GetWorldPositionFromGridPosition(Vector3Int gridPosition)
        {
            return grid.CellToWorld(gridPosition);
        }
    }
}
