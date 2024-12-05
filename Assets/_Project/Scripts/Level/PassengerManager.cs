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
    public class PassengerManager: Publisher<GridObjectsController, GridSystem>, IDisposable, Pancake.Pattern.IObserver<SitPublish>
    {
        private List<Passenger> _passengers;
        private GridSystem _gridSystem;
        private Dictionary<DefinedColors, Material> _materialDictionary;
        private GridObjectsController _gridObjectsController;

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
            _passengers[index].sitPublish.AddObserver(this);
        }

        private void AddObserverPassenger(Passenger passenger)
        {
            AddObserver(passenger);
            passenger.sitPublish.AddObserver(this);
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
            Notify(_gridObjectsController,_gridSystem);
        }

        public void OnNotify(SitPublish value)
        {
            value.Passenger.sitPublish.RemoveObserver(this);
            RemoveObserver(value.Passenger);
            var targetGrid =
                _gridObjectsController.GetGridObject(value.Passenger.lastTargetGridCell.CellPosition + Vector3Int.down);
            
            value.Passenger.transform.SetParent(targetGrid.transform);
            value.Passenger.transform.rotation=Quaternion.identity;
            value.Passenger.transform.DOLocalJump(Vector3.up/2, 1, 1, .5f);
            value.Passenger.sitGridObject = targetGrid;
            var nextPassenger=_passengers.FirstOrDefault(x => x.sitGridObject == null);
            if(nextPassenger!=null)
                AddObserverPassenger(nextPassenger);
        }
    }
}