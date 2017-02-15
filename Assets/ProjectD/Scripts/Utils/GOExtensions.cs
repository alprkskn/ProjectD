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

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name" || prop.PropertyType.Equals(typeof(Material)) || prop.PropertyType.Equals(typeof(Material[]))) continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }

}