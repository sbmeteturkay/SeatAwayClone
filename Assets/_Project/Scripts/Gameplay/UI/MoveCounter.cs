using SMTD.Grid;
using TMPro;
using UnityEngine;

namespace SMTD.BusPassengerGame.UI{
    public class MoveCounter : MonoBehaviour
    {
        [SerializeField] TMP_Text moveCounterText;

        #region MonoBehaviour

        private void Start()
        {
            GridObjectsController.OnDragObjectMoved+= GridManagerOnOnDragObjectMoved;
        }

        private void OnDestroy()
        {
            GridObjectsController.OnDragObjectMoved-= GridManagerOnOnDragObjectMoved;
        }

        #endregion


        private void GridManagerOnOnDragObjectMoved(IDraggable obj)
        {
            moveCounterText.text = (int.Parse(moveCounterText.text) + 1).ToString();
        }
    }
}
