using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class StatePattern : MonoBehaviour
	{
		[HideInInspector]
		public IState currentState;

		// Update is called once per frame
		void Update()
		{
			currentState.UpdateState();
		}
	}
}