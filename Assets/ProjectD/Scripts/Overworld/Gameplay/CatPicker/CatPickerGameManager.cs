using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatPickerGameManager : MonoBehaviour
	{
		private Player _player;
		private Pathfinder2D _pathfinder;

		private List<GameObject> _targetItems;
		private List<CatStatePattern> _cats;
		private List<Vector3> _spawnPoints;

		private GameObject[] _catPrefabs;

		// TODO: initialize players, targetItems, UI, pathfinder. Whatever is needed when
		// the game transitions into the CatPicker game.

		// This class will be responsible to assign the cats their targets. Track the player/cat interaction.
		// Also keep track of the target items.

		public void Initialize(GameObject level, Player player, Pathfinder2D pathfinder)
		{
			_player = player;
			_pathfinder = pathfinder;

			_targetItems = new List<GameObject>();
			_cats = new List<CatStatePattern>();

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