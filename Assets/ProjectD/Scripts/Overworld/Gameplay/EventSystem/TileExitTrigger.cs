using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class TileExitTrigger : Trigger
    {
        private BoxCollider2D _collider;
        private Rigidbody2D _rigidbody;
        // Use this for initialization
        void Start()
        {
            _collider = GetComponentInChildren<BoxCollider2D>();
            _collider.isTrigger = true;

            _rigidbody = GetComponent<Rigidbody2D>();

            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody2D>();
            }

            _rigidbody.isKinematic = true;
        }

        void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag == "Player")
            {
                Debug.Log("Tile exit trigger fired.");
                Fire();
            }
        }

        public new static TileExitTrigger Create(string[] lines)
        {
            var targetGO = GameObject.Find(lines[1]);

            if(targetGO == null)
            {
                Debug.LogErrorFormat("{0} cannot be found in scene.", lines[1]);
            }

            var result = targetGO.AddComponent<TileExitTrigger>();
            result.TriggerID = lines[2];

            return result;
        }
    }
}