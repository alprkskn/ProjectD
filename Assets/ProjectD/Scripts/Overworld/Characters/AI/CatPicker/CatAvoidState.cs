using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectD.Overworld
{
	public class CatAvoidState : ICatPickerState
	{
		private readonly CatStatePattern _ownerStatePattern;
		private readonly CatPickerGameManager _gameManager;

        private bool _running;


		public CatAvoidState(CatStatePattern cat, CatPickerGameManager manager)
		{
            _ownerStatePattern = cat;
            _gameManager = manager;
		}

		public void OnTriggerEnter2D(Collider2D other)
		{
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
            if (!_running)
            {
                var catPos = _ownerStatePattern.transform.position;
                var delta = catPos - _gameManager.Player.transform.position;
                delta.Normalize();

                _ownerStatePattern.NavigationAgent.TargetReached += OnTargetReached;
                _ownerStatePattern.NavigationAgent.FindPath(catPos, catPos + delta * Random.Range(1, 3) * TileUtils.TileSize);

                _running = true;
            }
		}

        private void OnTargetReached(Pathfinding2D arg1, Vector3 arg2)
        {
            _ownerStatePattern.NavigationAgent.TargetReached -= OnTargetReached;
            _running = false;
            ToCatIdleState();
        }

        public void ToCatAvoidState()
		{
			throw new NotImplementedException();
		}

        public void Initialize()
        {
            _running = false;
            _ownerStatePattern.NavigationAgent.SpeedFactor = 2f;
            Debug.Log("Into avoid state.");
        }
    }
}