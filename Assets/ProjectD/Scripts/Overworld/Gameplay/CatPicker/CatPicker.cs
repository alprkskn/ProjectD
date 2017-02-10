using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatPicker : MonoBehaviour
	{
		private Player _player;
		private List<GameObject> _targetItems;
		private List<GameEntity> _cats;
		private Pathfinder2D _pathfinder;


		// TODO: initialize players, targetItems, UI, pathfinder. Whatever is needed when
		// the game transitions into the CatPicker game.

		// This class will be responsible to assign the cats their targets. Track the player/cat interaction.
		// Also keep track of the target items.

		public void Initialize(GameObject level, Player player, Pathfinder2D pathfinder)
		{
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