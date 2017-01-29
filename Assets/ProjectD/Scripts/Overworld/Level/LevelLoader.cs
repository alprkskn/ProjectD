using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiled2Unity;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public GameObject[] LevelPrefabs;
    public RPGCharController PlayerController;
    public Player PlayerScript;
    public OverworldCameraController CameraController;


    private GameConfiguration _gameConf;
    private InteractionsManager _interactionsManager;
    private Dictionary<string, GameObject> _levels;
    private List<BaseSprite> _dynamiclevelObjects;
    private TiledMap _currentLevel = null;
    private string _currentLevelName = null;
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

        _gameConf = gameObject.AddComponent<GameConfiguration>();
        _gameConf.Initialize();
        _gameConf.SetupFromInitializationFiles();

        _interactionsManager = gameObject.AddComponent<InteractionsManager>();
        _interactionsManager.InitializeForPlayer(PlayerScript);
        _pathfinder = GetComponent<Pathfinder2D>();
        StartCoroutine(LoadLevel("Scene1"));
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
        SetWarpPoints(go);
        SetSpriteObjects(go);
        PopulateInventories(go);
        CameraController.SetCameraBounds(new Bounds(new Vector3(_currentLevel.MapWidthInPixels / 2, _currentLevel.MapHeightInPixels / 2, 0), new Vector3(_currentLevel.MapWidthInPixels, _currentLevel.MapHeightInPixels, 10)));
        _pathfinder.Create2DMap();

        var entry = GetEntryPoint((isTransition) ? prevLevel : "Spawn");

        if (!isTransition) PlayerController.facing = Vector2.down;

        PlayerController.transform.MoveObjectTo2D(GridUtils.TiledObjectMidPoint(entry));
        PlayerController.ResetTarget();
    }

    private void SetSpriteObjects(GameObject level)
    {
        _dynamiclevelObjects = new List<BaseSprite>();
        foreach (var sprite in FindObjectsOfType<BaseSprite>())
        {
            var sr = sprite.SpriteRenderer;
            sr.sortingOrder = (_currentLevel.MapHeightInPixels / _currentLevel.TileHeight) - (int)(sprite.transform.position.y - sr.bounds.extents.y) / _currentLevel.TileHeight + sprite.Levitation;

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

    private void PopulateInventories(GameObject level)
    {
        var inventories = level.GetComponentsInChildren<ItemInventory>();

        foreach(var inv in inventories)
        {
            var hashString = _currentLevelName + "/Objects/" + inv.name;

            var items = _gameConf.GetInventoryState(hashString);

            if(items != null)
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
                obj.SpriteRenderer.sortingOrder = (int) ((_currentLevel.MapHeightInPixels / _currentLevel.TileHeight) - (obj.transform.position.y - obj.SpriteRenderer.bounds.extents.y) / _currentLevel.TileHeight + obj.Levitation);
            }
        }
    }
}
