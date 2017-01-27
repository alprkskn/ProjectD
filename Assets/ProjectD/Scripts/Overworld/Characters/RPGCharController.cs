using UnityEngine;
using System.Collections;
using System;

public class RPGCharController : MonoBehaviour
{

    public event Action<Vector2, GameObject> CharacterBumped = delegate { };

    private Vector3? _target;
    private bool _running;
    private Transform _transform;
    public int tileSize = 32;
    public float runMultiplier;

    public float baseSpeed;
    public LayerMask obstacleLayers;

    public float speed
    {
        get
        {
            return ((_running) ? runMultiplier : 1f) * baseSpeed * tileSize;
        }
    }

    public void ResetTarget()
    {
        _target = null;
    }

    // Use this for initialization
    void Start()
    {
        _transform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        _running = Input.GetKey(KeyCode.LeftShift);
        if (_target.HasValue)
        {
            var d = _target.Value - _transform.position;
            var _dx = (d.x == 0f) ? 0 : Mathf.Sign(d.x);
            var _dy = (d.y == 0f) ? 0 : Mathf.Sign(d.y);

            if (_dx != 0f && _dy != 0f)
            {
                _dx /= Mathf.Sqrt(2);
                _dy /= Mathf.Sqrt(2);
            }

            var delta = (speed * Time.deltaTime);

            _dx *= delta;
            _dy *= delta;

            if ((_dx < 0 && _dx + _transform.position.x < _target.Value.x) || (_dx > 0 && _dx + _transform.position.x > _target.Value.x))
            {
                _dx = _target.Value.x - _transform.position.x;
            }
            if ((_dy < 0 && _dy + _transform.position.y < _target.Value.y) || (_dy > 0 && _dy + _transform.position.y > _target.Value.y))
            {
                _dy = _target.Value.y - _transform.position.y;
            }

            if (Vector2.Distance(_transform.position, _target.Value) < 1f)
            {
                _transform.position = new Vector3(_target.Value.x, _target.Value.y, _transform.position.z);
                _target = null;
                CheckForNewTarget();
            }
            else
            {
                _transform.Translate(_dx, _dy, 0);
            }
        }
        else
        {
            CheckForNewTarget();
        }
    }

    private void CheckForNewTarget()
    {
        var dx = Input.GetAxisRaw("Horizontal");
        var dy = Input.GetAxisRaw("Vertical");

        if (dx != 0 || dy != 0)
        {
            _target = new Vector3(_transform.position.x + tileSize * dx, _transform.position.y + tileSize * dy);

            var col = Physics2D.OverlapCircle((Vector2)_target, 1f, obstacleLayers);
            if (col != null)
            {
                if (Input.anyKeyDown)
                {
                    CharacterBumped((Vector2)_target, col.gameObject);
                    Debug.Log("Character bumped.");
                }
                _target = null;
            }
        }

    }
}
