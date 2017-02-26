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

        private bool _isHoldingCat;
        private CatPickerCat _heldCat;

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
                Interact();
            }
            //dummy.transform.position = this.transform.position + (Vector3)_charController.facing * 32;
        }

        private void Interact()
        {
            if (!_isHoldingCat)
            {
                PlayerInteracts.Invoke();
            }
            else
            {
                DropCat();
            }
        }

        public bool PickupCat(CatPickerCat cat)
        {
            cat.DisableCat();
            _isHoldingCat = true;
            _heldCat = cat;

            cat.transform.SetParent(_transform);
            cat.transform.localPosition = Vector3.zero;
            Debug.Log("Picked Up!");

            return true;
        }

        public void DropCat()
        {
            var dropPos = (Vector2) _transform.position + _charController.facing * TileUtils.TileSize;
            dropPos = TileUtils.SnapToGrid(dropPos);

            var agents = GameObject.Find("Agents");

            _heldCat.transform.SetParent(agents.transform);
            _heldCat.transform.position = dropPos;


            _isHoldingCat = false;
            _heldCat.EnableCat();
            _heldCat.EmitDroppedEvent(dropPos);
            _heldCat = null;
        }
    }
}