using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public interface ICatPickerState
	{
        void Initialize();

		void UpdateState();

		void OnTriggerEnter2D(Collider2D other);

		void ToCatIdleState();

		void ToCatReachTargetState();

		void ToCatChaseState();

		void ToCatAvoidState();

		void ToCatJumpOverObstacleState();

		void ToCatMoveOverObstacleState();
	}
}