using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    public event Action<GameObject> PlayerDetected = delegate { };

    private Collider2D _collider;

    // Use this for initialization
    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true;
        var rgd = this.gameObject.AddComponent<Rigidbody2D>();
        rgd.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Toggle(bool on)
    {
        _collider.enabled = on;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerDetected.Invoke(this.gameObject);
        }
    }
}
