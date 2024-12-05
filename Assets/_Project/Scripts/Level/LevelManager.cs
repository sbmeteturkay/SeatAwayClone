using System.Collections.Generic;
using Pancake.Pools;
using Sisus.Init;
using SMTD.BusPassengerGame;
using SMTD.Grid;
using SMTD.Grid.Level;
using UnityEngine;

namespace SMTD.LevelSystem
{
    public class LevelManager : MonoBehaviour<GridSystem, PassengerManager, GridObjectsController>
    {
        [SerializeField] LevelData levelData;
        PassengerManager _passengerManager;
        GridObjectsController _gridObjectsController;
        GridSystem _gridSystem;

        protected override void Init(GridSystem firstArgument, PassengerManager secondArgument, GridObjectsController controller)
        {
            Debug.Log(firstArgument);
            _gridSystem = firstArgument;
            _passengerManager = secondArgument;
            _gridObjectsController = controller;
            _gridSystem.InitGrid(levelData.GridSize,levelData.CellSize);
            InitPassengerManager();
            InitGridObjectsController();
        }

        private void InitGridObjectsController()
        {
            _gridObjectsController.Init(levelData.levelDesignData.materialDictionary);
            for (var x = 0; x < levelData.Grid.GetLength(0); x++)
            {
                for (var y = 0; y < levelData.Grid.GetLength(1); y++)
                {
                    var cell = levelData.Grid[x, y];
                    if (cell.DefinedColors == null) continue;
                    var seat = levelData.levelDesignData.seatModel.Request<GridObject>();
                    seat.SetColor((DefinedColors)cell.DefinedColors);
                    _gridObjectsController.AddGridObject(seat,new Vector2Int(x,y));
                }
            }

        }

        private void InitPassengerManager()
        {
            List<Passenger> passengers = new List<Passenger>();
            foreach (var passengerColor in levelData.passengers)
            {
                var passenger = levelData.levelDesignData.passengerModel.Request<Passenger>();
                passenger.SetColor(passengerColor);
                passengers.Add(passenger);
            }
            _passengerManager.Initialize(passengers,_gridSystem,levelData.levelDesignData.materialDictionary,_gridObjectsController);
        }
    }


}