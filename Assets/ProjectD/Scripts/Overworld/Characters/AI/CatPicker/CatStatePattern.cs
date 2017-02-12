using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatStatePattern : StatePattern
	{
		private Vector3 _chaseTarget;
		private CircleCollider2D _alertCollider;

		private CatAvoidState _avoidState;
		private CatChaseState _chaseState;
		private CatReachTargetState _reachTargetState;
		private CatIdleState _idleState;
		private Agent _navigationAgent;

		protected override void Awake()
		{
			base.Awake();

			_avoidState = new CatAvoidState(this);
			_chaseState = new CatChaseState(this);
			_reachTargetState = new CatReachTargetState(this);
			_idleState = new CatIdleState(this);

			_navigationAgent = GetComponent<Agent>();

            _currentState = _idleState;
		}

		protected override void Update()
		{
			base.Update();
		}
	}
}