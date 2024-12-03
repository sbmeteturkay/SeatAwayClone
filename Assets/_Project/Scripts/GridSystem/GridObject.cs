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

        public void OnMove( bool canMoveLeft=true, bool canMoveRight=true)
        {
            //ray hit position
            var inputPosition = _gridInput.GetInputMapPosition();
            
            //world position of ray hit cell
            var cellPosition= _grid.GetCellCenterWorld(_grid.WorldToCell(inputPosition));
            
            //this objects cell position
            var objectsCellPosition=_grid.WorldToCell(transform.position);

            //to check if object inside grid
            var onTopCell = objectsCellPosition.y+2>(_gridSize.y/2);
            var onBottomCell = objectsCellPosition.y-1<-(_gridSize.y/2);
            var onFarRightCell = objectsCellPosition.x+2>(_gridSize.x/2);
            var onFarLeftCell = objectsCellPosition.x-1<-(_gridSize.x/2);

            var targetX = onFarRightCell &&
                          //is trying to move right
                          inputPosition.x > objectsCellPosition.x
                ?
                //set far right x
                _grid.CellToWorld(objectsCellPosition).x + .6f
                : onFarLeftCell && 
                  //is trying to move left
                  inputPosition.x < objectsCellPosition.x
                    //set far right x
                    ? _grid.CellToWorld(objectsCellPosition).x + .6f
                    //if not on edges, set input position
                    : inputPosition.x;
            
            var targetZ = onTopCell&&inputPosition.z>objectsCellPosition.y?_grid.CellToWorld(objectsCellPosition).z+.6f: onBottomCell&&inputPosition.z<objectsCellPosition.y? _grid.CellToWorld(objectsCellPosition).z+.6f:inputPosition.z;
            
            var movablePosition = new Vector3(targetX, _lastTargetPosition.y + 0.1f, targetZ);

            transform.position = Vector3.MoveTowards(transform.position, movablePosition, Time.deltaTime * 20);
           
            _lastTargetPosition = _grid.GetCellCenterWorld(_grid.WorldToCell(movablePosition));
        }
        
    }
}