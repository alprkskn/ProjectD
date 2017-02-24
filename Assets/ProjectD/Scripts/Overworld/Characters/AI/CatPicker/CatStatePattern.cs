using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatStatePattern : StatePattern
	{

        public enum CatStates
        {
            Avoid, Chase, ReachTarget, Idle,
			JumpOverObstacle, MoveOverObstacle
        }

		public event Action<CatStatePattern, GameObject> LostTarget; // Thrown when the target for the cat is lost for some reason.
        public event Action<Pathfinding2D, Vector3> TargetReached = delegate { };
		public event Action<GameObject, CatStatePattern> TargetKnockedDown = delegate { };

		private Vector3? _chaseTarget;
        public Vector3? ChaseTarget
        {
            get
            {
                return _chaseTarget;
            }
        }

		private GameObject _targetObject;
		public GameObject TargetObject
		{
			get
			{
				return _targetObject;
			}
		}

		public bool OnObstacle = false;

		private CircleCollider2D _alertCollider;

        private Dictionary<CatStates, ICatPickerState> _states;
		private Agent _navigationAgent;
        public Agent NavigationAgent
        {
            get
            {
                return _navigationAgent;
            }
        }

        private CatPickerCat _catEntity;
        public CatPickerCat CatEntity
        {
            get
            {
                return _catEntity;
            }
        }

        protected CatPickerGameManager _gameManager;

        public void SetManager(CatPickerGameManager manager)
        {
            _gameManager = manager;
            _states = new Dictionary<CatStates, ICatPickerState>()
            {
                { CatStates.Avoid, new CatAvoidState(this, _gameManager) },
                { CatStates.Chase, new CatChaseState(this, _gameManager) },
                { CatStates.ReachTarget, new CatReachTargetState(this, _gameManager) },
                { CatStates.Idle, new CatIdleState(this, _gameManager) },
                { CatStates.JumpOverObstacle, new CatJumpOverObstacleState(this, _gameManager) },
                { CatStates.MoveOverObstacle, new CatMoveOverObstacleState(this, _gameManager) }
            };

            _currentState = _states[CatStates.Idle];
        }

        protected override void Awake()
        {
            base.Awake();


            _navigationAgent = GetComponent<Agent>();
            _catEntity = GetComponent<CatPickerCat>();

                _alertCollider = GetComponent<CircleCollider2D>();
            if (_alertCollider == null)
            {
                _alertCollider = gameObject.AddComponent<CircleCollider2D>();
                _alertCollider.radius = TileUtils.TileSize * 1.5f;
                _alertCollider.isTrigger = true;
            }

		}

        
		void OnTriggerEnter2D(Collider2D other)
        {
            _currentState.OnTriggerEnter2D(other);
        }

		protected override void Update()
		{
			base.Update();
		}

		public void SetChaseTarget(Vector3? target, GameObject go)
		{
			_chaseTarget = target;
			_targetObject = go;
		}

        public void ChangeState(CatStates newState)
        {
            _currentState = _states[newState];
            _currentState.Initialize();
        }

        public void EmitTargetReached(Vector3 target)
        {
            TargetReached.Invoke(this._navigationAgent, target);
        }

		public void EmitTargetKnockedDown()
		{
			TargetKnockedDown.Invoke(_targetObject, this);
		}
	}
}