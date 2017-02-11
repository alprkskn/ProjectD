using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatReachTargetState : IState
	{
		private readonly CatStatePattern ownerStatePattern;

		public CatReachTargetState(CatStatePattern cat)
		{

		}

		public void OnTriggerEnter(Collider other)
		{
			throw new NotImplementedException();
		}

		public void ToAlertState()
		{
			throw new NotImplementedException();
		}

		public void ToChaseState()
		{
			throw new NotImplementedException();
		}

		public void ToPatrolState()
		{
			throw new NotImplementedException();
		}

		public void UpdateState()
		{
			throw new NotImplementedException();
		}
	}
}