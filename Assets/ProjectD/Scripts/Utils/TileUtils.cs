using System.Collections;
using System.Collections.Generic;
using Tiled2Unity;
using UnityEngine;

namespace ProjectD.Overworld
{
    public static class TileUtils
    {
        public static int TileSize { get; private set; }

        public static void Initialize(TiledMap map)
        {
            TileSize = map.TileHeight;
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
    }
}