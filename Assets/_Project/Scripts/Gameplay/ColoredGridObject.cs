using SMTD.BusPassengerGame;
using UnityEngine;

namespace SMTD.Grid
{
    public class ColoredGridObject:GridObject, IColorable
    {

        #region IColorable implementattion
        private DefinedColors _definedColor;
        [SerializeField] private Renderer _renderer;
        public Renderer Renderer
        {
            get=>_renderer;
            set => _renderer=value;
        }

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

        #endregion

    }
}