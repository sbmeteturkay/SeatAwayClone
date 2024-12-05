using System.Collections.Generic;
using SMTD.LevelSystem;
using TMPro;
using UnityEngine;

namespace SMTD.BusPassengerGame.UI
{
    public class PassengerGoalCounter : MonoBehaviour
    {
        [SerializeField] TMP_Text red;
        [SerializeField] TMP_Text blue;
        [SerializeField] TMP_Text orange;
        
        #region MonoBehaviour
        private void Awake()
        {
            PassengerManager.OnPassengerUpdate+= PassengerManagerOnOnPassengerUpdate; 
        }


        private void OnDestroy()
        {
            PassengerManager.OnPassengerUpdate-= PassengerManagerOnOnPassengerUpdate; 
        }
        #endregion

        private void PassengerManagerOnOnPassengerUpdate(List<Passenger> obj)
        {
            red.text = obj.FindAll(x => x.GetColor() == DefinedColors.Red&&x.sitGridObject==null).Count.ToString();
            blue.text = obj.FindAll(x => x.GetColor() == DefinedColors.Blue&&x.sitGridObject==null).Count.ToString();
            orange.text = obj.FindAll(x => x.GetColor() == DefinedColors.Orange&&x.sitGridObject==null).Count.ToString();
        }

    }
}