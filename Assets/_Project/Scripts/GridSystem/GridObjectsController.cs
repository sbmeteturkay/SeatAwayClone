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
        private List<GridObject> _gridObjects=new();
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
            _selectedGridObject?.SetMovementLimitations(gridSystem.CheckGridObjectMovementLimitations(_selectedGridObject.CurrentCell(),_selectedGridObject.LocatedGridCell()));
            _selectedGridObject?.OnDrag(gridSystem);
        }
        #endregion
        
        #region GridInput
        private void GridInputGridInputDown()
        {
            var inputDownCell = gridSystem.GetCellFromGridPosition(gridSystem.Grid.WorldToCell(gridInput.GetInputMapPosition()));
            //check if clicked position has object 
            foreach (var gridObject in _gridObjects)
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

        public void AddGridObject(GridObject gridObject,Vector2Int gridCell)
        {
            gridObject.Init(gridSystem.Grid, gridInput, gridSystem.GetCellFromGridPosition(new Vector3Int(gridCell.x, gridCell.y, 0)));
            gridObject.SetMaterial(_materialDictionary[gridObject.GetColor()]);
            _gridObjects.Add(gridObject);
        }

        public void Init(Dictionary<DefinedColors, Material> materialDictionary)
        {
            _materialDictionary = materialDictionary;
        }

        public DefinedColors GetGridObjectWithColor(IColorable colorable)
        {
            return _gridObjects.FirstOrDefault(x => x.GetColor() == colorable.GetColor()).GetColor();
        }
        public List<GridObject> GetGridObjectsWithColor(IColorable colorable)
        {
            return _gridObjects.FindAll(x => x.GetColor() == colorable.GetColor());
        }
        public GridObject GetGridObject(Vector3Int gridPos)
        {
            return _gridObjects.FirstOrDefault(x=>
                x.LocatedGridCell().CellPosition == gridPos
                );
        }
    }
}