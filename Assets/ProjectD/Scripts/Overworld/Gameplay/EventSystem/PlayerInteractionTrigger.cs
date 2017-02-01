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
    }
}