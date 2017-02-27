using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectD.Overworld
{
	public class CatReachTargetState : ICatPickerState
	{
		private readonly CatStatePattern _ownerStatePattern;
		private readonly CatPickerGameManager _gameManager;

		private Coroutine _pokeCoroutine;
        private bool _pokeFlag;

		public CatReachTargetState(CatStatePattern cat, CatPickerGameManager manager)
		{
            _ownerStatePattern = cat;
            _gameManager = manager;
		}

		public void OnTriggerEnter2D(Collider2D other)
		{
		}

		public void UpdateState()
		{
		}

        public void Initialize()
        {
			_pokeCoroutine = _ownerStatePattern.StartCoroutine(PokeTarget());
        }

		private IEnumerator PokeTarget()
		{
            _pokeFlag = true;
			yield return new WaitForSeconds(Random.Range(1f, 3f));
            if (_pokeFlag)
            {
                Debug.Log("Cat knocked down target.");
                _ownerStatePattern.EmitTargetKnockedDown();
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
			throw new NotImplementedException();
		}

		public void ToCatAvoidState()
		{
			throw new NotImplementedException();
		}

		public void ToCatJumpOverObstacleState()
		{
		}

		public void ToCatMoveOverObstacleState()
		{
			throw new NotImplementedException();
		}

        public void AbortState()
        {
            _ownerStatePattern.StopCoroutine(_pokeCoroutine);
        }
    }
}