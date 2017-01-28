using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractive
{
    string Tooltip();
    void Highlight(bool on);
    void Interact(GameObject player);
}
