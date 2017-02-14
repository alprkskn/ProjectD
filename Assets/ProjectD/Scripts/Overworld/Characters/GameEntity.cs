using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class GameEntity : MonoBehaviour
    {
        [SerializeField]
        private float MaxHealth;

        private float _health;
        public float Health
        {
            get
            {
                return _health;
            }
        }

		public float Hurt(float amount)
        {
            _health -= amount;

            if(_health <= 0)
            {
                // TODO: Die event.
                _health = 0;
            }

            return _health;
        }

		// Use this for initialization
		void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}