using DG.Tweening;
using UnityEngine;

namespace SMTD.GridSystem
{
    public class GridObject : MonoBehaviour,IDraggable
    {
        private Grid _grid;
        private GridInput _gridInput;
        private MovementLimitations _movementLimitations;
        private Vector3 _lastTargetPosition;
        private Tween _pickedAnimation;
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
            transform.DOMove(_lastTargetPosition,.2f);
            _pickedAnimation.Pause();
        }

        public void OnPick()
        {
            _pickedAnimation.Play();
        }

        public void OnDrag()
        {
            //ray hit position
            var inputPosition = _gridInput.GetInputMapPosition();
            
            //this objects cell position
            var objectsCellPosition=_grid.WorldToCell(transform.position);
            Debug.Log(objectsCellPosition.x);
            //to check if object inside grid

            var isTryingToMoveLeft=inputPosition.x < objectsCellPosition.x;
            var isTryingToMoveRight=inputPosition.x > objectsCellPosition.x;
            var isTryingToMoveUp=inputPosition.z>objectsCellPosition.y;
            var isTryingToMoveDown=inputPosition.z<objectsCellPosition.y;
            
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
            
            var movablePosition = new Vector3(targetX, _lastTargetPosition.y + 0.1f, targetZ);

            transform.position = Vector3.MoveTowards(transform.position, movablePosition, Time.deltaTime * 20);
            _lastTargetPosition = _grid.GetCellCenterWorld(_grid.WorldToCell(movablePosition));
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