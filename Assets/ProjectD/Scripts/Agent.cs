using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : Pathfinding2D
{
    public Transform target;
    private Collider2D col;


    // Use this for initialization
    void Start()
    {
        col = this.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Wall")) != null);
    }
}
