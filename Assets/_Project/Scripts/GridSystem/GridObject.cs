using System;
using DG.Tweening;
using Sisus.Init;
using SMTD.BusPassengerGame;
using UnityEngine;

namespace SMTD.Grid
{
    public class GridObject : MonoBehaviour<Renderer>,IDraggable, IColorable
    {
        private UnityEngine.Grid _grid;
        private GridInput _gridInput;
        private MovementLimitations _movementLimitations;
        private Tween _pickedAnimation;
        private GridCell _locatedCell;
        private GridCell _currentCell;
        private DefinedColors _definedColor;
        public event Action OnGridChange;
        
        protected override void Init(Renderer argument)
        {
            Renderer = argument;
        }
        public void Init(UnityEngine.Grid grid,GridInput input,GridCell targetCell)
        {
            _grid = grid;
            _gridInput = input;
            _locatedCell=targetCell;
            _currentCell=_locatedCell;
            targetCell.IsWalkable = false;
            //init boing animation which we will use while picked up
            _pickedAnimation = transform.DOScale(Vector3.one * 1.1f, .4f).SetLoops(-1,LoopType.Yoyo).SetAutoKill(false).SetEase(Ease.Linear).Pause();

            transform.position=_currentCell.WorldPosition;
        }
        
        #region IDraggable implementation
        public void OnRelease()
        {
            var prevLocatedCell = _locatedCell;
            transform.DOMove(_currentCell.WorldPosition,.2f);
            _locatedCell.IsWalkable = true;
            _locatedCell = _currentCell;
            _locatedCell.IsWalkable = false;
            _pickedAnimation.Pause();
            if(prevLocatedCell!=_locatedCell)
                OnGridChange?.Invoke();
        }

        public void OnPick()
        {
            _pickedAnimation.Play();
        }

        public void OnDrag(GridSystem gridSystem)
        {
            //ray hit position
            var inputPosition = _gridInput.GetInputMapPosition();
            
            //this objects cell position
            var currentCell=gridSystem.GetCellFromGridPosition(_grid.WorldToCell(transform.position));
            
            var isTryingToMoveLeft = inputPosition.x < transform.position.x;
            var isTryingToMoveRight = inputPosition.x > transform.position.x;
            var isTryingToMoveUp = inputPosition.z>transform.position.z;
            var isTryingToMoveDown = inputPosition.z<transform.position.z;
            
            var targetX = 
                (!_movementLimitations.CanMoveRight && isTryingToMoveRight) || 
                (!_movementLimitations.CanMoveLeft && isTryingToMoveLeft)
                    ? currentCell.WorldPosition.x
                    : inputPosition.x;
            var targetZ = 
                (!_movementLimitations.CanMoveUp && isTryingToMoveUp) || 
                (!_movementLimitations.CanMoveDown && isTryingToMoveDown)
                    ? currentCell.WorldPosition.z
                    : inputPosition.z;
            
            var movablePosition = new Vector3(targetX, _locatedCell.WorldPosition.y + 0.1f, targetZ);

            transform.position = Vector3.MoveTowards(transform.position, movablePosition, Time.deltaTime * 20);
            _currentCell = currentCell;
        }
        public void SetMovementLimitations(MovementLimitations movementLimitations)
        {
            _movementLimitations = movementLimitations;
        }
        
        public GridCell LocatedGridCell()
        {
            return _locatedCell;
        }
        public GridCell CurrentCell()
        {
            return _currentCell;
        }
        #endregion

        #region IColorable implementattion

        public Renderer Renderer { get; set; }

        public DefinedColors GetColor()
        {
            return _definedColor;
        }

        public void SetColor(DefinedColors color)
        {
            _definedColor=color;
        }

        public void SetMaterial(Material material)
        {
            Renderer.material = material;
        }
        #endregion

    }

}