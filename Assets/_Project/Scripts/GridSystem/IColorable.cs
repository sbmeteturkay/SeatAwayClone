using System;
using SMTD.LevelSystem;

namespace SMTD.BusPassengerGame
{
    interface IColorable
    {
        DefinedColors GetColor();
        void SetColor(DefinedColors color);
    }
    [Serializable]
    public enum DefinedColors
    {
        Blue,
        Red,
        Orange,
    }
}