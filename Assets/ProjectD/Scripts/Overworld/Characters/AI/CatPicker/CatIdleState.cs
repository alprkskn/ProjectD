using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatIdleState : ICatPickerState
	{
		private readonly CatStatePattern ownerStatePattern;

		public CatIdleState(CatStatePattern cat)
		{

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
	}
}