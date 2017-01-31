using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public string Brief;
    public string Description;

    public Dictionary<string, bool> RequiredActions;
    public List<Event> QuestEvents;
}

public class QuestManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
