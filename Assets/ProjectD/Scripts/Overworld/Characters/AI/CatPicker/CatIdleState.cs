using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatIdleState : ICatPickerState
	{
		private readonly CatStatePattern _ownerStatePattern;
		private readonly CatPickerGameManager _gameManager;

        private Coroutine _chaseInterrupt;

		public CatIdleState(CatStatePattern cat, CatPickerGameManager manager)
		{
            _ownerStatePattern = cat;
            _gameManager = manager;
		}

		public void OnTriggerEnter2D(Collider2D other)
		{
		}

		public void ToCatReachTargetState()
		{
		}

		public void ToCatChaseState()
		{
            _ownerStatePattern.ChangeState(CatStatePattern.CatStates.Chase);
		}

		public void ToCatIdleState()
		{
		}

		public void UpdateState()
		{
            if (_ownerStatePattern.ChaseTarget != null)
            {
                if (_chaseInterrupt == null)
                {
                    _chaseInterrupt = _ownerStatePattern.StartCoroutine(InterruptForChase());                    
                }
            }
		}

		public void ToCatAvoidState()
		{
		}

        IEnumerator InterruptForChase()
        {
            // TODO: Wait for the end of the current animation if exists. Then go into chase state.
            yield return new WaitForEndOfFrame();

            ToCatChaseState();
            _chaseInterrupt = null;
        }

        public void Initialize()
        {
        }
    }
}