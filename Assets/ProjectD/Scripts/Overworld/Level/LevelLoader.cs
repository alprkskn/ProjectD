﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiled2Unity;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class LevelLoader : MonoBehaviour
    {
        public event Action<GameObject> RemovedObject = delegate { };
        public event Action<Agent> RemovedAgent = delegate { };

        public GameObject[] LevelPrefabs;
        public RPGCharController PlayerController;
        public Player PlayerScript;
        public OverworldCameraController CameraController;


        private GameConfiguration _gameConf;

		private InteractionsManager _interactionsManager;
		public InteractionsManager InteractionsManager
		{
			get { return _interactionsManager; }
		}

		private EventManager _eventManager;
        private QuestManager _questManager;

        private Dictionary<string, GameObject> _levels;
        private List<BaseSprite> _dynamiclevelObjects;
        private List<WarpPoint> _warpPoints;
        private TiledMap _currentLevel = null;
        private string _currentLevelName = null;
		private List<Event> _currentLevelEvents = null;
        private List<Trigger> _currentLevelTriggers;
        private Transform _currentLevelObjectsLayer;
        private Transform _currentLevelAgentsLayer;

        private Pathfinder2D _pathfinder;

		public LayerMask ObstacleLayers
		{
			get
			{
				if(_pathfinder != null)
				{
					return _pathfinder.ObstaclesLayerMask;
				}
				else
				{
					return 0;
				}
			}
		}

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
			_eventManager.InitiateMinigameEvent += OnInitiateMinigameEvent;

			_gameConf = new GameConfiguration();
            _gameConf.Initialize(_eventManager);

            _interactionsManager = gameObject.AddComponent<InteractionsManager>();
            _interactionsManager.InitializeForPlayer(PlayerScript, this);


            _questManager = gameObject.AddComponent<QuestManager>();
            _questManager.Initialize(_eventManager);
            _questManager.QuestCompleted += OnQuestCompleted;

            _pathfinder = GetComponent<Pathfinder2D>();

            if (PlayerPrefs.HasKey("SaveGame"))
            {
                try
                {
                    _gameConf.SetupFromPlayerPrefs();
					_gameConf.InitializePlayerItems(PlayerScript.GetComponent<Inventory>());
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

		private void OnInitiateMinigameEvent(MinigameEnum obj)
		{
			switch (obj)
			{
				case MinigameEnum.CatPicker:
					InitiateCatPickerGame();
					break;
				default:
					Debug.LogFormat("{0} mini game is not defined.", obj.ToString());
					break;
			}
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

                RemovedObject.Invoke(objectToRemove.gameObject);
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
            _eventManager.UnregisterEvents(obj.QuestEvents);
            _gameConf.CurrentQuestId = obj.NextQuest.name;
			_eventManager.RegisterEvents(obj.NextQuest.QuestEvents, _currentLevelName);
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
				_eventManager.UnregisterEvents(_currentLevelEvents);
                Destroy(_currentLevel.gameObject);
                _dynamiclevelObjects.Clear();
            }

            yield return new WaitForEndOfFrame();

            var go = Instantiate(_levels[levelName]);
            var tm = go.GetComponent<TiledMap>();

            go.transform.position = Vector2.up * tm.MapHeightInPixels;
            _currentLevel = tm;
            TileUtils.Initialize(_currentLevel);
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
					Debug.Log(_gameConf.CurrentQuestId);
                    var quest = Resources.Load<Quest>("GameInfo/Overworld/Quests/" + _gameConf.CurrentQuestId);
                    PushQuest(quest);
                }
            }

			_currentLevelEvents = _eventManager.LoadSceneEvents(_currentLevelName);
        }

        public void SaveAndQuit()
        {
            _gameConf.LastPlayerPosition = PlayerController.Position;
            _gameConf.LastLoadedScene = _currentLevelName;
            _gameConf.SaveToPlayerPrefs(PlayerScript.GetComponent<Inventory>());

            // TODO: Actually, return to main menu.
            Application.Quit();
        }

        public void AddAgentToScene(GameObject obj)
        {
            obj.transform.SetParent(_currentLevelAgentsLayer);
            var bs = obj.GetComponent<BaseSprite>();

            if(bs != null)
            {
                bs.SetSortingLayer(_currentLevel);
                if (!bs.gameObject.isStatic)
                {
                    _dynamiclevelObjects.Add(bs);
                }
            }
        }

        public void RemoveAgentFromScene(GameObject obj)
        {
            var bs = obj.GetComponent<BaseSprite>();

            if(bs != null)
            {
                if (_dynamiclevelObjects.Contains(bs))
                {
                    _dynamiclevelObjects.Remove(bs);
                }
            }

            var agent = obj.GetComponent<Agent>();
            if(agent != null)
            {
                RemovedAgent.Invoke(agent);
            }

            Destroy(obj);
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
            if(_warpPoints == null)
            {
                _warpPoints = new List<WarpPoint>();
            }
            else
            {
                _warpPoints.Clear();
            }

            var spawnLayer = level.transform.Find("Spawn");

            foreach (var child in spawnLayer.GetImmediateChildren())
            {
                var wp = child.gameObject.AddComponent<WarpPoint>();
                wp.PlayerDetected += (x) => StartCoroutine(LoadLevel(x.name, true));
                _warpPoints.Add(wp);
                Debug.LogFormat("Warp point detected: {0}", child.name);
            }
        }

        private void SetSceneTriggers(GameObject level, string levelName)
        {
            var conf = Resources.LoadAll<TextAsset>("GameInfo/Overworld/Triggers/" + levelName);


            List<Trigger> addedTriggers = new List<Trigger>();

            if (conf == null) return;

            foreach (var trigConf in conf)
            {
                if (trigConf != null)
                {
                    var lines = trigConf.text.Split('\n').Select(x => x.Replace("\r", "")).ToArray();
                    var trigObject = Type.GetType("ProjectD.Overworld." + lines[0]).GetMethod("Create").Invoke(null, new object[] { lines }) as Trigger;
                    if (trigObject != null)
                    {
                        addedTriggers.Add(trigObject);
                    }
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

		private void InitiateCatPickerGame()
		{
			Debug.Log("Cat picker game initialized.");
			var cpgm = gameObject.AddComponent<CatPickerGameManager>();
			cpgm.Initialize(this, _currentLevel.gameObject, PlayerScript, _pathfinder);
			cpgm.QuitGameEvent += QuitCatPickerGame;
		}

		private void QuitCatPickerGame()
		{
			Destroy(GetComponent<CatPickerGameManager>());
		}

		public void ToggleWarpPoints(bool on)
        {
            if (_warpPoints == null) return;
            
            foreach(var wp in _warpPoints)
            {
                wp.Toggle(on);
            }
        }

        public void ToggleSingleWarpPoint(WarpPoint wp, bool on)
        {
            wp.Toggle(on);
        }

        GameObject go;

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

            if (go == null)
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.localScale = Vector3.one * 6f;
            }

            //var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //p.z = 0;
            //p = new Vector3(225, 415, 0);
            //p = TileUtils.SnapToGrid(p);
            //go.transform.position = p;

        }
    }
}