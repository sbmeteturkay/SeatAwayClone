using System;
using System.Collections.Generic;
using UnityEngine;

namespace SMTD.Grid
{
    public class GridObjectsController : MonoBehaviour
    {
        [SerializeField] GridSystem gridSystem;
        [SerializeField] GridInput gridInput;
        [SerializeField] List<GridObject> gridObjects;
        private IDraggable _selectedGridObject;
        public static event Action<IDraggable> OnDragObjectMoved;

        #region MonoBehaviour
        private void Start()
        {
            gridInput.GridInputDown += GridInputGridInputDown;
            gridInput.GridInputCancelled += GridInputOnGridInputCancelled;
            for (var index = 0; index < gridObjects.Count; index++)
            {
                var gridObject = gridObjects[index];
                gridObject.Init(gridSystem.Grid, gridInput, gridSystem.GetCellFromGridPosition(new Vector3Int(index, 1, 0)));
            }
        }

        private void OnDestroy()
        {
            gridInput.GridInputDown -= GridInputGridInputDown;
            gridInput.GridInputCancelled -= GridInputOnGridInputCancelled;
        }
        
        private void Update()
        {
            _selectedGridObject?.SetMovementLimitations(gridSystem.CheckGridObjectMovementLimitations(_selectedGridObject.CurrentCell(),_selectedGridObject.LocatedGridCell()));
            _selectedGridObject?.OnDrag(gridSystem);
        }
        #endregion
        
        #region GridInput
        private void GridInputGridInputDown()
        {
            var inputDownCell = gridSystem.GetCellFromGridPosition(gridSystem.Grid.WorldToCell(gridInput.GetInputMapPosition()));
            //check if clicked position has object 
            foreach (var gridObject in gridObjects)
            {
                if (gridObject.LocatedGridCell().CellPosition != inputDownCell.CellPosition) continue;
                _selectedGridObject = gridObject;
                _selectedGridObject.OnGridChange+= SelectedGridObjectOnOnGridChange;
                gridObject.OnPick();
                break;
            }
        }
        private void GridInputOnGridInputCancelled()
        {
            if (_selectedGridObject != null)
            {
                _selectedGridObject.OnRelease();
                _selectedGridObject.OnGridChange -= SelectedGridObjectOnOnGridChange;
                _selectedGridObject=null;
            }

        }
        private void SelectedGridObjectOnOnGridChange()
        {
            OnDragObjectMoved?.Invoke(_selectedGridObject);
        }


        #endregion
    }
}