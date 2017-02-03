using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class PlayerBumpTrigger : Trigger
    {
        public void Initialize(RPGCharController player)
        {
            player.CharacterBumped += OnPlayerBumped;
        }

        private void OnPlayerBumped(Vector2 arg1, GameObject arg2)
        {
            if(arg2 == this.gameObject)
            {
                Fire();
            }
        }

        public new static PlayerBumpTrigger Create(string[] lines)
        {
            var targetGO = GameObject.Find(lines[1]);

            if(targetGO == null)
            {
                Debug.LogErrorFormat("{0} cannot be found in scene.", lines[1]);
            }

            var trig = targetGO.AddComponent<PlayerBumpTrigger>();
            trig.TriggerID = lines[2];

            var player = GameObject.Find("Player").GetComponent<RPGCharController>();

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