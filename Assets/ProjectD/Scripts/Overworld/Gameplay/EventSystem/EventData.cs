using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	[CreateAssetMenu(fileName = "EventData", menuName = "EventData", order = 2)]
	public class EventData : ScriptableObject
	{
		public float Timer;
		public string EventID;
		public string TriggerID;
		public string SceneID;
		public bool OneShot;
		public List<EventAction> EventActions;
	}
}