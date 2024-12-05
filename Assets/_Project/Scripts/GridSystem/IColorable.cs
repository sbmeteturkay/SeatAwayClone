using System;
using SMTD.LevelSystem;
using UnityEngine;

namespace SMTD.BusPassengerGame
{
    interface IColorable
    {
        Renderer Renderer { get; set; }
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