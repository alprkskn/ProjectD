using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class EquippableItem : UsableItem
	{
		protected GameObject _equippedPlayerGO = null;

		public virtual void Equip(Player player)
		{
			player.SetHeldItem(this);
			_equippedPlayerGO = player.gameObject;
		}

		public virtual void UnEquip(Player player)
		{
			player.SetHeldItem(null);
			_equippedPlayerGO = null;
		}
	}
}