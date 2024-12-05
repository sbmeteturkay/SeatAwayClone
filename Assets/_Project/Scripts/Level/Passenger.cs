using Sisus.Init;
using SMTD.Grid;
using UnityEngine;

namespace SMTD.BusPassengerGame
{
    public class Passenger : MonoBehaviour<Renderer>, IMovable, IColorable
    {
        DefinedColors _definedColor;
        public State state;


        public void Move(Vector3 position)
        {
            transform.position = position;
        }

        public Renderer Renderer { get; set; }

        public DefinedColors GetColor()
        {
            return _definedColor;
        }

        public void SetColor(DefinedColors color)
        {
            _definedColor=color;
        }

        public void SetMaterial(Material material)
        {
            Renderer.material = material;
        }
        public enum State
        {
            OnLine,
            Moving,
            Seated
        }

        protected override void Init(Renderer argument)
        {
            Renderer=argument;
        }
    }
}