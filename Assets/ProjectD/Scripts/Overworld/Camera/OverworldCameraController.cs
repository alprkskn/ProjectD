using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldCameraController : MonoBehaviour
{

    [SerializeField]
    private Transform _target;

    private Bounds _cameraBounds;
    private Transform _transform;
    private Camera _camera;

    public void SetCameraBounds(Bounds bounds)
    {
        _cameraBounds = bounds;
    }


    // Use this for initialization
    void Start()
    {
        _transform = this.transform;
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var newPos = new Vector3(_target.position.x, _target.position.y, _transform.position.z);

        var camExtentY = _camera.orthographicSize;
        var camExtentX = _camera.orthographicSize * _camera.aspect;

        if(_cameraBounds.extents.x < camExtentX)
        {
            newPos.x = _cameraBounds.center.x;
        }
        else if(_target.position.x + camExtentX > _cameraBounds.max.x)
        {
            newPos.x = _cameraBounds.max.x - camExtentX;
        }
        else if(_target.position.x - camExtentX < _cameraBounds.min.x)
        {
            newPos.x = _cameraBounds.min.x + camExtentX;
        }

        if(_cameraBounds.extents.y < camExtentY)
        {
            newPos.y = _cameraBounds.center.y;
        }
        else if(_target.position.y + camExtentY > _cameraBounds.max.y)
        {
            newPos.y = _cameraBounds.max.y - camExtentY;
        }
        else if(_target.position.y - camExtentY < _cameraBounds.min.y)
        {
            newPos.y = _cameraBounds.min.y + camExtentY;
        }

        _transform.position = newPos;
    }
}
