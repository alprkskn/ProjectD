using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public interface IInteractive
    {
		GameObject GetGO();
        string Tooltip();
        void Highlight(bool on);
        void Interact(GameObject player);
    }
}