using UnityEngine;

namespace SMTD.GridSystem
{
    interface IDraggable
    {
        void OnPick();
        void OnRelease();
        void OnDrag();
        Vector3Int CurrentGridPosition();
        void SetMovementLimitations(MovementLimitations movementLimitations);
    }

    interface IMovable
    {
        void Move(Vector3 position);
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