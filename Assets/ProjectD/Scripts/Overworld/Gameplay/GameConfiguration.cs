using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfiguration : MonoBehaviour
{
    public int CurrentQuestId;
    public List<string> InventoryNames;

    public Vector2 LastPlayerPosition;
    public string LastLoadedScene;

    private Dictionary<string, GameObject> _items;
    private Dictionary<string, List<BaseItem>> _inventoryStates;

    public void InitializeFromPlayerPrefs()
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

    // Use this for initialization
    void Start()
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
