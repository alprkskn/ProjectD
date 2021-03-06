﻿using System;
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
					Debug.Log("Quest Completed! Next: " + _currentQuest.NextQuest.name);
                    QuestCompleted.Invoke(_currentQuest);
                    _currentQuest = _currentQuest.NextQuest;
                }
            }
        }

        public void SetCurrentQuest(Quest quest)
        {
			var q = Instantiate(quest);
			q.name = q.name.Replace("(Clone)", "");
            _currentQuest = q;
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