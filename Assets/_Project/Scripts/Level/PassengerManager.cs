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
            LineUpPassengers(false);
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
        private void LineUpPassengers(bool smooth=false)
        {
            var topRightCell =_gridSystem.GetCellFromGridPosition(new Vector3Int(_gridSystem.GridSize.x - 1, _gridSystem.GridSize.y - 1, 0));
            var onQueuePassengers = _passengers.FindAll(x => x.sitGridObject == null);
            for (var index = 0; index < onQueuePassengers.Count; index++)
            {
                var passenger = onQueuePassengers[index];
                //to line up passengers from 1 grid right from right edge
                passenger.Move(topRightCell.WorldPosition+new Vector3(1.5f*_gridSystem.CellSize.x,0,-index),true);
            }
            OnPassengerUpdate?.Invoke(_passengers);
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
            if (nextPassenger != null)
            {
                AddObserverPassenger(nextPassenger);
                LineUpPassengers(true);
                Notify(this);
            }
        }

        public bool HasPassengerOnObject(GridObject gridObject)
        {
            var sittingPassengers=_passengers.FindAll(x => x.sitGridObject !=null);
            foreach (var passenger in sittingPassengers)
            {
                var hasPassenger = passenger.sitGridObject.LocatedGridCell().CellPosition ==
                                   gridObject.LocatedGridCell().CellPosition;
                if (hasPassenger)
                    return true;
            }
            return false;
        }
    }
}