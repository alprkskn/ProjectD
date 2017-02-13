using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatChaseState : ICatPickerState
	{
		private readonly CatStatePattern _ownerStatePattern;

        private Vector3? _chaseTarget;

		public CatChaseState(CatStatePattern cat)
		{
            _ownerStatePattern = cat;
		}

		public void OnTriggerEnter(Collider other)
		{
			throw new NotImplementedException();
		}

		public void ToCatReachTargetState()
		{
			throw new NotImplementedException();
		}

		public void ToCatChaseState()
		{
			throw new NotImplementedException();
		}

		public void ToCatIdleState()
		{
            _ownerStatePattern.ChangeState(CatStatePattern.CatStates.Idle);
		}

		public void UpdateState()
		{
            if(_ownerStatePattern.ChaseTarget == null)
            {
                _ownerStatePattern.NavigationAgent.Path.Clear();
                ToCatIdleState();
            }

            if(_chaseTarget == null || _ownerStatePattern.ChaseTarget != _chaseTarget)
            {
                _chaseTarget = _ownerStatePattern.ChaseTarget;
                _ownerStatePattern.NavigationAgent.FindPath(_ownerStatePattern.transform.position, _chaseTarget.Value);
                Debug.LogFormat("{0} is headed to {1}.", _ownerStatePattern.name, _chaseTarget);
            }
		}

		public void ToCatAvoidState()
		{
			throw new NotImplementedException();
		}
	}
}