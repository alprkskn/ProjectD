﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectD.Overworld
{

    public enum TriggerTypes
    {
        TileEnter, TileStay, TileExit, PlayerBump, PlayerInteraction
    }

    public class Trigger : MonoBehaviour
    {
        public string TriggerID;
        public event Action<Trigger> FireEvent = delegate { };

        protected BoxCollider2D _collider;
        protected Rigidbody2D _rigidbody;


        protected virtual void Start()
        {

            _collider = GetComponentInChildren<BoxCollider2D>();
            _collider.isTrigger = true;

            _rigidbody = GetComponent<Rigidbody2D>();

            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody2D>();
            }

            _rigidbody.isKinematic = true;
        }

        public virtual void Fire()
        {
            FireEvent.Invoke(this);
        }

        public static Trigger Create(string[] conf)
        {
            return null;
        }
    }

    public enum EventActionType
    {
        Place, Remove, QuestMessage, PlayAnim
    }

    [Serializable]
    public class EventAction
    {
        public EventActionType ActionType;

        public string GOName;
        public Vector3 Position;
        public string Message;
        public string AnimName;
    }

    [Serializable]
    public class Event
    {
        public float Timer;
        public string EventID;
        public string TriggerID;
        public string SceneID;
        public Trigger Trigger;
        public bool Active;
        public bool OneShot;
        public List<EventAction> EventActions;
    }

    public class EventManager : MonoBehaviour
    {
        public event Action<string, Vector3> PlaceEvent = delegate { };
        public event Action<string> RemoveEvent = delegate { };
        public event Action<string> QuestMessageEvent = delegate { };
        public event Action<string, string> PlayAnimEvent = delegate { };
        public event Action<Event> EventFired = delegate { };

        private HashSet<string> _firedOneShotEvents;
        private Dictionary<string, List<Event>> _registeredEvents;
        private List<Event> _tickingEvents;

        private Dictionary<string, Trigger> _triggers;

        public void Initialize()
        {
            _firedOneShotEvents = new HashSet<string>();
            _registeredEvents = new Dictionary<string, List<Event>>();
            _tickingEvents = new List<Event>();
            _triggers = new Dictionary<string, Trigger>();

            InitializeGlobalTriggers();
        }

        private void InitializeGlobalTriggers()
        {
            // TODO: Perhaps use ScriptableObjects to add globaltriggers.
        }

        public void RemoveSceneTriggers(List<Trigger> sceneTriggers)
        {
            // TODO: Clear old scene bound triggers before unloading a scene.
			foreach(var trig in sceneTriggers)
			{
				if (_triggers.ContainsKey(trig.TriggerID))
				{
					_triggers.Remove(trig.TriggerID);
				}
			}
        }

        public void AddSceneTriggers(List<Trigger> sceneTiggers)
        {
            // TODO: Add new triggers that come with the new scene.
			foreach(var trig in sceneTiggers)
			{
				_triggers.Add(trig.TriggerID, trig);
			}

            RefreshTriggers();
        }

        void Update()
        {
            for (int i = _tickingEvents.Count - 1; i >= 0; i--)
            {
                var evnt = _tickingEvents[i];
                evnt.Timer -= Time.deltaTime;

                if (evnt.Timer <= 0)
                {
                    FireEvent(evnt);
                }

                _tickingEvents.RemoveAt(i);
            }
        }

        public void SetShotEvents(HashSet<string> shotEvents)
        {
            _firedOneShotEvents = new HashSet<string>(shotEvents);
        }

        public void RegisterEvent(Event e, string sceneID)
        {
            if (e.SceneID != sceneID)
                return;

            e.Active = true;
            if (e.TriggerID != null)
            {
                if (!_registeredEvents.ContainsKey(e.TriggerID))
                {
                    _registeredEvents.Add(e.TriggerID, new List<Event>());
                }
                _registeredEvents[e.TriggerID].Add(e);
            }
            else
            {
                _tickingEvents.Add(e);
            }
            RefreshTriggers();
        }

        public void RegisterEvents(List<Event> e, string sceneID)
        {
            foreach (var evnt in e)
            {
                if (evnt.SceneID != sceneID)
                    continue;

                evnt.Active = true;
                if (evnt.TriggerID != null)
                {
                    if (!_registeredEvents.ContainsKey(evnt.TriggerID))
                    {
                        _registeredEvents.Add(evnt.TriggerID, new List<Event>());
                    }
                    _registeredEvents[evnt.TriggerID].Add(evnt);
                }
                else
                {
                    _tickingEvents.Add(evnt);
                }
            }
            RefreshTriggers();
        }

        public void UnregisterEvents(List<Event> e)
        {
            foreach (var pair in _registeredEvents)
            {
                pair.Value.RemoveAll(x => e.Contains(x));
            }

            foreach (var evnt in e)
            {
                evnt.Active = false;
            }
        }

        private void FireEvent(Event evnt)
        {
            if(evnt.OneShot && _firedOneShotEvents.Contains(evnt.EventID))
            {
                return;
            }

            foreach (var act in evnt.EventActions)
            {
                switch (act.ActionType)
                {
                    case EventActionType.Place:
                        PlaceEvent.Invoke(act.GOName, act.Position);
                        break;
                    case EventActionType.Remove:
                        RemoveEvent.Invoke(act.GOName);
                        break;
                    case EventActionType.QuestMessage:
                        QuestMessageEvent.Invoke(act.Message);
                        break;
                    case EventActionType.PlayAnim:
                        PlayAnimEvent.Invoke(act.GOName, act.AnimName);
                        break;
                }
            }
        }

        private void RefreshTriggers()
        {
            foreach (var pair in _registeredEvents)
            {
                foreach (var e in pair.Value)
                {
                    if (_triggers.ContainsKey(pair.Key))
                    {
                        if (e.Trigger == null)
                        {
                            var trig = _triggers[pair.Key];
                            e.Trigger = trig;
                            trig.FireEvent += (t) =>
                            {
                                if (e.Active)
                                {
                                    _tickingEvents.Add(e);
                                    _registeredEvents[pair.Key].Remove(e);
                                }
                            };
                        }
                    }
                    else
                    {
                        e.Trigger = null;
                    }
                }
            }
        }
    }
}