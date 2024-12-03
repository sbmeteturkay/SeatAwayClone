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
            foreach (var gridObject in gridObjects)
            {
                if (gridObject.selected)
                {
                    gridObject.OnMove(CanGridObjectMoveUp(gridObject), CanGridObjectMoveDown(gridObject));
                }
            }
        }

        private void GridInputGridInputDown()
        {
            var inputDownCellPosition = grid.GetCellCenterWorld(grid.WorldToCell(gridInput.GetInputMapPosition()));
            //check if clicked position has object 
            foreach (var gridObject in gridObjects)
            {
                if (gridObject.transform.position != inputDownCellPosition) continue;
                gridObject.OnSelected();
                break;
            }
        }
        private void GridInputOnGridInputCancelled()
        {
            foreach (var gridObject in gridObjects)
            {
                if (gridObject.selected)
                {
                    gridObject.OnDrop();
                }
            }
        }

        private void SetGridRendererSize(Vector2 tile)
        {
            gridRenderer.size = tile;
        }

        private bool CanGridObjectMoveUp(GridObject checkGridObject)
        {
            var currentGridPosition=grid.WorldToCell(checkGridObject.transform.position);
            return currentGridPosition.y<(gridSize.y/2);
        }
        private bool CanGridObjectMoveDown(GridObject checkGridObject)
        {
            var currentGridPosition=grid.WorldToCell(checkGridObject.transform.position);
            return currentGridPosition.y>-(gridSize.y/2);
        }
    }
}