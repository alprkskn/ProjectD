using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class TileEnterTrigger : Trigger
    {
        private BoxCollider2D _collider;
        private Rigidbody2D _rigidbody;
        // Use this for initialization
        void Start()
        {
            _collider = GetComponentInChildren<BoxCollider2D>();
            _collider.isTrigger = true;

            _rigidbody = gameObject.AddComponent<Rigidbody2D>();
            _rigidbody.isKinematic = true;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Player")
            {
                Debug.Log("Tile enter trigger fired.");
                Fire();
            }
        }
    }
}