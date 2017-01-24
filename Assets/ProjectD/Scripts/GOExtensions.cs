using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GOExtensions
{
    public static List<Transform> GetImmediateChildren(this Transform parent)
    {
        return parent.GetComponentsInChildren<Transform>().Where(go => go.gameObject != parent.gameObject).ToList();
    }

    public static void MoveObjectTo2D(this Transform obj, Vector2 xy)
    {
        obj.position = new Vector3(xy.x, xy.y, obj.position.z);
    }

}