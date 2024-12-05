using System;
using DG.Tweening;
using UnityEngine;

namespace SMTD.Grid
{
    public class GridObject : MonoBehaviour,IDraggable
    {
        private UnityEngine.Grid _grid;
        private GridInput _gridInput;
        private MovementLimitations _movementLimitations;
        private Tween _pickedAnimation;
        private GridCell _locatedCell;
        private GridCell _currentCell;
        public event Action OnGridChange;
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
            if(_locatedCell!=_currentCell)
                OnGridChange?.Invoke();
            transform.DOMove(_currentCell.WorldPosition,.2f);
            _locatedCell.IsWalkable = true;
            _locatedCell = _currentCell;
            _locatedCell.IsWalkable = false;
            _pickedAnimation.Pause();
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
    }

}