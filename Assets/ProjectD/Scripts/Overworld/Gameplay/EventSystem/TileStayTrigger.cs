using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class TileStayTrigger : Trigger
    {
        public float Timer;
        private float _activeTimer;
        private bool _playerIn;

        public void Initialize(float timer)
        {
            Timer = timer;
        }

        // Use this for initialization

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

        public new static TileStayTrigger Create(string[] lines)
        {
            var targetGO = GameObject.Find(lines[1]);

            if(targetGO == null)
            {
                Debug.LogErrorFormat("{0} cannot be found in scene.", lines[1]);
            }

            var trig = targetGO.AddComponent<TileStayTrigger>();
            trig.TriggerID = lines[2];

            trig.Initialize(float.Parse(lines[3]));

            return trig;
        }
    }
}