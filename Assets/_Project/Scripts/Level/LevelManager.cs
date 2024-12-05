using System.Collections.Generic;
using SMTD.BusPassengerGame;
using SMTD.Grid;
using SMTD.Grid.Level;
using UnityEngine;

namespace SMTD.LevelSystem
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] LevelData levelData;
        [SerializeField] GridSystem gridSystem;
        [SerializeField] PassengerManager passengerManager;
        [SerializeField] List<Passenger> passengers;

        private void Awake()
        {
            gridSystem.InitGrid(levelData.GridSize,levelData.CellSize);
            InitPassengerManager();
        }

        private void InitPassengerManager()
        {
            passengerManager.Initialize(passengers,gridSystem);
        }
    }


}