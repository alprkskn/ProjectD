using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiled2Unity;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public GameObject[] LevelPrefabs;
    public RPGCharController Player;
    public OverworldCameraController CameraController;


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

        _pathfinder = GetComponent<Pathfinder2D>();
        LoadLevel("Scene1");
    }

    public bool LoadLevel(string levelName, bool isTransition = false)
    {
        if (!_levels.ContainsKey(levelName))
        {
            return false;
        }

        string prevLevel = null;

        if (_currentLevel != null)
        {
            // Destroy level
            // Move this under the map class as a function if it gets more complex.
            prevLevel = _currentLevelName;
            Destroy(_currentLevel.gameObject);
        }
        var go = Instantiate(_levels[levelName]);
        var tm = go.GetComponent<TiledMap>();

        go.transform.position = Vector2.up * tm.MapHeightInPixels;
        _currentLevel = tm;
        GridUtils.SetGridTileSize(_currentLevel.TileHeight);
        _currentLevelName = levelName;
        SetWarpPoints(go);
        SetSpriteObjects(go);
        CameraController.SetCameraBounds(new Bounds(new Vector3(_currentLevel.MapWidthInPixels / 2, _currentLevel.MapHeightInPixels / 2, 0), new Vector3(_currentLevel.MapWidthInPixels, _currentLevel.MapHeightInPixels, 10)));
        _pathfinder.Create2DMap();

        if (isTransition)
        {
            var entry = GetEntryPoint(prevLevel);
            Player.transform.MoveObjectTo2D(GridUtils.TiledObjectMidPoint(entry));
            Player.ResetTarget();
        }

        return true;
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
            wp.PlayerDetected += (x) => LoadLevel(x.name, true);
            Debug.LogFormat("Warp point detected: {0}", child.name);
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
