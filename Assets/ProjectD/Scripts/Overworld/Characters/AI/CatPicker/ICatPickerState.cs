using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public interface ICatPickerState
	{
		void UpdateState();

		void OnTriggerEnter(Collider other);

		void ToCatIdleState();

		void ToCatReachTargetState();

		void ToCatChaseState();

		void ToCatAvoidState();
	}
}