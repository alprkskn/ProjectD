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
            Avoid, Chase, ReachTarget, Idle
        }

		public event Action<CatStatePattern, GameObject> LostTarget; // Thrown when the target for the cat is lost for some reason.
        public event Action<Pathfinding2D, Vector3> TargetReached = delegate { };

		private Vector3? _chaseTarget;
        public Vector3? ChaseTarget
        {
            get
            {
                return _chaseTarget;
            }
        }


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

        protected override void Awake()
        {
            base.Awake();

            _states = new Dictionary<CatStates, ICatPickerState>()
            {
                { CatStates.Avoid, new CatAvoidState(this) },
                { CatStates.Chase, new CatChaseState(this) },
                { CatStates.ReachTarget, new CatReachTargetState(this) },
                { CatStates.Idle, new CatIdleState(this) }
            };

            _navigationAgent = GetComponent<Agent>();
            _catEntity = GetComponent<CatPickerCat>();

            _alertCollider = GetComponent<CircleCollider2D>();
            if (_alertCollider == null)
            {
                _alertCollider = gameObject.AddComponent<CircleCollider2D>();
                _alertCollider.radius = TileUtils.TileSize;
            }

            _currentState = _states[CatStates.Idle];
		}

		protected override void Update()
		{
			base.Update();
		}

		public void SetChaseTarget(Vector3? target)
		{
			_chaseTarget = target;
		}

        public void ChangeState(CatStates newState)
        {
            _currentState = _states[newState];
        }

        public void EmitTargetReached(Vector3 target)
        {
            TargetReached.Invoke(this._navigationAgent, target);
        }
	}
}