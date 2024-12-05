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
    public class Passenger : MonoBehaviour, IMovable, IColorable
    {
        DefinedColors _definedColor;
        public State state;
        public void Move(Vector3 position)
        {
            transform.position = position;
        }

        public DefinedColors GetColor()
        {
            return _definedColor;
        }

        public void SetColor(DefinedColors color)
        {
            _definedColor=color;
        }
    }
}