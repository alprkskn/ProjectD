using System.Collections;
using System.Collections.Generic;
using Tiled2Unity;
using UnityEngine;

namespace ProjectD.Overworld
{
    public static class TileUtils
    {
        public static int TileSize { get; private set; }
		public static Vector2 GridSize { get; private set; }
		public static Vector2 GridOffset { get; private set; }

        public static void Initialize(TiledMap map)
        {
            TileSize = map.TileHeight;
			GridSize = new Vector2(Mathf.CeilToInt(map.MapWidthInPixels / (float)map.TileWidth), Mathf.CeilToInt(map.MapHeightInPixels / (float)map.TileHeight));
        }

        public static Vector3 GridToWorldPos(int x, int y)
        {
            return new Vector3(TileSize / 2 + x * TileSize, TileSize / 2 + y * TileSize, 0);
        }

        public static Vector2 WorldPosToGrid(Vector3 pos)
        {
            return new Vector2((int)(pos.x / TileSize), (int)(pos.y / TileSize));
        }

        public static Vector3 SnapToGrid(Vector3 pos)
        {
            return new Vector3(TileSize / 2 + (int)(pos.x / TileSize) * TileSize, TileSize / 2 + (int)(pos.y / TileSize) * TileSize, pos.z);
        }

		public static IEnumerator<Vector3> RadialTraversePositions(Vector3 point)
		{
			int constraint = (int)Mathf.Max(GridSize.x, GridSize.y);
			Vector2 center = new Vector2((int)(point.x / TileSize), (int)(point.y / TileSize));
			Debug.LogFormat("{0} {1} {2}", point, GridToWorldPos((int)center.x, (int)center.y), center);
			//center = new Vector2(3, 14);

			var indices = RadialTraverseIndices(center, constraint);
			while (indices.MoveNext())
			{
				var i = indices.Current;

				if(i.x >= 0 && i.x < GridSize.x && i.y >= 0 && i.y < GridSize.y)
				{
					yield return GridToWorldPos((int)i.x, (int)i.y);
				}
			}
		}
		

		public static IEnumerator<Vector2> RadialTraverseIndices(Vector2 point, int maxRadius)
		{
			var radius = 0;

			while(radius <= maxRadius)
			{
				// Start from right
				yield return new Vector2((int)point.x + radius, (int)point.y);

				// Go up
				for(int i = 1; i <= radius; i++)
				{
					yield return new Vector2((int)point.x + radius, (int)point.y + i);
				}

				// Go left
				for(int i = 1; i <= 2 * radius; i++)
				{
					yield return new Vector2((int)point.x + radius - i, (int)point.y + radius);
				}

				// Go down
				for(int i = 1; i <= 2 * radius; i++)
				{
					yield return new Vector2((int)point.x - radius, (int)point.y + radius - i);
				}

				// Go right
				for(int i = 1; i <= 2 * radius; i++)
				{
					yield return new Vector2((int)point.x - radius + i, (int)point.y - radius);
				}

				// Go up till starting position
				for(int i = 1; i <= radius - 1; i++)
				{
					yield return new Vector2((int)point.x + radius, (int)point.y - radius + i);
				}
				radius++;
			}
		}
    }
}