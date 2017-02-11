using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class StatePattern : MonoBehaviour
	{
		protected ICatPickerState _currentState;

		// Update is called once per frame
		protected virtual void Update()
		{
			_currentState.UpdateState();
		}
	}
}