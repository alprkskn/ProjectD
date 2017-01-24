using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtils
{
    private static int _tileSize;

    public static void SetGridTileSize(int tileSize)
    {
        _tileSize = tileSize;
    }

    public static Vector2 TiledObjectMidPoint(GameObject tiledObject)
    {
        var p = tiledObject.transform.position;
        return new Vector2(p.x + _tileSize / 2, p.y - _tileSize / 2);
    }

}
