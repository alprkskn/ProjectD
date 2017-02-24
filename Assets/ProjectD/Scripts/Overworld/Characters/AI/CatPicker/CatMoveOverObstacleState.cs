using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatMoveOverObstacleState : ICatPickerState
	{
		private CatPickerGameManager _gameManager;
		private CatStatePattern _ownerStatePattern;

		private Transform _catTransform;
		private Vector2 _target;
		private Vector2 _direction;

		public CatMoveOverObstacleState(CatStatePattern cat, CatPickerGameManager manager)
		{
			_ownerStatePattern = cat;
			_gameManager = manager;
			_catTransform = cat.transform;
		}

		public void Initialize()
		{
			_target = TileUtils.SnapToGrid(_ownerStatePattern.TargetObject.transform.position + Vector3.up * 2f);

			//var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			//go.transform.localScale = Vector3.one * 6f;
			//go.transform.position = _catTransform.position;

			//go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			//go.transform.localScale = Vector3.one * 6f;
			//go.transform.position = _target;

			_direction = _target - (Vector2) _catTransform.position;
			_direction.Normalize();
		}

		public void UpdateState()
		{
			if(Vector2.Distance(_catTransform.position, _target) > 0.5f)
			{
				Debug.Log("Moving: " + Vector2.Distance(_catTransform.position, _target));
				var disp = _ownerStatePattern.NavigationAgent.baseSpeed * Time.deltaTime;
				_catTransform.Translate(_direction.x * disp, _direction.y * disp, 0f);
			}
			else
			{
				Debug.Log("Cat moved over obstacle.");
				ToCatReachTargetState();
			}
		}

		public void OnTriggerEnter2D(Collider2D other)
		{
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
			throw new NotImplementedException();
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
			_ownerStatePattern.ChangeState(CatStatePattern.CatStates.ReachTarget);
		}

	}
}