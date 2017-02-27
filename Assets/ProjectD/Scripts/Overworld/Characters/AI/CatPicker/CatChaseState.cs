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
		private readonly CatPickerGameManager _gameManager;

        private Vector3? _chaseTarget;
        private Vector3? _subTarget;
        private bool _onActualTarget;

		public CatChaseState(CatStatePattern cat, CatPickerGameManager manager)
		{
            _ownerStatePattern = cat;
            _gameManager = manager;
		}

		public void OnTriggerEnter2D(Collider2D other)
		{
            if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // To alert state.
                _ownerStatePattern.NavigationAgent.TargetReached -= SubTargetCallback;
                _ownerStatePattern.NavigationAgent.TargetReached -= ActualTargetCallback;
                ToCatAvoidState();
            }
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
			ToCatJumpOverObstacleState();
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
            _ownerStatePattern.ChangeState(CatStatePattern.CatStates.Avoid);
		}

        public void Initialize()
        {
            _chaseTarget = null;
            _ownerStatePattern.NavigationAgent.SpeedFactor = 1f;
            Debug.Log("Into Chase State.");
        }

		public void ToCatJumpOverObstacleState()
		{
			// Cat comes along the obstacle.
			_ownerStatePattern.ChangeState(CatStatePattern.CatStates.JumpOverObstacle);
		}

		public void ToCatMoveOverObstacleState()
		{
			throw new NotImplementedException();
		}

        public void AbortState()
        {
        }
    }
}