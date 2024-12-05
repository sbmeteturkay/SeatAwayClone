using System;
using UnityEngine;

namespace SMTD.Grid
{
    public interface IDraggable
    {
        void OnPick();
        void OnRelease();
        void OnDrag(GridSystem gridSystem);
        GridCell LocatedGridCell();
        GridCell CurrentCell();
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