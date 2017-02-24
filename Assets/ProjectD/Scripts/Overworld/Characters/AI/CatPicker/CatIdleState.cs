using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
			yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
			if (!_ownerStatePattern.OnObstacle)
				ToCatChaseState();
			else
				ToCatMoveOverObstacleState();
			_chaseInterrupt = null;
		}

		public void Initialize()
		{
			Debug.Log("Cat idle.");
		}

		public void ToCatJumpOverObstacleState()
		{
			throw new NotImplementedException();
		}

		public void ToCatMoveOverObstacleState()
		{
			_ownerStatePattern.ChangeState(CatStatePattern.CatStates.MoveOverObstacle);
		}
	}
}