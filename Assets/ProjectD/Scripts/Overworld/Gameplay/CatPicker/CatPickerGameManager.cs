using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatPickerGameManager : MonoBehaviour
	{

		private class Obstacle
		{
			GameObject _gameObject;
			List<Vector2[]> _reachingPoints;
			List<GameObject> _targetItems;

			public Obstacle(GameObject go)
			{
				_gameObject = go;
			}

			public void SetReachingPoints(List<Vector2[]> rp)
			{
				_reachingPoints = rp;
			}

			public void SetTargetItems(List<GameObject> ti)
			{
				_targetItems = ti;
			}
		}

		private Pathfinder2D _pathfinder;
		private LevelLoader _levelLoader;

		private List<GameObject> _targetItems;
		private List<CatStatePattern> _cats;
		private List<Vector3> _spawnPoints;
		private List<Obstacle> _obstacles;

		private Dictionary<GameObject, List<CatStatePattern>> _targetCatMatch;


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

		public void Initialize(LevelLoader levelLoader, GameObject level, Player player, Pathfinder2D pathfinder)
		{
			_player = player;
			_pathfinder = pathfinder;
			_levelLoader = levelLoader;

			_targetItems = new List<GameObject>();
			_cats = new List<CatStatePattern>();
			_targetCatMatch = new Dictionary<GameObject, List<CatStatePattern>>();

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
					cat.SetChaseTarget(null);
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
			cat.SetChaseTarget(TileUtils.SnapToGrid(t.transform.position));

			cat.TargetReached += OnCatReachedTarget;

		}

		private void OnCatReachedTarget(Pathfinding2D arg1, Vector3 arg2)
		{
			var go = arg1.gameObject;

			// TODO: Normally this goes into the Target reached state.
			// For now we just remove the agents from the scene.
			_levelLoader.RemoveAgentFromScene(go);
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

					_obstacles.Add(obs);

					var col = t.GetComponent<BoxCollider2D>();
					var bounds = col.bounds;

					//RaycastHit2D[] results = new RaycastHit2D[10];

					var results = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);

					for (int i = 0; i < results.Length; i++)
					{
						var droppable = results[i].GetComponent<DroppableItem>();
						if(results[i] != col && droppable != null)
							Debug.LogFormat("{1} Overlaps with {0}", results[i].name, col.name);
					}
				}
			}

			// TODO: Obstacles should be aware of their surroundings. Namely the reachible tiles 
			// which can be used as jump points.
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