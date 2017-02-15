using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatReachTargetState : ICatPickerState
	{
		private readonly CatStatePattern _ownerStatePattern;
		private readonly CatPickerGameManager _gameManager;

		public CatReachTargetState(CatStatePattern cat, CatPickerGameManager manager)
		{
            _ownerStatePattern = cat;
            _gameManager = manager;
		}

		public void OnTriggerEnter2D(Collider2D other)
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
			throw new NotImplementedException();
		}

		public void UpdateState()
		{
			throw new NotImplementedException();
		}

		public void ToCatAvoidState()
		{
			throw new NotImplementedException();
		}

        public void Initialize()
        {
        }
    }
}