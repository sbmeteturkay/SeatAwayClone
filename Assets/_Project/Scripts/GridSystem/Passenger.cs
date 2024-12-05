using SMTD.Grid;
using SMTD.LevelSystem;
using UnityEngine;

namespace SMTD.BusPassengerGame
{
    public enum State
    {
        onLine,
        moving,
        seated
    }
    public class Passenger : MonoBehaviour, IMovable
    {
        DefinedColors _definedColor;
        public State state;
        public void Move(Vector3 position)
        {
            transform.position = position;
        }
    }
}