using System;
using System.Collections.Generic;
using Pancake.Linq;
using SMTD.BusPassengerGame;
using UnityEngine;

namespace SMTD.Grid
{
    public class GridObjectsController : MonoBehaviour
    {
        [SerializeField] GridSystem gridSystem;
        [SerializeField] GridInput gridInput;
        protected readonly List<GridObject> GridObjects=new();
        private IDraggable _selectedGridObject;
        private Dictionary<DefinedColors, Material> _materialDictionary = new();
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
            if (_selectedGridObject != null)
            {
                var selectedGridObjectCurrentCell = gridSystem.GetCellFromWorldPosition(_selectedGridObject.WorldPosition());
                _selectedGridObject.SetMovementLimitations(gridSystem.CheckGridObjectMovementLimitations(selectedGridObjectCurrentCell,_selectedGridObject.LocatedGridCell()));
                _selectedGridObject.OnDrag(gridInput.GetInputMapPosition(),selectedGridObjectCurrentCell.WorldPosition);
            }

        }
        #endregion
        
        #region GridInput
        private void GridInputGridInputDown()
        {
            var inputDownCell = gridSystem.GetCellFromGridPosition(gridSystem.Grid.WorldToCell(gridInput.GetInputMapPosition()));
            //check if clicked position has object 
            foreach (var gridObject in GridObjects)
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
            if (_selectedGridObject == null) return;
            _selectedGridObject.OnRelease(gridSystem.GetCellFromWorldPosition(_selectedGridObject.WorldPosition()));
            _selectedGridObject.OnGridChange -= SelectedGridObjectOnOnGridChange;
            _selectedGridObject=null;
        }
        private void SelectedGridObjectOnOnGridChange()
        {
            OnDragObjectMoved?.Invoke(_selectedGridObject);
        }
        #endregion

        public void AddGridObject(GridObject gridObject,Vector2Int gridCell)
        {
            gridObject.Init(gridSystem.GetCellFromGridPosition(new Vector3Int(gridCell.x, gridCell.y, 0)));
            GridObjects.Add(gridObject);
        }

        public void Init(Dictionary<DefinedColors, Material> materialDictionary)
        {
            _materialDictionary = materialDictionary;
        }


        public GridObject GetGridObject(Vector3Int gridPos)
        {
            return GridObjects.FirstOrDefault(x=>
                x.LocatedGridCell().CellPosition == gridPos
                );
        }
    }
}