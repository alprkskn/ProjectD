using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : Inventory, IInteractible
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
}
