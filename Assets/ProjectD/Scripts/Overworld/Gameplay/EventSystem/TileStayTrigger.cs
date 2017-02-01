using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class TileStayTrigger : Trigger
    {
        private BoxCollider2D _collider;
        private Rigidbody2D _rigidbody;

        public float Timer { get; private set; }
        private float _activeTimer;

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

            }
        }

        void OnTriggerStay2D(Collider2D col)
        {
            if(col.tag == "Player")
            {
                _activeTimer += Time.deltaTime;
                if(_activeTimer >= Timer)
                {
                    Fire();
                }
            }
        }

        void OnTriggerExit2D(Collider2D col)
        {
            if(col.tag == "Player")
            {
                _activeTimer = 0f;
            }
        }
    }
}