using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake.Pattern;
using Sisus.Init;
using SMTD.Grid;
using UnityEngine;

namespace SMTD.BusPassengerGame
{
    
    public class Passenger : MonoBehaviour<Renderer>, IColorable, IObserver<PassengerManager>
    {
        public Renderer Renderer { get; set; }
        public Animator animator;
        private GridObject _sitGridObject;
        private DefinedColors _definedColor;
        private StateMachine _stateMachine;
        private bool _seated = false;
        private bool _findSeat = false;

        protected override void Init(Renderer argument)
        {
            Renderer=argument;
            InitPassengerStates();
        }

        #region MonoBehaviour
        private void Update()
        {
            _stateMachine.Update();
        }
        #endregion

        private void InitPassengerStates()
        {
            _stateMachine = new StateMachine();
            var onQueueState = new OnQueue(this);
            var movingTowardsState = new MovingTowardsTarget(this);
            var seatedState = new Seated(this);
            
            _stateMachine.ChangeState(onQueueState);
            _stateMachine.AddTransition(onQueueState,movingTowardsState,ConditionToSearchOnGrid);
            _stateMachine.AddTransition(movingTowardsState,seatedState,ConditionToSeat);
        }

        bool ConditionToSeat()
        {
            return _seated;
        }
        bool ConditionToSearchOnGrid()
        {
            return _findSeat;
        }
        public void Move(Vector3 position,bool smooth=false)
        {
            if (smooth)
            {
                transform.DOMove(position,0.5f);
                return;
            }
            transform.position = position;
        }
        public DefinedColors GetColor()
        {
            return _definedColor;
        }

        public void SetColor(DefinedColors color)
        {
            _definedColor=color;
        }

        public void SetMaterial(Material material)
        {
            Renderer.material = material;
        }

        public bool HasSeated()
        {
            return _sitGridObject != null;
        }

        public GridObject GetSeatedGridObject()
        {
            return _sitGridObject;
        }
        public void OnNotify(PassengerManager passengerManager)
        {
            var gridSystem = passengerManager.GetGridSystem;
            GridCell startCell =gridSystem.GetCell(new Vector3Int(gridSystem.GridSize.x-1, gridSystem.GridSize.y - 1, 0)); // Sağ üst hücre
            foreach (var seatGridObject in passengerManager.GetGridObjectsController.GetGridObjectsWithColor(this))
            {
                GridCell seatCell = seatGridObject.LocatedGridCell();
                if (passengerManager.HasPassengerOnObject(seatGridObject)) continue;
                GridCell targetCell = gridSystem.GetCellFromGridPosition(seatCell.CellPosition + Vector3Int.up);
                
                List<GridCell> path = PathFinder.FindPath(startCell,targetCell , gridSystem);
                if (path != null)
                {
                    transform.DOKill();
                    _sitGridObject = seatGridObject;
                    passengerManager.RemoveObserver(this);
                    passengerManager.QueueNextPassenger();
                    StartCoroutine(FollowPath(path,transform.gameObject));
                    _findSeat=true;
                    break;
                }
            }
        }

        private IEnumerator FollowPath(List<GridCell> path,GameObject target)
        {
            foreach (var cell in path)
            {
                Vector3 targetPosition = new Vector3(cell.WorldPosition.x, 0, cell.WorldPosition.z);
                while (Vector3.Distance(target.transform.position, targetPosition) > 0.1f)
                {
                    target.transform.position =
                        Vector3.MoveTowards(target.transform.position, targetPosition, 4f * Time.deltaTime);
                    target.transform.LookAt(targetPosition);
                    yield return null;
                }
            }
            _seated=true;
        }
    }

    public abstract class PassengerState : State
    {
        protected readonly Passenger Passenger;

        protected PassengerState(Passenger passenger)
        {
            this.Passenger = passenger;
        }
    }
    public class OnQueue:PassengerState{
        public OnQueue(Passenger passenger) : base(passenger)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Passenger.animator.CrossFade("Idle",0.2f);
        }
    }

    public class MovingTowardsTarget : PassengerState
    {
        public MovingTowardsTarget(Passenger passenger) : base(passenger)
        {
        }
        public override void OnEnter()
        {
            base.OnEnter();
            Passenger.animator.CrossFade("Walk",0.2f);
        }
    }

    public class Seated : PassengerState
    {
        public Seated(Passenger passenger) : base(passenger)
        {
        }
        public override void OnEnter()
        {
            base.OnEnter();
            Passenger.animator.CrossFade("Sit",.2f);
            Passenger.transform.SetParent(Passenger.GetSeatedGridObject().transform);
            Passenger.transform.rotation=Quaternion.identity;
            Passenger.transform.DOLocalJump(Vector3.zero, 1, 1, .5f);
        }
    }
}