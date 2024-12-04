using System;
using DG.Tweening;
using UnityEngine;

namespace SMTD.GridSystem
{
    public class GridObject : MonoBehaviour,IDraggable
    {
        private Grid _grid;
        private GridInput _gridInput;
        private MovementLimitations _movementLimitations;
        private Vector3 _lastInteractedCellPosition;
        private Vector3 _currentCellPosition;
        private Tween _pickedAnimation;
        public event Action OnGridChange;
        public void Init(Grid grid,GridInput input)
        {
            _grid = grid;
            _gridInput = input;
            
            //init boing animation which we will use while picked up
            _pickedAnimation = transform.DOScale(Vector3.one * 1.1f, .4f).SetLoops(-1,LoopType.Yoyo).SetAutoKill(false).SetEase(Ease.Linear).Pause();

            transform.position=_grid.GetCellCenterWorld(_grid.WorldToCell(transform.position));
        }
        
        #region IDraggable implementation
        public void OnRelease()
        {
            if(_currentCellPosition!=_lastInteractedCellPosition)
                OnGridChange?.Invoke();
            transform.DOMove(_lastInteractedCellPosition,.2f);
            _currentCellPosition = _lastInteractedCellPosition;
            _pickedAnimation.Pause();
        }

        public void OnPick()
        {
            _currentCellPosition=transform.position;;
            _pickedAnimation.Play();
        }

        public void OnDrag()
        {
            //ray hit position
            var inputPosition = _gridInput.GetInputMapPosition();
            
            //this objects cell position
            var objectsCellPosition=_grid.WorldToCell(transform.position);

            var isTryingToMoveLeft = inputPosition.x < transform.position.x;
            var isTryingToMoveRight = inputPosition.x > transform.position.x;
            var isTryingToMoveUp = inputPosition.z>transform.position.z;
            var isTryingToMoveDown = inputPosition.z<transform.position.z;
            
            var targetX = 
                !_movementLimitations.CanMoveRight && isTryingToMoveRight ?_grid.CellToWorld(objectsCellPosition).x + .6f
                :
                !_movementLimitations.CanMoveLeft && isTryingToMoveLeft ? _grid.CellToWorld(objectsCellPosition).x + .6f
                    //if not on edges, set input position
                    : inputPosition.x;
            
            var targetZ =
                !_movementLimitations.CanMoveUp && isTryingToMoveUp?_grid.CellToWorld(objectsCellPosition).z+.6f
                : 
                !_movementLimitations.CanMoveDown && isTryingToMoveDown? _grid.CellToWorld(objectsCellPosition).z+.6f
                    //if not on edges, set input position
                : inputPosition.z;
            
            var movablePosition = new Vector3(targetX, _lastInteractedCellPosition.y + 0.1f, targetZ);

            transform.position = Vector3.MoveTowards(transform.position, movablePosition, Time.deltaTime * 20);
            _lastInteractedCellPosition = _grid.GetCellCenterWorld(_grid.WorldToCell(movablePosition));
        }
        public void SetMovementLimitations(MovementLimitations movementLimitations)
        {
            _movementLimitations = movementLimitations;
        }


        public Vector3Int CurrentGridPosition()
        {
            return _grid.WorldToCell(transform.position);
        }
        #endregion
    }

}