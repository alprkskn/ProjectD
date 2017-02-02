using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class TileExitTrigger : Trigger
    {
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

            var trig = targetGO.AddComponent<TileExitTrigger>();
            trig.TriggerID = lines[2];
            trig.OneShot = bool.Parse(lines[3]);

            return trig;
        }
    }
}