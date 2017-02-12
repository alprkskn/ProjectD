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
		}

		public void ToCatReachTargetState()
		{
		}

		public void ToCatChaseState()
		{
		}

		public void ToCatIdleState()
		{
		}

		public void UpdateState()
		{
		}

		public void ToCatAvoidState()
		{
		}
	}
}