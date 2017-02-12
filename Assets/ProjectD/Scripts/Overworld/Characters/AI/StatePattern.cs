using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class StatePattern : MonoBehaviour
	{
		protected ICatPickerState _currentState;

		protected virtual void Awake()
		{

		}

		protected virtual void Start()
		{

		}

		protected virtual void Update()
		{
            if (_currentState != null)
            {
                _currentState.UpdateState();
            }
		}
	}
}