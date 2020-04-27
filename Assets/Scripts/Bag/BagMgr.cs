using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework;

public class Item
{
    public int id;
    public int num;
}

public class BagMgr : Singleton<BagMgr>
{
    public List<Item> items = new List<Item>();

    public void InitItemInfo()
    {
        for (int i = 0; i < 100000; i++)
        {
            Item item = new Item() { id = i, num = i };
            items.Add(item);
        }
    }
}
