using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectD.Overworld
{
	public class CatChaseState : ICatPickerState
	{
		private readonly CatStatePattern _ownerStatePattern;

        private Vector3? _chaseTarget;
        private Vector3? _subTarget;
        private bool _onActualTarget;

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
                SubTargetCallback(_ownerStatePattern.NavigationAgent, _ownerStatePattern.transform.position);
                //_ownerStatePattern.NavigationAgent.TargetReached +=  FindPath(_ownerStatePattern.transform.position, _chaseTarget.Value);
                Debug.LogFormat("{0} is headed to {1}.", _ownerStatePattern.name, _chaseTarget);
            }

		}

        private void SubTargetCallback(Pathfinding2D pf, Vector3 pos)
        {
            _ownerStatePattern.NavigationAgent.TargetReached -= SubTargetCallback;
            AssignSubTarget();

            if (_onActualTarget)
            {
                _ownerStatePattern.NavigationAgent.TargetReached += ActualTargetCallback;
            }
            else
            {
                _ownerStatePattern.NavigationAgent.TargetReached += SubTargetCallback;
            }

            _ownerStatePattern.NavigationAgent.FindPath(pos, _subTarget.Value);
        }

        private void ActualTargetCallback(Pathfinding2D pf, Vector3 pos)
        {
            _ownerStatePattern.NavigationAgent.TargetReached -= ActualTargetCallback;
            _ownerStatePattern.EmitTargetReached(pos);
            Debug.Log("Target reached.");
        }


        private void AssignSubTarget()
        {
            var dice = Random.value;

            if(dice > 0.3f)
            {
                var current = TileUtils.WorldPosToGrid(_ownerStatePattern.transform.position);
                var target = TileUtils.WorldPosToGrid(_chaseTarget.Value);

                var x = (int)Random.Range(Mathf.Min(target.x, current.x) - 1, Mathf.Max(target.x, current.x) + 1);
                var y = (int)Random.Range(Mathf.Min(target.y, current.y) - 1, Mathf.Max(target.y, current.y) + 1);

                _subTarget = TileUtils.GridToWorldPos(x, y);
                _onActualTarget = false;
            }
            else
            {
                _subTarget = _chaseTarget;
                _onActualTarget = true;
            }
        }

		public void ToCatAvoidState()
		{
			throw new NotImplementedException();
		}
	}
}