using System.Collections.Generic;
using UnityEngine;

namespace SMTD.GridSystem
{
    public class GridManager : MonoBehaviour
    {
        //just a sprite to render tiles which is independent of unity's grid components
        [SerializeField] SpriteRenderer gridRenderer;
        [SerializeField] Grid grid;
        [SerializeField] GridInput gridInput;
        [SerializeField] List<GridObject> gridObjects;
        [SerializeField] Vector2 gridSize;
        GridObject _selectedGridObject;
        private void Start()
        {
            gridInput.GridInputDown += GridInputGridInputDown;
            gridInput.GridInputCancelled += GridInputOnGridInputCancelled;
            foreach (var gridObject in gridObjects)
            {
                gridObject.Init(grid,gridInput,gridSize);
            }
        }

        private void OnDestroy()
        {
            gridInput.GridInputDown -= GridInputGridInputDown;
            gridInput.GridInputCancelled -= GridInputOnGridInputCancelled;
        }

        private void Update()
        {
            _selectedGridObject?.OnMove(CanGridObjectMoveInDirection());
        }

        private void GridInputGridInputDown()
        {
            var inputDownCellPosition = grid.GetCellCenterWorld(grid.WorldToCell(gridInput.GetInputMapPosition()));
            //check if clicked position has object 
            foreach (var gridObject in gridObjects)
            {
                if (gridObject.transform.position != inputDownCellPosition) continue;
                _selectedGridObject = gridObject;
                gridObject.OnSelected();
                break;
            }
        }
        private void GridInputOnGridInputCancelled()
        {
            _selectedGridObject?.OnDrop();
            _selectedGridObject=null;
        }

        private void SetGridRendererSize(Vector2 tile)
        {
            gridRenderer.size = tile;
        }
        private MovementOptions CanGridObjectMoveInDirection()
        {
            // Hedef pozisyonu hesapla
            var currentGridPosition = grid.WorldToCell(_selectedGridObject.transform.position);
            
            var onTopCell = currentGridPosition.y+2>(gridSize.y/2);
            var onBottomCell = currentGridPosition.y-1<-(gridSize.y/2);
            
            var onFarRightCell = currentGridPosition.x+2>(gridSize.x/2);
            var onFarLeftCell = currentGridPosition.x-1<-(gridSize.x/2);

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

            return new MovementOptions(
                    !onFarLeftCell && !leftCellOccupied,
                    !onFarRightCell && !rightCellOccupied,
                    !onTopCell && !upCellOccupied,
                    !onBottomCell && !downCellOccupied
                );

        }
    }
}
