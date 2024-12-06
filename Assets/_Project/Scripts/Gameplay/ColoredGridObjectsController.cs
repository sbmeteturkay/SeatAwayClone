using System.Collections.Generic;
using Pancake.Linq;
using SMTD.BusPassengerGame;

namespace SMTD.Grid
{
    public class ColoredGridObjectsController:GridObjectsController
    {
        public DefinedColors GetGridObjectWithColor(IColorable colorable)
        {
            return ((IColorable)GridObjects.FirstOrDefault(x => ((IColorable)x).GetColor() == colorable.GetColor())).GetColor();
        }
        public List<GridObject> GetGridObjectsWithColor(IColorable colorable)
        {
            return GridObjects.FindAll(x => ((IColorable)x).GetColor() == colorable.GetColor());
        }
    }
}