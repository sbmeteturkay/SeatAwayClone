
using System;
using SMTD.GridSystem;
using TMPro;
using UnityEngine;

namespace SMTD.BusPassengerGame{
    public class MoveCounter : MonoBehaviour
    {
        [SerializeField] TMP_Text moveCounterText;

        private void Start()
        {
            GridManager.OnDragObjectMoved+= GridManagerOnOnDragObjectMoved;
        }

        private void GridManagerOnOnDragObjectMoved(IDraggable obj)
        {
            moveCounterText.text = (int.Parse(moveCounterText.text) + 1).ToString();
        }
    }
}
