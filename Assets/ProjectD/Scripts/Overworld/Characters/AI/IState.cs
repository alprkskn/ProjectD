using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public interface IState
	{
		void UpdateState();

		void OnTriggerEnter(Collider other);

		void ToPatrolState();

		void ToAlertState();

		void ToChaseState();
	}
}