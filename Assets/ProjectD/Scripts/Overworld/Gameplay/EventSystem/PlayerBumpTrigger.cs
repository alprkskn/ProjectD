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
    }
}