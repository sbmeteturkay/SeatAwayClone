using System.Collections.Generic;
using SMTD.BusPassengerGame;
using SMTD.Grid;
using UnityEngine;

namespace SMTD.LevelSystem
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] GridSystem gridSystem;
        [SerializeField] PassengerManager passengerManager;
        [SerializeField] List<Passenger> passengers;
        [SerializeField] Vector2Int gridSize;
        [SerializeField] Vector3 cellSize;

        private void Awake()
        {
            gridSystem.InitGrid(gridSize,cellSize);
            InitPassengerManager();
        }

        private void InitPassengerManager()
        {
            passengerManager.Initialize(passengers,gridSystem);
        }
    }

    public enum DefinedColors
    {
        Blue,
        Red,
        Orange,
    }
}