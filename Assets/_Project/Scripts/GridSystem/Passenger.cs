using UnityEngine;

namespace SMTD.GridSystem
{
    public class Passenger : MonoBehaviour, IMovable
    {
        public void Move(Vector3 position)
        {
            transform.position = position;
        }
    }
}