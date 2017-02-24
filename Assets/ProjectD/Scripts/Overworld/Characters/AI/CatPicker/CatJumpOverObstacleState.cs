using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectD.Overworld
{
	public class CatJumpOverObstacleState : ICatPickerState
	{
		private CatPickerGameManager _gameManager;
		private CatStatePattern _ownerStatePattern;

		private float _delayTimer;
		private bool _jumped;
		private Vector2 _jumpPos;

		private Coroutine _jumpCoroutine;

		public CatJumpOverObstacleState(CatStatePattern cat, CatPickerGameManager manager)
		{
            _ownerStatePattern = cat;
            _gameManager = manager;
		}

		public void Initialize()
		{
			_delayTimer = Random.Range(1f, 3f);
			_jumped = false;
			var obstacle = _gameManager.GetTargetObstacle(_ownerStatePattern.TargetObject);
			_jumpPos = obstacle.GetClosestTopLayerTile(_ownerStatePattern.transform.position);

		}

		public void OnTriggerEnter2D(Collider2D other)
		{
		}

		public void UpdateState()
		{
			if (!_jumped)
			{
				if (_delayTimer > 0f)
				{
					_delayTimer -= Time.deltaTime;
				}
				else
				{
					_jumped = true;
					Debug.Log("Start jump animation.");
					_jumpCoroutine = _ownerStatePattern.StartCoroutine(ControlJumpAnimation());
				}
			}
		}

		private IEnumerator ControlJumpAnimation()
		{
			_ownerStatePattern.transform.position = _jumpPos;
			_ownerStatePattern.OnObstacle = true;
			ToCatIdleState();
			yield return null;
		}

		public void ToCatAvoidState()
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

		public void ToCatJumpOverObstacleState()
		{
			throw new NotImplementedException();
		}

		public void ToCatMoveOverObstacleState()
		{
			throw new NotImplementedException();
		}

		public void ToCatReachTargetState()
		{
			throw new NotImplementedException();
		}

	}
}