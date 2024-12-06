using System;
using DG.Tweening;
using UnityEngine;

namespace SMTD.Grid
{
    public class GridObject : MonoBehaviour,IDraggable
    {
        private MovementLimitations _movementLimitations;
        private Tween _pickedAnimation;
        private GridCell _locatedCell;
        public event Action OnGridChange;
        

        public void Init(GridCell targetCell)
        {
            _locatedCell=targetCell;
            targetCell.IsWalkable = false;
            //init boing animation which we will use while picked up
            _pickedAnimation = transform.DOScale(Vector3.one * 1.1f, .4f).SetLoops(-1,LoopType.Yoyo).SetAutoKill(false).SetEase(Ease.Linear).Pause();
            transform.position=_locatedCell.WorldPosition;
        }
        
        #region IDraggable implementation
        public void OnRelease(GridCell cell)
        {
            var prevLocatedCell = _locatedCell;
            transform.DOMove(cell.WorldPosition,.2f);
            _locatedCell.IsWalkable = true;
            _locatedCell = cell;
            _locatedCell.IsWalkable = false;
            _pickedAnimation.Pause();
            if(prevLocatedCell!=_locatedCell)
                OnGridChange?.Invoke();
        }

        public void OnPick()
        {
            _pickedAnimation.Play();
        }

        public void OnDrag(Vector3 inputPosition,Vector3 currentCellWorldPosition)
        {
            //ray hit position
            //this objects cell position
            
            var isTryingToMoveLeft = inputPosition.x < transform.position.x;
            var isTryingToMoveRight = inputPosition.x > transform.position.x;
            var isTryingToMoveUp = inputPosition.z>transform.position.z;
            var isTryingToMoveDown = inputPosition.z<transform.position.z;
            
            var targetX = 
                (!_movementLimitations.CanMoveRight && isTryingToMoveRight) || 
                (!_movementLimitations.CanMoveLeft && isTryingToMoveLeft)
                    ? currentCellWorldPosition.x
                    : inputPosition.x;
            var targetZ = 
                (!_movementLimitations.CanMoveUp && isTryingToMoveUp) || 
                (!_movementLimitations.CanMoveDown && isTryingToMoveDown)
                    ? currentCellWorldPosition.z
                    : inputPosition.z;
            
            var movablePosition = new Vector3(targetX, _locatedCell.WorldPosition.y + 0.1f, targetZ);

            transform.position = Vector3.MoveTowards(transform.position, movablePosition, Time.deltaTime * 20);
        }

        public Vector3 WorldPosition()
        {
            return transform.position;
        }

        public void SetMovementLimitations(MovementLimitations movementLimitations)
        {
            _movementLimitations = movementLimitations;
        }
        
        public GridCell LocatedGridCell()
        {
            return _locatedCell;
        }

        #endregion


    }
}