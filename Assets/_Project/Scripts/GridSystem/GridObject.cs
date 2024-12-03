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
        private Tween _pickedAnimation;
        public void Init(Grid grid,GridInput input)
        {
            _pickedAnimation = transform.DOScale(Vector3.one * 1.1f, .4f).SetLoops(-1,LoopType.Yoyo).SetAutoKill(false).SetEase(Ease.InOutBounce).Pause();
            _grid = grid;
            _gridInput = input;
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

        public void OnMove()
        {
            //ray hit position
            var inputPosition = _gridInput.GetInputMapPosition();
            
            //world position of ray hit cell
            var cellPosition= _grid.GetCellCenterWorld(_grid.WorldToCell(inputPosition));
            
            //movable position
            Vector3 movablePosition = new Vector3(inputPosition.x, _lastTargetPosition.y + 0.5f, inputPosition.z);

            transform.position = Vector3.MoveTowards(transform.position, movablePosition, Time.deltaTime * 10);
            _lastTargetPosition = cellPosition;
        }
    }
}