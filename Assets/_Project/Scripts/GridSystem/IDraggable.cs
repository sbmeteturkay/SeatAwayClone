using System;
using UnityEngine;

namespace SMTD.Grid
{
    public interface IDraggable
    {
        void OnPick();
        void OnRelease(GridCell cell);
        void OnDrag(Vector3 inputPosition,Vector3 currentCellWorldPosition);
        Vector3 WorldPosition();
        GridCell LocatedGridCell();
        void SetMovementLimitations(MovementLimitations movementLimitations);
        event Action OnGridChange;
    }

    public struct MovementLimitations
    {
        public readonly bool CanMoveLeft;
        public readonly bool CanMoveRight;
        public readonly bool CanMoveUp;
        public readonly bool CanMoveDown;

        public MovementLimitations(bool canMoveLeft, bool canMoveRight, bool canMoveUp, bool canMoveDown)
        {
            CanMoveLeft = canMoveLeft;
            CanMoveRight = canMoveRight;
            CanMoveUp = canMoveUp;
            CanMoveDown = canMoveDown;
        }
    }
}