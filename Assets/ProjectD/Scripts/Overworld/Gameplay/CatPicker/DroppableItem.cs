using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class DroppableItem : BaseSprite
    {
        public Vector2 SpriteOffset;

        private GameObject _spriteHolder;

        void Start()
        {
            _spriteHolder = new GameObject("SpriteHolder");
            _spriteHolder.transform.SetParent(_transform);
            var temp = GOExtensions.CopyComponent<SpriteRenderer>(_spriteRenderer, _spriteHolder);
            Destroy(_spriteRenderer);
            _spriteRenderer = temp;
            _spriteHolder.transform.localPosition = SpriteOffset;
        }
    }
}