using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action PlayerInteracts = delegate { };
    public event Action<List<IInteractible>> PlayerReachesInteractibles = delegate { };
    public event Action<ItemInventory> PlayerOpenedItemInventory = delegate { };
    public event Action<Inventory> PlayerOpenedInventory = delegate { };

    private RPGCharController _charController;
    private Transform _transform;
    //private GameObject dummy;

    public void EmitPlayerOpenedItemInventory(ItemInventory inventory)
    {
        PlayerOpenedItemInventory.Invoke(inventory);
    }

    // Use this for initialization
    void Start()
    {
        _transform = GetComponent<Transform>();
        _charController = GetComponent<RPGCharController>();
        //dummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //dummy.transform.localScale = Vector3.one * 16f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var cols = Physics2D.OverlapCircleAll(_transform.position + (Vector3)_charController.facing * _charController.tileSize, 2f);

        var reachible = new List<IInteractible>();

        foreach(var obj in cols)
        {
            var interact = obj.GetComponent<IInteractible>();
            if(interact != null)
            {
                reachible.Add(interact);
            }
        }

        PlayerReachesInteractibles.Invoke(reachible);
        //dummy.transform.position = this.transform.position + (Vector3)_charController.facing * 32;
    }
}
