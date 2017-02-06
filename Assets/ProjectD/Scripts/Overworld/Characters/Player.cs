using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class Player : GameEntity
    {
        public event Action PlayerInteracts = delegate { };
        public event Action<List<IInteractive>> PlayerReachesInteractives = delegate { };
        public event Action<ItemInventory> PlayerOpenedItemInventory = delegate { };
        public event Action<Inventory> PlayerOpenedInventory = delegate { };

        private RPGCharController _charController;
        private Transform _transform;
        private Inventory _inventory;

        public Inventory Inventory
        {
            get
            {
                return _inventory;
            }
        }
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
            _inventory = GetComponent<Inventory>();
            //dummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //dummy.transform.localScale = Vector3.one * 16f;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            var cols = Physics2D.OverlapCircleAll(_transform.position + (Vector3)_charController.facing * _charController.tileSize, 2f);

            var reachible = new List<IInteractive>();

            foreach (var obj in cols)
            {
                var interact = obj.GetComponent<IInteractive>();
                if (interact != null)
                {
                    reachible.Add(interact);
                }
            }

            PlayerReachesInteractives.Invoke(reachible);

            if (Input.GetKeyDown(KeyCode.X))
            {
                PlayerInteracts.Invoke();
            }
            //dummy.transform.position = this.transform.position + (Vector3)_charController.facing * 32;
        }
    }
}