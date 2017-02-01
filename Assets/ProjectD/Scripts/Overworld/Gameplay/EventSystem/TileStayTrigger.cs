using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class TileStayTrigger : Trigger
    {
        private BoxCollider2D _collider;
        private Rigidbody2D _rigidbody;

        public float Timer;
        private float _activeTimer;
        private bool _playerIn;

        public void Initialize(float timer)
        {
            Timer = timer;
        }

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
            if(col.tag == "Player")
            {
                Debug.Log("player entered");
                _playerIn = true;
            }
        }

        void Update()
        {
            if (_playerIn)
            {
                _activeTimer += Time.deltaTime;
                if(_activeTimer >= Timer)
                {
                    Debug.Log("Tile stay trigger fired.");
                    Fire();
                    _activeTimer = 0;
                }
            }
        }

        void OnTriggerExit2D(Collider2D col)
        {
            if(col.tag == "Player")
            {
                Debug.Log("Player left");
                _playerIn = false;
                _activeTimer = 0f;
            }
        }
    }
}