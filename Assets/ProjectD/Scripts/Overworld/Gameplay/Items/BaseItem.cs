using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class BaseItem : MonoBehaviour
	{
		protected BaseSprite _itemSprite;

		[SerializeField]
		protected Sprite _uiIcon;

		public virtual void Update()
		{

		}

		public virtual void Start()
		{
			_itemSprite = GetComponent<BaseSprite>();
		}
	}
}