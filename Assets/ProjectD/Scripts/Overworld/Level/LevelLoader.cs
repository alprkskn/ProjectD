using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiled2Unity;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class LevelLoader : MonoBehaviour
    {
        public GameObject[] LevelPrefabs;
        public RPGCharController PlayerController;
        public Player PlayerScript;
        public OverworldCameraController CameraController;


        private GameConfiguration _gameConf;
        private InteractionsManager _interactionsManager;
        private EventManager _eventManager;
        private QuestManager _questManager;

        private Dictionary<string, GameObject> _levels;
        private List<BaseSprite> _dynamiclevelObjects;
        private TiledMap _currentLevel = null;
        private string _currentLevelName = null;
        private List<Trigger> _currentLevelTriggers;
        private Transform _currentLevelObjectsLayer;
        private Transform _currentLevelAgentsLayer;

        private Pathfinder2D _pathfinder;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            _levels = new Dictionary<string, GameObject>();
            foreach (var lvl in LevelPrefabs)
            {
                _levels.Add(lvl.name, lvl);
            }

            PlayerScript = PlayerController.GetComponent<Player>();

            _eventManager = gameObject.AddComponent<EventManager>();
            _eventManager.Initialize();
            _eventManager.PlaceEvent += OnPlaceEvent;
            _eventManager.RemoveEvent += OnRemoveEvent;
            _eventManager.PlayAnimEvent += OnPlayAnimEvent;

            _gameConf = gameObject.AddComponent<GameConfiguration>();
            _gameConf.Initialize(_eventManager);

            _interactionsManager = gameObject.AddComponent<InteractionsManager>();
            _interactionsManager.InitializeForPlayer(PlayerScript);


            _questManager = gameObject.AddComponent<QuestManager>();
            _questManager.Initialize(_eventManager);
            _questManager.QuestCompleted += OnQuestCompleted;

            _pathfinder = GetComponent<Pathfinder2D>();

            if (PlayerPrefs.HasKey("SaveGame"))
            {
                try
                {
                    _gameConf.SetupFromPlayerPrefs(PlayerScript.GetComponent<Inventory>());
                }
                catch (Exception e)
                {
                    Debug.Log("Save game could not be restored. " + e.Message);
                    Debug.Log("Initializing a new game. Clearing the old save game prefs.");
                    _gameConf.ResetPlayerPrefs();
                    _gameConf.SetupFromInitializationFiles();
                }
            }
            else
            {
                _gameConf.SetupFromInitializationFiles();
            }


            StartCoroutine(LoadLevel(_gameConf.LastLoadedScene));
            PlayerController.transform.MoveObjectTo2D(_gameConf.LastPlayerPosition);
            PlayerController.facing = Vector2.down;
        }

        private void OnPlayAnimEvent(string arg1, string arg2)
        {
            var target = GameObject.Find(arg1);

            // TODO: Decide how you will set and play the animations.
            throw new NotImplementedException();
        }

        private void OnRemoveEvent(string obj)
        {
            var objectToRemove = _currentLevelObjectsLayer.Find(obj);

            if (objectToRemove != null)
            {
                if (!objectToRemove.gameObject.isStatic)
                {
                    var bs = objectToRemove.GetComponent<BaseSprite>();

                    if (bs != null)
                    {
                        _dynamiclevelObjects.Remove(bs);
                    }
                }

                GameObject.Destroy(objectToRemove.gameObject);
            }
        }

        private void OnPlaceEvent(string arg1, Vector3 arg2)
        {
            var obj = Resources.Load<GameObject>(arg1);

            var go = Instantiate(obj);
            go.name = go.name.Replace("(Clone)", "");
            go.transform.position = arg2;
            go.transform.SetParent(_currentLevelObjectsLayer, true);

            var bs = go.GetComponent<BaseSprite>();
            if (bs != null)
            {
                bs.SetSortingLayer(_currentLevel);

                if (!go.isStatic)
                {
                    _dynamiclevelObjects.Add(bs);
                }
            }
        }

        private void PushQuest(Quest quest)
        {
            _questManager.SetCurrentQuest(quest);
            _eventManager.RegisterEvents(quest.QuestEvents, _currentLevelName);
        }

        private void OnQuestCompleted(Quest obj)
        {
            // TODO: Do whatever you need after a finished quest and push the new quest to the manager.
            _eventManager.UnregisterEvents(obj.QuestEvents);
            _gameConf.CurrentQuestId = obj.name;
        }

        public IEnumerator LoadLevel(string levelName, bool isTransition = false)
        {
            if (!_levels.ContainsKey(levelName))
            {
                yield break;
            }

            string prevLevel = null;

            if (_currentLevel != null)
            {
                // Destroy level
                // Move this under the map class as a function if it gets more complex.
                prevLevel = _currentLevelName;

                _eventManager.RemoveSceneTriggers(_currentLevelTriggers);
                Destroy(_currentLevel.gameObject);
                _dynamiclevelObjects.Clear();
            }

            yield return new WaitForEndOfFrame();

            var go = Instantiate(_levels[levelName]);
            var tm = go.GetComponent<TiledMap>();

            go.transform.position = Vector2.up * tm.MapHeightInPixels;
            _currentLevel = tm;
            GridUtils.SetGridTileSize(_currentLevel.TileHeight);
            _currentLevelName = levelName;

            _currentLevelObjectsLayer = go.transform.Find("Objects").transform;
            var agentsLayer = GameObject.Find("GameEntities");
            if (agentsLayer == null)
            {
                _currentLevelAgentsLayer = new GameObject("GameEntities").transform;
                _currentLevelAgentsLayer.SetParent(go.transform, true);
            }
            else
            {
                _currentLevelAgentsLayer = agentsLayer.transform;
            }

            SetWarpPoints(go);
            SetSpriteObjects(go);
            SetSceneTriggers(go, _currentLevelName);
            PopulateInventories(go);

            CameraController.SetCameraBounds(new Bounds(new Vector3(_currentLevel.MapWidthInPixels / 2, _currentLevel.MapHeightInPixels / 2, 0), new Vector3(_currentLevel.MapWidthInPixels, _currentLevel.MapHeightInPixels, 10)));
            _pathfinder.Create2DMap();
            _eventManager.AddSceneTriggers(_currentLevelTriggers);


            if (isTransition)
            {
                var entry = GetEntryPoint(prevLevel);
                PlayerController.transform.MoveObjectTo2D(GridUtils.TiledObjectMidPoint(entry));
                PlayerController.ResetTarget();
            }

            if (_questManager.CurrentQuest != null)
            {
                _eventManager.RegisterEvents(_questManager.CurrentQuest.QuestEvents, _currentLevelName);
            }
            else
            {
                if (_gameConf.CurrentQuestId != null)
                {
                    var quest = Resources.Load<Quest>("GameInfo/Overworld/Quests/" + _gameConf.CurrentQuestId);
                    PushQuest(quest);
                }
            }
        }

        public void SaveAndQuit()
        {
            _gameConf.LastPlayerPosition = PlayerController.Position;
            _gameConf.LastLoadedScene = _currentLevelName;
            _gameConf.SaveToPlayerPrefs(PlayerScript.GetComponent<Inventory>());

            // TODO: Actually, return to main menu.
            Application.Quit();
        }

        private void SetSpriteObjects(GameObject level)
        {
            _dynamiclevelObjects = new List<BaseSprite>();
            foreach (var sprite in FindObjectsOfType<BaseSprite>())
            {
                sprite.SetSortingLayer(_currentLevel);

                if (!sprite.gameObject.isStatic)
                {
                    _dynamiclevelObjects.Add(sprite);
                }
            }
        }

        private void SetWarpPoints(GameObject level)
        {
            var spawnLayer = level.transform.Find("Spawn");

            foreach (var child in spawnLayer.GetImmediateChildren())
            {
                var wp = child.gameObject.AddComponent<WarpPoint>();
                wp.PlayerDetected += (x) => StartCoroutine(LoadLevel(x.name, true));
                Debug.LogFormat("Warp point detected: {0}", child.name);
            }
        }

        private void SetSceneTriggers(GameObject level, string levelName)
        {
            // TODO: Decide how you will define triggers on level objects.
            // Create or set the triggers in this  method.
            var conf = Resources.LoadAll<TextAsset>("GameInfo/Overworld/Triggers/" + levelName);


            List<Trigger> addedTriggers = new List<Trigger>();

            if (conf == null) return;

            foreach (var trigConf in conf)
            {
                if (trigConf != null)
                {
                    var lines = trigConf.text.Split('\n').Select(x => x.Replace("\r", "")).ToArray();
                    var t = Type.GetType("ProjectD.Overworld.TileEnterTrigger");
                    var trigObject = Type.GetType("ProjectD.Overworld." + lines[0]).GetMethod("Create").Invoke(null, new object[] { lines }) as Trigger;
                    addedTriggers.Add(trigObject);
                }
            }

            _currentLevelTriggers = addedTriggers;
        }

        private void PopulateInventories(GameObject level)
        {
            var inventories = level.GetComponentsInChildren<ItemInventory>();

            foreach (var inv in inventories)
            {
                var hashString = _currentLevelName + "/Objects/" + inv.name;

                var items = _gameConf.GetInventoryState(hashString);

                if (items != null)
                {
                    inv.items = items;
                }
            }
        }

        private GameObject GetEntryPoint(string prevLevel)
        {
            var entryLayer = _currentLevel.transform.Find("Entry");

            var entryTile = entryLayer.GetImmediateChildren().Where(x => x.name == prevLevel).First();

            return entryTile.gameObject;
        }

        void Update()
        {
            if (_dynamiclevelObjects != null)
            {
                foreach (var obj in _dynamiclevelObjects)
                {
                    obj.SpriteRenderer.sortingOrder = (int)((_currentLevel.MapHeightInPixels / _currentLevel.TileHeight) - (obj.transform.position.y - obj.SpriteRenderer.bounds.extents.y) / _currentLevel.TileHeight + obj.Levitation);
                }
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveAndQuit();
            }
            else if (Input.GetKeyDown(KeyCode.F9))
            {
                _gameConf.ResetPlayerPrefs();
            }
        }
    }
}