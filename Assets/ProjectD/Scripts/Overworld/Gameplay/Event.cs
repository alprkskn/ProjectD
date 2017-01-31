using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public Trigger Trigger;

    public List<Action> EventActions;
}

public class EventManager : MonoBehaviour
{
    private List<Event> _registeredEvents;
    private List<Event> _tickingEvents;

    private Dictionary<string, Trigger> _triggers;

    public void Initialize()
    {
        _registeredEvents = new List<Event>();
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
            _registeredEvents.Remove(evnt);
        }
    }

    public void RegisterEvent(Event e)
    {
        _registeredEvents.Add(e);

        if (e.Trigger != null)
        {
            e.Trigger.FireEvent += (trig) => _tickingEvents.Add(e);
        }
        else
        {
            _tickingEvents.Add(e);
        }
    }

    public void RegisterEvents(List<Event> e)
    {
        foreach (var evnt in e)
        {
            _registeredEvents.Add(evnt);

            if (evnt.Trigger != null)
            {
                evnt.Trigger.FireEvent += (trig) => _tickingEvents.Add(evnt);
            }
            else
            {
                _tickingEvents.Add(evnt);
            }
        }
    }

    private void FireEvent(Event evnt)
    {
        foreach (var act in evnt.EventActions)
        {
            act.Invoke();
        }
    }
}
