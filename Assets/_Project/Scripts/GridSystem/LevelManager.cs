using System.Collections.Generic;
using UnityEngine;

namespace SMTD.GridSystem
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] GridManager gridManager;
        [SerializeField] List<Passenger> passengers;
        [SerializeField] Vector2 gridSize;

        private void Start()
        {
            gridManager.InitGrid(gridSize);
        }

        public void InitPassengers()
        {
            for (var index = 0; index < passengers.Count; index++)
            {
                var passenger = passengers[index];
               // passenger.Move(gridManager.GetWorldPositionFromGridPosition(new Vector3Int((gridSize.x / 2) + 1,(-gridSize / 2)+index));
            }
        }
    }
}