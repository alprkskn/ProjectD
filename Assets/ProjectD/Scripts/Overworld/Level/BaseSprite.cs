using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSprite : MonoBehaviour
{

    public int Levitation;
    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if(_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            return _spriteRenderer;
        }
    }

    private Transform _transform;

    public void SetSortingLayer(Tiled2Unity.TiledMap map)
    {
        SpriteRenderer.sortingOrder = (map.MapHeightInPixels / map.TileHeight) - (int)(_transform.position.y - SpriteRenderer.bounds.extents.y) / map.TileHeight + Levitation;
    }

    // Use this for initialization
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _transform = transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
