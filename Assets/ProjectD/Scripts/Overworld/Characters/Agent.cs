using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : Pathfinding2D
{
    private Pathfinder2D _pathFinder;
    // Use this for initialization
    void Start()
    {
        _pathFinder = Pathfinder2D.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        Move(baseSpeed);

        if (Input.GetMouseButtonDown(0))
        {
            var start = _pathFinder.FindClosestNodePos(this.transform.position);
            var end = _pathFinder.FindClosestNodePos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            FindPath(start, end);
        }

    }
}
