using System;
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
        
        Vector2 _gridSize;
        IDraggable _selectedGridObject;
        
        //TODO: to be initialized from level builder
        [SerializeField] List<GridObject> gridObjects;
        private bool ShouldFitX => (int)_gridSize.x % 2 == 1;
        private bool ShouldFitY => (int)_gridSize.y % 2 == 1;

        public void InitGrid(Vector2 gridSize)
        {
            _gridSize = gridSize;
            SetGridRendererSize(_gridSize);
            //fit grid render to grid components grid tiles
            gridRenderer.transform.position=new Vector3( 
                gridRenderer.transform.position.x+ (ShouldFitX?grid.cellSize.x/2:0),
                gridRenderer.transform.position.y,
                gridRenderer.transform.position.z+(ShouldFitY?grid.cellSize.y/2:0));
        }
        #region MonoBehaviour
        private void Start()
        {
            gridInput.GridInputDown += GridInputGridInputDown;
            gridInput.GridInputCancelled += GridInputOnGridInputCancelled;
            foreach (var gridObject in gridObjects)
            {
                gridObject.Init(grid,gridInput);
            }
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

        private void GridInputGridInputDown()
        {
            var inputDownCellPosition = grid.GetCellCenterWorld(grid.WorldToCell(gridInput.GetInputMapPosition()));
            //check if clicked position has object 
            foreach (var gridObject in gridObjects)
            {
                if (gridObject.transform.position != inputDownCellPosition) continue;
                _selectedGridObject = gridObject;
                gridObject.OnPick();
                break;
            }
        }
        private void GridInputOnGridInputCancelled()
        {
            _selectedGridObject?.OnRelease();
            _selectedGridObject=null;
        }

        private void SetGridRendererSize(Vector2 tile)
        {
            gridRenderer.size = tile;
        }
        private MovementLimitations CheckGridObjectMovementLimitations()
        {
            // Hedef pozisyonu hesapla
            var currentGridPosition = _selectedGridObject.CurrentGridPosition();
            
            var onTopCell = currentGridPosition.y+(ShouldFitY?1:2)>(_gridSize.y/2);
            var onBottomCell = currentGridPosition.y-1<-(_gridSize.y/2);
            
            var onFarRightCell = currentGridPosition.x+(ShouldFitX?1:2)>(_gridSize.x/2);
            var onFarLeftCell = currentGridPosition.x-1<-(_gridSize.x/2);

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
