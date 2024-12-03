using System;
using UnityEngine;

namespace SMTD.GridSystem
{
    public class GridManager : MonoBehaviour
    {
        //just a sprite to render tiles which is independent of unity's grid components
        [SerializeField] SpriteRenderer gridRenderer;
        [SerializeField] Grid grid;
        [SerializeField] GridInput gridInput;
        [SerializeField] private GameObject testObject;
        private bool _objectSelected;
        private void Start()
        {
            gridInput.GridInputDown += GridInputGridInputDown;
            gridInput.GridInputCancelled += GridInputOnGridInputCancelled;
        }

        private void OnDestroy()
        {
            gridInput.GridInputDown -= GridInputGridInputDown;
            gridInput.GridInputCancelled -= GridInputOnGridInputCancelled;
            
        }

        private void Update()
        {
            if (_objectSelected)
            {
                var selectedMapPosition = gridInput.GetSelectedMapPosition();
                Vector3Int cellPosition = grid.WorldToCell(selectedMapPosition);
                testObject.transform.position = new Vector3(selectedMapPosition.x,grid.GetCellCenterWorld(cellPosition).y+.5f,selectedMapPosition.z);
            }
        }

        private void GridInputGridInputDown()
        {
            //check if clicked position has object 
            if (true)
            {
                _objectSelected = true;
            }
           
        }
        private void GridInputOnGridInputCancelled()
        {
            _objectSelected = false;
            var selectedMapPosition = gridInput.GetSelectedMapPosition();
            Vector3Int cellPosition = grid.WorldToCell(selectedMapPosition);
            testObject.transform.position = grid.GetCellCenterWorld(cellPosition);
        }



        private void SetGridRendererSize(Vector2 tile)
        {
            gridRenderer.size = tile;
        }
        
    }
}
