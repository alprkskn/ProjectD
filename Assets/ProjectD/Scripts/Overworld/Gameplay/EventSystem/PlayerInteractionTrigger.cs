using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class PlayerInteractionTrigger : Trigger
    {
        public void Initialize(Player player)
        {
            player.PlayerInteracts += OnPlayerInteracts;
        }

        private void OnPlayerInteracts()
        {
            Fire();
        }

        public new static PlayerInteractionTrigger Create(string[] lines)
        {
            var targetGO = GameObject.Find(lines[1]);

            if(targetGO == null)
            {
                Debug.LogErrorFormat("{0} cannot be found in scene.", lines[1]);
            }

            var trig = targetGO.AddComponent<PlayerInteractionTrigger>();
            trig.TriggerID = lines[2];

            var player = GameObject.Find("Player").GetComponent<Player>();

            if(player == null)
            {
                Debug.LogError("Could not find the player in the scene.");
                return null;
            }

            trig.Initialize(player);

            return trig;
        }
    }
}