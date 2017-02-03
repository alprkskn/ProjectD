using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class TileEnterTrigger : Trigger
    {
        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Player")
            {
                Debug.Log("Tile enter trigger fired.");
                Fire();
            }
        }

        public new static TileEnterTrigger Create(string[] lines)
        {
            var targetGO = GameObject.Find(lines[1]);

            if(targetGO == null)
            {
                Debug.LogErrorFormat("{0} cannot be found in scene.", lines[1]);
            }

            var trig = targetGO.AddComponent<TileEnterTrigger>();
            trig.TriggerID = lines[2];

            return trig;
        }
    }
}