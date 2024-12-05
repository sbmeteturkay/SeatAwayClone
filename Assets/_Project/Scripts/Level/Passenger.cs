using System.Collections;
using System.Collections.Generic;
using Pancake.Pattern;
using Sisus.Init;
using SMTD.Grid;
using UnityEngine;

namespace SMTD.BusPassengerGame
{
    public class SitPublish : Publisher<SitPublish>
    {
        public Passenger Passenger;

        public SitPublish(Passenger passenger)
        {
            Passenger = passenger;
        }

        public void CheckNotify()
        {
            Notify(this);
        }
    }

    
    public class Passenger : MonoBehaviour<Renderer>, IColorable, IObserver<GridObjectsController,GridSystem>
    {
        public Renderer Renderer { get; set; }
        public Animator animator;
        private DefinedColors _definedColor;
        private StateMachine _stateMachine;
        private bool _seated = false;
        private bool _findSeat = false;
        private bool _onQueue = false;
        public SitPublish sitPublish;
        public GridObject sitGridObject;
        public GridCell lastTargetGridCell;

        protected override void Init(Renderer argument)
        {
            Renderer=argument;
            InitPassengerStates();
            sitPublish = new SitPublish(this);
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
        public void Move(Vector3 position)
        {
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

        public void OnNotify(GridObjectsController gridObjectsController,GridSystem gridSystem)
        {
            GridCell startCell =gridSystem.GetCell(new Vector3Int(gridSystem.GridSize.x-1, gridSystem.GridSize.y - 1, 0)); // Sağ üst hücre
            foreach (var gridObject in gridObjectsController.GetGridObjectsWithColor(this))
            {
                GridCell targetCell = gridSystem.GetCellFromGridPosition(gridObject.LocatedGridCell().CellPosition + Vector3Int.up);

                Debug.Log(gridObject.LocatedGridCell().CellPosition);
                List<GridCell> path = PathFinder.FindPath(startCell,targetCell , gridSystem);
                if (path != null)
                {
                    _findSeat=true;
                    StartCoroutine(FollowPath(path,transform.gameObject));
                    break;
                }
                Debug.Log("Yol bulunamadı!");
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

                lastTargetGridCell = cell;
            }
            _seated=true;
        }
    }

    public abstract class PassengerState : State
    {
        protected Passenger passenger;
        public PassengerState(Passenger passenger)
        {
            this.passenger = passenger;
        }
    }
    public class OnQueue:PassengerState{
        public OnQueue(Passenger passenger) : base(passenger)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
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
            passenger.animator.CrossFade("Walk",0.2f);
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
            passenger.animator.CrossFade("Sit",.2f);
            passenger.sitPublish.CheckNotify();
        }
    }
}