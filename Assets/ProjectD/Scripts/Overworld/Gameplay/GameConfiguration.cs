using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameConfiguration : MonoBehaviour
{
    public int CurrentQuestId;
    public List<string> InventoryNames;

    public Vector2 LastPlayerPosition;
    public string LastLoadedScene;

    private Dictionary<string, GameObject> _items;
    private Dictionary<string, List<BaseItem>> _inventoryStates;

    public void SetupFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("CurrentQuest"))
        {
            CurrentQuestId = PlayerPrefs.GetInt("CurrentQuest");
        }

        _inventoryStates = new Dictionary<string, List<BaseItem>>();

        foreach(var name in InventoryNames)
        {
            _inventoryStates.Add(name, new List<BaseItem>());
            var containedItems = PlayerPrefs.GetString("I:" + name);

            foreach (var item in containedItems.Split(';'))
            {
                _inventoryStates[name].Add(_items[item].GetComponentInChildren<BaseItem>());
            }
        }

        string[] loc = PlayerPrefs.GetString("PlayerPosition").Split();
        LastPlayerPosition = new Vector2(float.Parse(loc[0]), float.Parse(loc[1]));
        LastLoadedScene = PlayerPrefs.GetString("PlayerScene");        
    }

    public void SetupFromInitializationFiles()
    {
        CurrentQuestId = 0;
        _inventoryStates = new Dictionary<string, List<BaseItem>>();

        var initialInventoriesFile = Resources.Load<TextAsset>("GameInfo/InitialInventoryStates");

        var initialInventories = new Dictionary<string, string>();
        foreach(var s in initialInventoriesFile.text.Split('\n').Select(x => x.Replace("\r", "")))
        {
            var split = s.Split();
            initialInventories.Add(split[0], split[1]);
        }

        foreach(var name in InventoryNames)
        {
            _inventoryStates.Add(name, new List<BaseItem>());

            if (initialInventories.ContainsKey(name))
            {
                foreach (var item in initialInventories[name].Split(';'))
                {
                    if (_items.ContainsKey(item))
                    {
                        _inventoryStates[name].Add(_items[item].GetComponentInChildren<BaseItem>());
                    }
                    else
                    {
                        Debug.LogFormat("Items list does not contain {0}", item);
                    }
                }
            }
        }
    }

    public List<BaseItem> GetInventoryState(string inventoryName)
    {
        if (_inventoryStates.ContainsKey(inventoryName))
        {
            return _inventoryStates[inventoryName];
        }

        return null;
    }

    // Use this for initialization
    public void Initialize()
    {
        PopulateInventoryNames();
        PopulateItems();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PopulateInventoryNames()
    {
        var inventories = Resources.Load<TextAsset>("GameInfo/Inventories");
        InventoryNames = new List<string>();
        foreach(var i in inventories.text.Split('\n'))
        {
            InventoryNames.Add(i.Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
        }
    }

    private void PopulateItems()
    {
        _items = new Dictionary<string, GameObject>();
        foreach(var item in Resources.LoadAll<GameObject>("GameInfo/Items/"))
        {
            _items.Add(item.name, item);
        }
    }
}
