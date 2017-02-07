using ProjectD.Overworld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = ProjectD.Overworld.Event;

namespace ProjectD
{

    public class GameConfiguration : MonoBehaviour
    {
        public string CurrentQuestId;
        public List<string> InventoryNames;

        public Vector2 LastPlayerPosition;
        public string LastLoadedScene;

        private Dictionary<string, GameObject> _items;
        private Dictionary<string, List<BaseItem>> _inventoryStates;
        private HashSet<string> _shotEventIds;
        private EventManager _eventManager;

        public void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteKey("SaveGame");
        }

        public void SetupFromPlayerPrefs(Inventory playerInventory)
        {
            if (PlayerPrefs.HasKey("CurrentQuest"))
            {
                CurrentQuestId = PlayerPrefs.GetString("CurrentQuest");
            }

            _inventoryStates = new Dictionary<string, List<BaseItem>>();

            foreach (var name in InventoryNames)
            {
                _inventoryStates.Add(name, new List<BaseItem>());
                var containedItems = PlayerPrefs.GetString("I:" + name).Split(';');

                foreach (var item in containedItems)
                {
                    if (item.Length > 0)
                    {
                        _inventoryStates[name].Add(_items[item].GetComponentInChildren<BaseItem>());
                    }
                }
            }

            var playerItems = PlayerPrefs.GetString("I:Player").Split(';');

            foreach (var item in playerItems)
            {
                if (item.Length > 0)
                {
                    playerInventory.AddItem(_items[item].GetComponentInChildren<BaseItem>());
                }
            }

            var shotEvents = PlayerPrefs.GetString("ShotEvents").Split(';');
            _shotEventIds = new HashSet<string>();

            foreach(var id in shotEvents)
            {
                if (id.Length > 0)
                {
                    _shotEventIds.Add(id);
                }
            }
            _eventManager.SetShotEvents(_shotEventIds);

            string[] loc = PlayerPrefs.GetString("PlayerPosition").Split();
            LastPlayerPosition = new Vector2(float.Parse(loc[0]), float.Parse(loc[1]));
            LastLoadedScene = PlayerPrefs.GetString("PlayerScene");
        }

        public void SaveToPlayerPrefs(Inventory playerInventory)
        {
            PlayerPrefs.SetInt("SaveGame", 1);
            PlayerPrefs.SetString("CurrentQuest", CurrentQuestId);

            foreach (var name in InventoryNames)
            {
                var saveString = "";
                foreach (var item in _inventoryStates[name])
                {
                    saveString += item.name + ';';
                }

                PlayerPrefs.SetString("I:" + name, saveString);
            }

            var playerItems = "";
            foreach (var item in playerInventory.items)
            {
                playerItems += item.name + ';';
            }
            PlayerPrefs.SetString("I:Player", playerItems);

            PlayerPrefs.SetString("PlayerPosition", LastPlayerPosition.x.ToString() + ' ' + LastPlayerPosition.y.ToString());
            PlayerPrefs.SetString("PlayerScene", LastLoadedScene);
            PlayerPrefs.SetString("ShotEvents", string.Join(",", _shotEventIds.ToArray()));
        }

        public void SetupFromInitializationFiles()
        {
            CurrentQuestId = "01_Quest";
            _inventoryStates = new Dictionary<string, List<BaseItem>>();
            _shotEventIds = new HashSet<string>();
            _eventManager.SetShotEvents(_shotEventIds);

            var initialInventoriesFile = Resources.Load<TextAsset>("GameInfo/InitialInventoryStates");

            var initialInventories = new Dictionary<string, string>();
            foreach (var s in initialInventoriesFile.text.Split('\n').Select(x => x.Replace("\r", "")))
            {
                var split = s.Split();
                initialInventories.Add(split[0], split[1]);
            }

            foreach (var name in InventoryNames)
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

            LastLoadedScene = "Scene1";
            LastPlayerPosition = new Vector2(112, 464);
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
        public void Initialize(EventManager eventManager)
        {
            _eventManager = eventManager;
            _eventManager.EventFired += OnEventFired;
            PopulateInventoryNames();
            PopulateItems();
        }

        private void OnEventFired(Event obj)
        {
            if (obj.OneShot)
            {
                _shotEventIds.Add(obj.EventID);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void PopulateInventoryNames()
        {
            var inventories = Resources.Load<TextAsset>("GameInfo/Inventories");
            InventoryNames = new List<string>();
            foreach (var i in inventories.text.Split('\n'))
            {
                InventoryNames.Add(i.Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
            }
        }

        private void PopulateItems()
        {
            _items = new Dictionary<string, GameObject>();
            foreach (var item in Resources.LoadAll<GameObject>("GameInfo/Items/"))
            {
                _items.Add(item.name, item);
            }
        }
    }
}