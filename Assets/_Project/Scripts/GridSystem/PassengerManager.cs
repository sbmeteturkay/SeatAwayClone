using System.Collections.Generic;
using System.Linq;
using SMTD.Grid;
using UnityEngine;

namespace SMTD.BusPassengerGame
{
    public class PassengerManager:MonoBehaviour
    {
        //todo:init from level scriptable object
         private List<Passenger> _passengers;
         private GridSystem _gridSystem;

        #region  Monobehaviour
        private void Start()
        {
            GridObjectsController.OnDragObjectMoved+= GridManagerOnOnDragObjectMoved;
        }
    
        private void OnDestroy()
        {
            GridObjectsController.OnDragObjectMoved-= GridManagerOnOnDragObjectMoved;
        }

        #endregion

        public void Initialize(List<Passenger> passengers, GridSystem gridSystem)
        {
            _gridSystem = gridSystem;
            CreatePassengers(passengers);
        }
        private void CreatePassengers(List<Passenger> passengers)
        {
            _passengers=passengers;
            LineUpPassengers();
        }

        private void LineUpPassengers()
        {
            var topRightCell =
                _gridSystem.GetCellFromGridPosition(new Vector3Int(_gridSystem.GridSize.x - 1, _gridSystem.GridSize.y - 1, 0));
            for (var index = 0; index < _passengers.Count; index++)
            {
                var passenger = _passengers[index];
                
                //to line up passengers from 1 grid right from right edge
                passenger.Move(topRightCell.WorldPosition+new Vector3(1.5f*_gridSystem.CellSize.x,0,-index));
            }
        }
        private void GridManagerOnOnDragObjectMoved(IDraggable obj)
        {
            GridCell startCell =_gridSystem.GetCell(new Vector3Int(_gridSystem.GridSize.x-1, _gridSystem.GridSize.y - 1, 0)); // Sağ üst hücre
            GridCell targetCell = _gridSystem.GetCell(new Vector3Int(0 , 0, 0)); // Sol alt hücre
            List<GridCell> path = PathFinder.FindPath(startCell, targetCell, _gridSystem);
            
            if (path != null)
            {
                var firstPassenger=_passengers.First(x => x.state == State.onLine);
                StartCoroutine(_gridSystem.FollowPath(path,firstPassenger.transform.gameObject));
                firstPassenger.state = State.moving;
                foreach (var passenger in _passengers)
                {
                    if (passenger.state == State.onLine)
                    {
                        passenger.Move(passenger.transform.position+Vector3.up);
                    }
                }
            }
            else
            {
                Debug.Log("Yol bulunamadı!");
            }
        }
    }
}