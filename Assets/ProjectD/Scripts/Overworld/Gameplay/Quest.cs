using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectD.Overworld
{
    [Serializable]
    public class RequiredActionsTuple
    {
        public string ActionMessage;
        public bool IsDone;
    }

    [CreateAssetMenu(fileName = "Quest", menuName = "Quest", order = 1)]
    public class Quest : ScriptableObject
    {
        public string Brief;
        public string Description;

        public List<Event> QuestEvents;

        public List<RequiredActionsTuple> RequiredActions;
        public Quest NextQuest;

        public bool CheckFinished()
        {
            return RequiredActions.All(x => x.IsDone);
        }
    }
}