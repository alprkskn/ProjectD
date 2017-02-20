using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class BaseSprite : MonoBehaviour
    {

        public int Levitation;

        protected Transform _transform;
        protected SpriteRenderer _spriteRenderer;

        public Bounds Bounds { get; private set; }
        public int Width { get; private set; } // Sprite's width in Tiles.
        public int Height { get; private set; } // Sprite's height in Tiles.

        public SpriteRenderer SpriteRenderer
        {
            get
            {
                if (_spriteRenderer == null)
                {
                    SetSpriteRenderer();
                }

                return _spriteRenderer;
            }
        }


        public void SetSortingLayer(Tiled2Unity.TiledMap map)
        {
            SpriteRenderer.sortingOrder = (map.MapHeightInPixels / map.TileHeight) - (int)(_transform.position.y - SpriteRenderer.bounds.extents.y) / map.TileHeight + Levitation;
        }

        // Use this for initialization
        void Awake()
        {
            SetSpriteRenderer();
            _transform = transform;
        }

        private void SetSpriteRenderer()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            var bnds = _spriteRenderer.bounds;

            this.Bounds = bnds;
            this.Width = (int)(bnds.size.x / TileUtils.TileSize);
            this.Height = (int)(bnds.size.y / TileUtils.TileSize);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}