using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class PlayerInteractionTrigger : Trigger
    {
		private InteractionsManager _interactionsManager;
		private GameObject _gameObject;

        public void Initialize(InteractionsManager interactionsManager)
        {
			_interactionsManager = interactionsManager;
            _interactionsManager.PlayerInteracts += OnPlayerInteracts;
        }

        private void OnPlayerInteracts(Player player, IInteractive obj)
        {
			if (obj.GetGO() == _gameObject)
			{
				Fire();
			}
        }

        public new static PlayerInteractionTrigger Create(string[] lines)
        {
            var targetGO = GameObject.Find(lines[1]);

            if (targetGO == null)
            {
                Debug.LogErrorFormat("{0} : {1} cannot be found in scene.", lines[2], lines[1]);
                return null;
            }
            else
            {
                var trig = targetGO.AddComponent<PlayerInteractionTrigger>();
				trig._gameObject = targetGO;
                trig.TriggerID = lines[2];

                var levelLoader = GameObject.Find("LevelController").GetComponent<LevelLoader>();

                if (levelLoader == null)
                {
                    Debug.LogError("Could not find the player in the scene.");
                    return null;
                }

                trig.Initialize(levelLoader.InteractionsManager);

                return trig;
            }
        }
    }
}