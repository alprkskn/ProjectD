using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectD.Overworld
{

    public class Quest
    {
        public string Brief;
        public string Description;

        public Dictionary<string, bool> RequiredActions;
        public List<Event> QuestEvents;

        public Quest NextQuest;

        public bool CheckFinished()
        {
            return RequiredActions.All(x => x.Value);
        }
    }

    public class QuestManager : MonoBehaviour
    {
        public event Action<Quest> QuestCompleted = delegate { };

        private Quest _currentQuest;
        private EventManager _eventManager;

        public Quest CurrentQuest
        {
            get { return _currentQuest; }
        }

        public void CheckQuestString(string message)
        {
            if (_currentQuest.RequiredActions.ContainsKey(message))
            {
                _currentQuest.RequiredActions[message] = true;
                if (_currentQuest.CheckFinished())
                {
                    QuestCompleted.Invoke(_currentQuest);
                    _currentQuest = _currentQuest.NextQuest;
                }
            }
        }

        public void SetCurrentQuest(Quest quest)
        {
            _currentQuest = quest;
        }

        public void Initialize(EventManager eventManager)
        {
            _eventManager = eventManager;
            _eventManager.QuestMessageEvent += CheckQuestString;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}