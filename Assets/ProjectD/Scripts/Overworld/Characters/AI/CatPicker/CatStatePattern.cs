using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
	public class CatStatePattern : StatePattern
	{
		private GameObject _chaseTarget;
		private CircleCollider2D _alertCollider;

		protected override void Update()
		{
			base.Update();
		}
	}
}