using DG.Tweening;
using UnityEngine;

namespace SMTD.GridSystem
{
    public class GridObject : MonoBehaviour
    {
        public bool selected;
        private Vector3 _lastTargetPosition;
        private Grid _grid;
        private GridInput _gridInput;
        private Vector2 _gridSize;
        private Tween _pickedAnimation;
        public void Init(Grid grid,GridInput input,Vector2 gridSize)
        {
            _pickedAnimation = transform.DOScale(Vector3.one * 1.1f, .4f).SetLoops(-1,LoopType.Yoyo).SetAutoKill(false).SetEase(Ease.InOutBounce).Pause();
            _grid = grid;
            _gridInput = input;
            _gridSize = gridSize;
            transform.position=_grid.GetCellCenterWorld(_grid.WorldToCell(transform.position));
        }
        public void OnDrop()
        {
            selected = false;
            transform.DOMove(_lastTargetPosition,.2f);
            _pickedAnimation.Pause();
        }

        public void OnSelected()
        {
            selected=true;
            _pickedAnimation.Play();
        }

        public void OnMove( MovementOptions movementOptions)
        {
            //ray hit position
            var inputPosition = _gridInput.GetInputMapPosition();
            
            //this objects cell position
            var objectsCellPosition=_grid.WorldToCell(transform.position);

            //to check if object inside grid

            var isTryingToMoveLeft=inputPosition.x < objectsCellPosition.x;
            var isTryingToMoveRight=inputPosition.x > objectsCellPosition.x;
            var isTryingToMoveUp=inputPosition.z>objectsCellPosition.y;
            var isTryingToMoveDown=inputPosition.z<objectsCellPosition.y;
            
            var targetX = 
                !movementOptions.CanMoveRight && isTryingToMoveRight ?_grid.CellToWorld(objectsCellPosition).x + .6f
                :
                !movementOptions.CanMoveLeft && isTryingToMoveLeft ? _grid.CellToWorld(objectsCellPosition).x + .6f
                    //if not on edges, set input position
                    : inputPosition.x;
            
            var targetZ =
                !movementOptions.CanMoveUp && isTryingToMoveUp?_grid.CellToWorld(objectsCellPosition).z+.6f
                : 
                !movementOptions.CanMoveDown && isTryingToMoveDown? _grid.CellToWorld(objectsCellPosition).z+.6f
                    //if not on edges, set input position
                : inputPosition.z;
            
            var movablePosition = new Vector3(targetX, _lastTargetPosition.y + 0.1f, targetZ);

            transform.position = Vector3.MoveTowards(transform.position, movablePosition, Time.deltaTime * 20);
           
            _lastTargetPosition = _grid.GetCellCenterWorld(_grid.WorldToCell(movablePosition));
        }
        
    }
    public struct MovementOptions
    {
        public bool CanMoveLeft;
        public bool CanMoveRight;
        public bool CanMoveUp;
        public bool CanMoveDown;

        public MovementOptions(bool canMoveLeft, bool canMoveRight, bool canMoveUp, bool canMoveDown)
        {
            CanMoveLeft = canMoveLeft;
            CanMoveRight = canMoveRight;
            CanMoveUp = canMoveUp;
            CanMoveDown = canMoveDown;
        }
    }
}