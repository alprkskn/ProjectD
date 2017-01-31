using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectD.Overworld
{
    public class Inventory : MonoBehaviour
    {
        public List<BaseItem> items = new List<BaseItem>();

        public void AddItem(BaseItem item)
        {
            items.Add(item);
        }

        public void RemoveItem(BaseItem item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
            }
        }
    }
}