using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectD.Overworld
{

    public class Trigger : MonoBehaviour
    {
        public event Action<Trigger> FireEvent = delegate { };
        private List<Func<bool>> _conditions;
        public bool OneShot;

        private bool _shotFlag;

        public void SetConditions(List<Func<bool>> conds)
        {
            _conditions = conds;
        }

        public virtual void Fire()
        {
            FireEvent.Invoke(this);
        }

        protected virtual void Update()
        {
            if (CheckConditions())
            {
                if (OneShot && !_shotFlag || !OneShot)
                {
                    Fire();
                    _shotFlag = true;
                }
            }
        }

        protected virtual bool CheckConditions()
        {
            return _conditions.All(x => x());
        }
    }

    public class Event
    {
        private float _initialTimer;

        public float Timer;
        public string TriggerID;
        public Trigger Trigger;
        public bool Active;

        public List<Action> EventActions;
    }

    public class EventManager : MonoBehaviour
    {
        private Dictionary<string, List<Event>> _registeredEvents;
        private List<Event> _tickingEvents;

        private Dictionary<string, Trigger> _triggers;

        public void Initialize()
        {
            _registeredEvents = new Dictionary<string, List<Event>>();
            _tickingEvents = new List<Event>();
            _triggers = new Dictionary<string, Trigger>();

            InitializeGlobalTriggers();
        }

        private void InitializeGlobalTriggers()
        {
            // TODO: Perhaps use ScriptableObjects to add globaltriggers.
        }

        public void RemoveSceneTriggers(GameObject scene)
        {
            // TODO: Clear old scene bound triggers before unloading a scene.
        }

        public void AddSceneTriggers(GameObject scene)
        {
            // TODO: Add new triggers that come with the new scene.
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

        public void RegisterEvent(Event e)
        {
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

        public void RegisterEvents(List<Event> e)
        {
            foreach (var evnt in e)
            {
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
            foreach (var act in evnt.EventActions)
            {
                act.Invoke();
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