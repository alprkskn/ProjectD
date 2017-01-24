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

}