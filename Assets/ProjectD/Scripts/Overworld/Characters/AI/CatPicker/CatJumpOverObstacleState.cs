using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatJumpOverObstacleState : ICatPickerState
	{
		private CatPickerGameManager _gameManager;
		private CatStatePattern _ownerStatePattern;

		public CatJumpOverObstacleState(CatStatePattern cat, CatPickerGameManager manager)
		{
            _ownerStatePattern = cat;
            _gameManager = manager;
		}

		public void Initialize()
		{
		}

		public void OnTriggerEnter2D(Collider2D other)
		{
		}

		public void UpdateState()
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
			throw new NotImplementedException();
		}

	}
}