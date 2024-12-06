using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pancake.Pattern;
using Sisus.Init;
using SMTD.Grid;
using UnityEngine;

namespace SMTD.BusPassengerGame
{

    [Service(typeof(PassengerManager))]
    public class PassengerManager: Publisher<PassengerManager>, IDisposable
    {
        private List<Passenger> _passengers;
        private GridSystem _gridSystem;
        private Dictionary<DefinedColors, Material> _materialDictionary;
        private GridObjectsController _gridObjectsController;
        public static event Action<List<Passenger>> OnPassengerUpdate;

        public void Initialize(List<Passenger> passengers, GridSystem gridSystem, Dictionary<DefinedColors, Material> materialDictionary,GridObjectsController gridObjectController)
        {
            _gridSystem = gridSystem;
            _materialDictionary= materialDictionary;
            _gridObjectsController = gridObjectController;
            CreatePassengers(passengers);
            GridObjectsController.OnDragObjectMoved+= GridManagerOnOnDragObjectMoved;
        }
        public void Dispose()
        {
            GridObjectsController.OnDragObjectMoved-= GridManagerOnOnDragObjectMoved;
        }
        private void CreatePassengers(List<Passenger> passengers)
        {
            _passengers=passengers;
            LineUpPassengers();
            SetColorMaterialsOfPassengers(); 
            AddObserverPassenger(0);
        }

        private void AddObserverPassenger(int index)
        {
            AddObserver(_passengers[index]);
        }

        private void AddObserverPassenger(Passenger passenger)
        {
            AddObserver(passenger);
        }
        private void SetColorMaterialsOfPassengers()
        {
            foreach (var passenger in _passengers)
            {
                passenger.SetMaterial(_materialDictionary[passenger.GetColor()]);
            }
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
            Notify(this);
        }

        public GridSystem GetGridSystem => _gridSystem;
        public GridObjectsController GetGridObjectsController => _gridObjectsController;

        public void QueueNextPassenger()
        {
            var nextPassenger=_passengers.FirstOrDefault(x => x.sitGridObject == null);
            if(nextPassenger!=null)
                AddObserverPassenger(nextPassenger);
            OnPassengerUpdate?.Invoke(_passengers);
        }

        public bool HasPassengerOnObject(GridCell cell)
        {
            foreach (var passenger in _passengers)
            {
                if (passenger.sitGridObject == null)
                    continue;
                return passenger.sitGridObject==_gridObjectsController.GetGridObject(cell.CellPosition);
            }
            return false;
        }
    }
}