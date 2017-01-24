using System.Collections;
using System.Collections.Generic;
using Tiled2Unity;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public GameObject[] LevelPrefabs;

    private Dictionary<string, GameObject> _levels;
    private TiledMap _currentLevel = null;
    private Pathfinder2D _pathfinder;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        _levels = new Dictionary<string, GameObject>();
        foreach(var lvl in LevelPrefabs)
        {
            _levels.Add(lvl.name, lvl);
        }

        _pathfinder = GetComponent<Pathfinder2D>();
        LoadLevel("Scene1");
    }

    public bool LoadLevel(string levelName)
    {
        if (!_levels.ContainsKey(levelName))
        {
            return false;
        }

        if(_currentLevel != null)
        {
            // Destroy level
            // Move this under the map class as a function if it gets more complex.
            Destroy(_currentLevel.gameObject);
        }
        var go = Instantiate(_levels[levelName]);
        var tm = go.GetComponent<TiledMap>();

        go.transform.position = Vector2.up * tm.MapHeightInPixels;
        _currentLevel = tm;
        SetWarpPoints(go);
        _pathfinder.Create2DMap();
        return true;
    }

    private void SetWarpPoints(GameObject level)
    {
        var spawnLayer = level.transform.Find("Spawn");

        foreach(var child in spawnLayer.GetImmediateChildren())
        {
            var wp = child.gameObject.AddComponent<WarpPoint>();
            wp.PlayerDetected += (x) => LoadLevel(x.name);
            Debug.LogFormat("Warp point detected: {0}", child.name);
        }

    }
}
