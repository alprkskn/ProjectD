using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class CatPickerCat : GameEntity, IInteractive
    {
        public event Action<CatPickerCat> PickedUpEvent = delegate { };
        public event Action<CatPickerCat, Vector2> DroppedEvent = delegate { };

        private GameObject _gameObject;
        private Transform _transform;
        private CatStatePattern _statePattern;
        private Agent _agent;
        private BoxCollider2D _collider;
        private Rigidbody2D _rigidbody;

		public GameObject GetGO()
		{
			return _gameObject;
		}

		public void Highlight(bool on)
        {
            var sr = GetComponent<SpriteRenderer>();
            sr.color = (on) ? Color.green : Color.white;
        }

        public void Interact(GameObject player)
        {
            PickedUpEvent.Invoke(this);
            var p = player.GetComponent<Player>();
            _statePattern.ChangeState(CatStatePattern.CatStates.Idle);
            p.PickupCat(this);
        }

        public string Tooltip()
        {
            throw new NotImplementedException();
        }

        // Use this for initialization
        void Start()
        {
            _gameObject = this.gameObject;
            _transform = this.transform;
            _statePattern = GetComponent<CatStatePattern>();
            _agent = GetComponent<Agent>();
            _collider = GetComponent<BoxCollider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DisableCat()
        {
            _statePattern.enabled = false;
            _agent.enabled = false;
            _collider.enabled = false;
        }

        public void EnableCat()
        {
            _statePattern.enabled = true;
            _agent.enabled = true;
            _collider.enabled = true;
        }

        public void EmitDroppedEvent(Vector2 position)
        {
            DroppedEvent.Invoke(this, position);
        }
    }
}