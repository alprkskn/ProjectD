using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionsManager : MonoBehaviour
{
    private Player _playerScript;
    private List<IInteractible> _reachibleObjects = new List<IInteractible>();

    public void InitializeForPlayer(Player playerScript)
    {
        _playerScript = playerScript;
        _playerScript.PlayerReachesInteractibles += OnNewReachibleInteractiblesArrive;
        _playerScript.PlayerInteracts += OnPlayerInteract;
    }

    private void OnPlayerInteract()
    {
        throw new System.NotImplementedException();
    }

    private void OnNewReachibleInteractiblesArrive(List<IInteractible> objs)
    {
        // First highlight newly arrived objects.
        // If they were present in the list. Don't do
        // anything about them.
        foreach(var interactible in objs)
        {
            if (_reachibleObjects.Contains(interactible))
            {
                // list already contains that interactible.
            }
            else
            {
                _reachibleObjects.Add(interactible);
                interactible.Highlight(true);
            }
        }

        for(int i = _reachibleObjects.Count-1; i >= 0; i--)
        {
            var interactible = _reachibleObjects[i];
            if (!objs.Contains(interactible))
            {
                interactible.Highlight(false);
                _reachibleObjects.Remove(interactible);
            }
        }
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
