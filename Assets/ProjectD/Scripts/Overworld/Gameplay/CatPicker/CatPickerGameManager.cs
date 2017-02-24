using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class Obstacle
	{
		GameObject _gameObject;
		List<Vector2> _reachingPoints;
		List<Vector2> _topLayerTiles;
		List<GameObject> _targetItems;

		public Obstacle(GameObject go)
		{
			_gameObject = go;
		}

		public void SetReachingPoints(List<Vector2> rp)
		{
			_reachingPoints = rp;
		}

		public void SetTargetItems(List<GameObject> ti)
		{
			_targetItems = ti;
		}

		public void SetTopLayerTiles(List<Vector2> tlt)
		{
			_topLayerTiles = tlt;
		}

		public Vector2 GetRandomReachingPoint()
		{
			if (_reachingPoints != null && _reachingPoints.Count > 0)
			{
				return _reachingPoints[Random.Range(0, _reachingPoints.Count)];
			}

			else throw new System.Exception(string.Format("{0} does not have any reachible points.", _gameObject.name));
		}

		public Vector2 GetClosestTopLayerTile(Vector2 pos)
		{
			float min = float.MaxValue;
			int index = 0;

			for (int i = 0; i < _topLayerTiles.Count; i++)
			{
				var deltaX = Mathf.Abs(pos.x - _topLayerTiles[i].x);
				if(deltaX < min)
				{
					min = deltaX;
					index = i;
				}
			}

			return _topLayerTiles[index];
		}
	}

	public class CatPickerGameManager : MonoBehaviour
	{


		private Pathfinder2D _pathfinder;
		private LevelLoader _levelLoader;

		private List<GameObject> _targetItems;
		private List<CatStatePattern> _cats;
		private List<Vector3> _spawnPoints;
		private List<Obstacle> _obstacles;

		private Dictionary<GameObject, List<CatStatePattern>> _targetCatMatch;
		private Dictionary<GameObject, Obstacle> _targetObstacleMatch;


		private GameObject[] _catPrefabs;
		private Transform _agentsParent;
		private Coroutine _catSpawner;

		private Player _player;
		public Player Player
		{
			get { return _player; }
		}


		// TODO: initialize players, targetItems, UI, pathfinder. Whatever is needed when
		// the game transitions into the CatPicker game.

		// This class will be responsible to assign the cats their targets. Track the player/cat interaction.
		// Also keep track of the target items.


		public Obstacle GetTargetObstacle(GameObject targetObject)
		{
			if (_targetObstacleMatch.ContainsKey(targetObject))
			{
				return _targetObstacleMatch[targetObject];
			}
			else
			{
				return null;
			}
		}

		public void Initialize(LevelLoader levelLoader, GameObject level, Player player, Pathfinder2D pathfinder)
		{
			_player = player;
			_pathfinder = pathfinder;
			_levelLoader = levelLoader;

			_targetItems = new List<GameObject>();
			_cats = new List<CatStatePattern>();
			_targetCatMatch = new Dictionary<GameObject, List<CatStatePattern>>();
			_targetObstacleMatch = new Dictionary<GameObject, Obstacle>();

			_catPrefabs = Resources.LoadAll<GameObject>("GameInfo/Overworld/CatPicker/CatPrefabs/");

			var spawnLayer = level.transform.Find("Spawn");
			var spawners = spawnLayer.GetImmediateChildren();
			_spawnPoints = new List<Vector3>();
			Debug.LogFormat("Found {0} spawn points.", spawners.Count);
			foreach (var s in spawners)
			{
				var col = s.GetComponent<BoxCollider2D>();
				_spawnPoints.Add(col.bounds.center);
			}

			// TODO: I might later centralize this gameObject into a
			// GameEntityManager or something instead of just assuming
			// that object is present.
			_agentsParent = GameObject.Find("Agents").transform;

			levelLoader.RemovedObject += OnObjectRemovedFromScene;
			levelLoader.RemovedAgent += OnAgentRemovedFromScene;

			PopulateTargetItems(level);
			PopulateObstacleItems(level);

			StartCoroutine(GameStartRoutine());
		}

		private void OnAgentRemovedFromScene(Agent obj)
		{
			var cat = obj.GetComponent<CatStatePattern>();

			if (cat != null)
			{
				_cats.Remove(cat);
				foreach (var pair in _targetCatMatch)
				{
					pair.Value.Remove(cat);
				}
			}
		}

		private void OnObjectRemovedFromScene(GameObject obj)
		{
			if (_targetItems.Contains(obj))
			{
				_targetItems.Remove(obj);

				foreach (var cat in _targetCatMatch[obj])
				{
					cat.SetChaseTarget(null, null);
				}

				_targetCatMatch.Remove(obj);
			}
		}

		private IEnumerator GameStartRoutine()
		{
			_levelLoader.ToggleWarpPoints(false);
			yield return new WaitForSeconds(2f);
			Debug.Log("Starting to spawn cats.");

			_catSpawner = StartCoroutine(CatSpawnerRoutine());
		}

		private IEnumerator CatSpawnerRoutine()
		{
			while (true)
			{
				if (_cats.Count < 3)
				{
					var cat = Instantiate<GameObject>(_catPrefabs[Random.Range(0, _catPrefabs.Length)]);

					_levelLoader.AddAgentToScene(cat);

					var sp = TileUtils.SnapToGrid(_spawnPoints[Random.Range(0, _spawnPoints.Count)]);
					cat.transform.position = sp;
					cat.transform.SetParent(_agentsParent);

					var csp = cat.GetComponent<CatStatePattern>();
					csp.LostTarget += OnCatLoseTarget;
					csp.SetManager(this);
					csp.CatEntity.PickedUpEvent += OnCatPickedUp;
					_cats.Add(csp);
					SetCatTarget(csp);

					cat.name = cat.name.Replace("(Clone)", "_" + Random.Range(0, 1000).ToString());

					yield return new WaitForSeconds(Random.Range(1, 3f));
				}
				else
				{
					yield return null;
				}
			}
		}

		public IEnumerator QuitGame()
		{
			_levelLoader.ToggleWarpPoints(true);
			yield return null;
		}

		private void OnCatLoseTarget(CatStatePattern obj, GameObject target)
		{
			_targetCatMatch[target].Remove(obj);
			throw new System.NotImplementedException();
		}

		private void SetCatTarget(CatStatePattern cat)
		{
			//TODO: Improve the pure random with some other selection technique.
			var t = _targetItems[Random.Range(0, _targetItems.Count)];
			_targetCatMatch[t].Add(cat);

			var p = _targetObstacleMatch[t].GetRandomReachingPoint();

			cat.SetChaseTarget(TileUtils.SnapToGrid(p), t);

			cat.TargetReached += OnCatReachedTarget;

		}

		private void OnCatReachedTarget(Pathfinding2D arg1, Vector3 arg2)
		{
			var go = arg1.gameObject;

			// TODO: Normally this goes into the Target reached state.
			// For now we just remove the agents from the scene.
			//_levelLoader.RemoveAgentFromScene(go);
		}

		private void OnCatPickedUp(CatPickerCat obj)
		{
			var go = obj.gameObject;
			_levelLoader.RemoveAgentFromScene(go);
		}


		private void PopulateTargetItems(GameObject levelObject)
		{
			var objs = levelObject.transform.Find("CatPicker/Droppables");

			foreach (var bs in objs.GetComponentsInChildren<DroppableItem>())
			{
				_targetItems.Add(bs.gameObject);
				_targetCatMatch.Add(bs.gameObject, new List<CatStatePattern>());
			}
		}

		private void PopulateObstacleItems(GameObject levelObject)
		{
			var objs = levelObject.transform.Find("Objects");

			_obstacles = new List<Obstacle>();

			foreach (var t in objs.GetImmediateChildren())
			{
				if ((_levelLoader.ObstacleLayers.value & (1 << t.gameObject.layer)) > 0)
				{
					var obs = new Obstacle(t.gameObject);
					var sprite = t.GetComponent<BaseSprite>();
					var center = sprite.transform.position;

					_obstacles.Add(obs);

					var col = t.GetComponent<BoxCollider2D>();
					var bounds = col.bounds;

					var results = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);

					List<GameObject> droppables = new List<GameObject>();

					for (int i = 0; i < results.Length; i++)
					{
						var droppable = results[i].GetComponent<DroppableItem>();
						if (results[i] != col && droppable != null)
						{
							Debug.LogFormat("{1} Overlaps with {0}", results[i].name, col.name);
							droppables.Add(droppable.gameObject);
							_targetObstacleMatch.Add(droppable.gameObject, obs);
						}
					}
					obs.SetTargetItems(droppables);

					var snappedBottomLeft = TileUtils.SnapToGrid(center - sprite.Bounds.extents);
					var neighbors = GetBaseNeighboringTiles(TileUtils.WorldPosToGrid(snappedBottomLeft), sprite.Width, sprite.Height);
					List<Vector2> baseTiles = new List<Vector2>();
					while (neighbors.MoveNext())
					{
						var c = neighbors.Current;
						var w = TileUtils.GridToWorldPos((int)c.x, (int)c.y);
						var overlaps = Physics2D.OverlapCircleAll(w, TileUtils.TileSize / 4f, _levelLoader.ObstacleLayers);
						if (overlaps.Length > 0)
						{
						}
						else
						{
							baseTiles.Add(w);
						}
					}
					obs.SetReachingPoints(baseTiles);

					var topLayerTiles = new List<Vector2>();
					var snappedTopLeft = TileUtils.SnapToGrid(new Vector2(center.x - sprite.Bounds.extents.x, center.y + sprite.Bounds.extents.y - 2f));
					for (int i = 0; i < sprite.Width; i++)
					{
						var p = new Vector3(snappedTopLeft.x + i * TileUtils.TileSize, snappedTopLeft.y, snappedTopLeft.z);
						topLayerTiles.Add(p);
					}
					obs.SetTopLayerTiles(topLayerTiles);
				}
			}

			// TODO: Obstacles should be aware of their surroundings. Namely the reachible tiles 
			// which can be used as jump points.
		}

		private IEnumerator<Vector2> GetNeighboringTiles(Vector2 bottomLeft, int width, int height)
		{
			for (int i = 0; i < width; i++)
			{
				yield return new Vector2(bottomLeft.x + i, bottomLeft.y - 1);
			}
			for (int i = 0; i < height; i++)
			{
				yield return new Vector2(bottomLeft.x + width, bottomLeft.y + i);
			}
			for (int i = 0; i < width; i++)
			{
				yield return new Vector2(bottomLeft.x + width - 1 - i, bottomLeft.y + height);
			}
			for (int i = 0; i < height; i++)
			{
				yield return new Vector2(bottomLeft.x - 1, bottomLeft.y + height - 1 - i);
			}
		}

		private IEnumerator<Vector2> GetBaseNeighboringTiles(Vector2 bottomLeft, int width, int height)
		{
			yield return new Vector3(bottomLeft.x - 1, bottomLeft.y);
			for (int i = 0; i < width; i++)
			{
				yield return new Vector2(bottomLeft.x + i, bottomLeft.y - 1);
			}
			yield return new Vector3(bottomLeft.x + width, bottomLeft.y);
		}


		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}