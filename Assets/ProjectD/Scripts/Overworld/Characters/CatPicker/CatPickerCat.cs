using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class CatPickerCat : GameEntity, IInteractive
    {
        public event Action<CatPickerCat> PickedUpEvent = delegate { };

        public void Highlight(bool on)
        {
            var sr = GetComponent<SpriteRenderer>();
            sr.color = (on) ? Color.green : Color.white;
        }

        public void Interact(GameObject player)
        {
            PickedUpEvent.Invoke(this);
        }

        public string Tooltip()
        {
            throw new NotImplementedException();
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