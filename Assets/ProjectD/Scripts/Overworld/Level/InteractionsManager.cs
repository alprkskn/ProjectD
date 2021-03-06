﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class InteractionsManager : MonoBehaviour
    {
		public event Action<Player, IInteractive> PlayerInteracts = delegate { };

        private Player _playerScript;
        private LevelLoader _levelLoader;

        private List<IInteractive> _reachibleObjects = new List<IInteractive>();

        public void InitializeForPlayer(Player playerScript, LevelLoader levelLoader)
        {
            _playerScript = playerScript;
            _playerScript.PlayerReachesInteractives += OnNewReachibleInteractivesArrive;
            _playerScript.PlayerInteracts += OnPlayerInteract;
            _playerScript.PlayerOpenedItemInventory += OnPlayerOpensItemInventory;

            _levelLoader = levelLoader;
            _levelLoader.RemovedAgent += (agent) =>
            {
                var interactive = agent.GetComponentsInChildren<IInteractive>();

                foreach (var i in interactive)
                {
                    _reachibleObjects.Remove(i);
                }
            };
            _levelLoader.RemovedObject += (obj) =>
            {
                var interactive = obj.GetComponentsInChildren<IInteractive>();

                foreach (var i in interactive)
                {
                    _reachibleObjects.Remove(i);
                }
            };

        }


        private void OnPlayerOpensItemInventory(ItemInventory obj)
        {

            // TODO: This event will not be used by this manager.
            // This is just an example usage for the inventory interaction.
            foreach (var item in obj.items)
            {
                _playerScript.Inventory.AddItem(item);
            }

            obj.items.Clear();
        }

        private void OnPlayerInteract()
        {
            if (_reachibleObjects.Count > 0)
            {
				var i = _reachibleObjects.Last();
                i.Interact(_playerScript.gameObject);
				PlayerInteracts.Invoke(_playerScript, i);
            }
        }

        private void OnNewReachibleInteractivesArrive(List<IInteractive> objs)
        {
            // First highlight newly arrived objects.
            // If they were present in the list. Don't do
            // anything about them.
            foreach (var interactive in objs)
            {
                if (_reachibleObjects.Contains(interactive))
                {
                    // list already contains that interactive.
                }
                else
                {
                    _reachibleObjects.Add(interactive);
                    interactive.Highlight(true);
                }
            }

            for (int i = _reachibleObjects.Count - 1; i >= 0; i--)
            {
                var interactive = _reachibleObjects[i];
                if (!objs.Contains(interactive))
                {
                    interactive.Highlight(false);
                    _reachibleObjects.Remove(interactive);
                }
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