using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SMTD.BusPassengerGame;
using UnityEngine;

namespace SMTD.Grid.Level
{
    [Serializable]
    [CreateAssetMenu(fileName = "NewLevelDesignData", menuName = "Level/LevelDesignData")]
    public class LevelDesignData: SerializedScriptableObject{
        public Dictionary<DefinedColors, Material> materialDictionary = new ();
        public GameObject passengerModel;
        public GameObject seatModel;
    }
}