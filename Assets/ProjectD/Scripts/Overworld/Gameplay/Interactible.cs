using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractible
{
    string Tooltip();
    void Highlight(bool on);
    void Interact(GameObject player);
}
