using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatPickerGameManager : MonoBehaviour
	{
		private Pathfinder2D _pathfinder;
        private LevelLoader _levelLoader;

		private List<GameObject> _targetItems;
		private List<CatStatePattern> _cats;
		private Dictionary<GameObject, List<CatStatePattern>> _targetCatMatch;
		private List<Vector3> _spawnPoints;

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
            levelLoader.RemovedAgent += OnAgentRemovedFromScene;

            PopulateTargetItems(level);

            StartCoroutine(GameStartRoutine());
		}

        private void OnAgentRemovedFromScene(Agent obj)
        {
            var cat = obj.GetComponent<CatStatePattern>();

            if(cat != null)
            {
                _cats.Remove(cat);
                foreach(var pair in _targetCatMatch)
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

                foreach(var cat in _targetCatMatch[obj])
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
            while(true)
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

            foreach(var bs in objs.GetComponentsInChildren<DroppableItem>())
            {
                _targetItems.Add(bs.gameObject);
                _targetCatMatch.Add(bs.gameObject, new List<CatStatePattern>());
            }
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