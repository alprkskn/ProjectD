using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class ItemInventory : Inventory, IInteractive
    {
        public void Highlight(bool on)
        {
            var sr = GetComponent<SpriteRenderer>();
            sr.color = (on) ? Color.yellow : Color.white;
        }

        public string Tooltip()
        {
            throw new NotImplementedException();
        }

        public void Interact(GameObject player)
        {
            var p = player.GetComponent<Player>();
            p.EmitPlayerOpenedItemInventory(this);
        }

		public GameObject GetGO()
		{
			return this.gameObject;
		}
	}
}