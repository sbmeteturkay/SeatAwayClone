using UnityEngine;

namespace SMTD.Grid.Level
{
    [CreateAssetMenu(fileName = "NewGridObject", menuName = "Grid/GridObject")]
    public class GridObjectData : ScriptableObject
    {
        public GameObject Model; // Model verisi
        public Color Color; // Seçilebilir renk
    }
}