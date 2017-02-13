using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatPickerGameManager : MonoBehaviour
	{
		private Player _player;
		private Pathfinder2D _pathfinder;
        private LevelLoader _levelLoader;

		private List<GameObject> _targetItems;
		private List<CatStatePattern> _cats;
		private Dictionary<GameObject, List<CatStatePattern>> _targetCatMatch;
		private List<Vector3> _spawnPoints;

		private GameObject[] _catPrefabs;
        private Transform _agentsParent;
        private Coroutine _catSpawner;


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
			foreach(var s in spawners)
			{
				var col = s.GetComponent<BoxCollider2D>();
				_spawnPoints.Add(col.bounds.center);
			}

            // TODO: I might later centralize this gameObject into a
            // GameEntityManager or something instead of just assuming
            // that object is present.
            _agentsParent = GameObject.Find("Agents").transform;

            levelLoader.RemovedObject += OnObjectRemovedFromScene;

            PopulateTargetItems(level);

            StartCoroutine(GameStartRoutine());
		}

        private void OnObjectRemovedFromScene(GameObject obj)
        {
            if (_targetItems.Contains(obj))
            {
                _targetItems.Remove(obj);

                foreach(var cat in _targetCatMatch[obj])
                {
                    cat.SetChaseTarget(null);
                }

                _targetCatMatch.Remove(obj);
            }
        }

        private IEnumerator GameStartRoutine()
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("Starting to spawn cats.");

            _catSpawner = StartCoroutine(CatSpawnerRoutine());
        }

        private IEnumerator CatSpawnerRoutine()
        {
            while(_cats.Count < 3)
            {
                var cat = Instantiate<GameObject>(_catPrefabs[Random.Range(0, _catPrefabs.Length)]);

                _levelLoader.AddAgentToScene(cat);

                var sp = TileUtils.SnapToGrid(_spawnPoints[Random.Range(0, _spawnPoints.Count)]);
                cat.transform.position = sp;
                cat.transform.SetParent(_agentsParent);

                var csp = cat.GetComponent<CatStatePattern>();
				csp.LostTarget += OnCatLoseTarget;
                _cats.Add(csp);
                SetCatTarget(csp);

                Debug.Log("Spawned a cat!");
                yield return new WaitForSeconds(Random.Range(1, 3f));
            }
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
            Debug.Log("Set target for cat: " + t.name);
		}

        private void PopulateTargetItems(GameObject levelObject)
        {
            var objs = levelObject.transform.Find("Objects");

            foreach(var bs in objs.GetComponentsInChildren<BaseSprite>())
            {
                _targetItems.Add(bs.gameObject);
                _targetCatMatch.Add(bs.gameObject, new List<CatStatePattern>());
            }
        }

		public void QuitGame()
		{

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