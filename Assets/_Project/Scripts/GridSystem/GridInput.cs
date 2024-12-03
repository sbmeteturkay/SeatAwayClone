using System;
using UnityEngine;

namespace SMTD.GridSystem
{
    public class GridInput: MonoBehaviour
    {
        [SerializeField] Camera sceneCamera;
        [SerializeField] LayerMask groundLayerMask;
        private Vector3 _lastMousePosition;
        
        public event Action GridInputDown;
        public event Action GridInputCancelled;

        public Vector3 GetSelectedMapPosition()
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = sceneCamera.nearClipPlane;
            Ray ray = sceneCamera.ScreenPointToRay(mousePosition);
            Debug.DrawRay(ray.origin,ray.direction*1000, Color.red);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayerMask))
            {
                _lastMousePosition =hit.point;
            }
            return _lastMousePosition;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GridInputDown?.Invoke();
            }
            if (Input.GetMouseButtonUp(0))
            {
                GridInputCancelled?.Invoke();
            }
        }
    }
}