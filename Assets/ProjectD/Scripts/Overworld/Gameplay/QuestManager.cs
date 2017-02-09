using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectD.Overworld
{
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
			Debug.Log("Checking quest string: " + message);
            if (_currentQuest.RequiredActions.Any(x => x.ActionMessage == message))
            {
                var tuple = _currentQuest.RequiredActions.Find(x => x.ActionMessage == message);
                tuple.IsDone = true;
                if (_currentQuest.CheckFinished())
                {
                    QuestCompleted.Invoke(_currentQuest);
                    _currentQuest = _currentQuest.NextQuest;
                }
            }
        }

        public void SetCurrentQuest(Quest quest)
        {
            _currentQuest = Instantiate(quest);
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