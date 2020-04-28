using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework;

public class BagPanel : BasePanel
{
    public RectTransform content;
    public int viewportHight;
    private Dictionary<int, GameObject> nowShowItems = new Dictionary<int, GameObject>();
    int oldMinIndex;
    int oldMaxIndex;
    void Update()
    {
        CheckShowAndHide();
    }

    void CheckShowAndHide()
    {
        if (content.anchoredPosition.y < 0)
            return;
        int minIndex = (int)(content.anchoredPosition.y) / 200 * 3;
        int maxIndex = (int)((content.anchoredPosition.y + viewportHight) / 200) * 3 + 2;
        if (maxIndex >= BagMgr.Instance.items.Count)
            maxIndex = BagMgr.Instance.items.Count - 1;
        for (int i = oldMinIndex; i < minIndex; i++)
        {
            if (nowShowItems.ContainsKey(i) && nowShowItems[i] != null)
                PoolMgr.Instance.PushObj("UI/Bag/BagItem", nowShowItems[i]);
            nowShowItems.Remove(i);
        }
        for (int i = maxIndex + 1; i <= oldMaxIndex; i++)
        {
            if (nowShowItems.ContainsKey(i) && nowShowItems[i] != null)
                PoolMgr.Instance.PushObj("UI/Bag/BagItem", nowShowItems[i]);
            nowShowItems.Remove(i);
        }

        oldMinIndex = minIndex;
        oldMaxIndex = maxIndex;

        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (nowShowItems.ContainsKey(i))
                continue;
            int index = i;
            nowShowItems.Add(index, null);
            PoolMgr.Instance.GetObj("UI/Bag/BagItem", (obj) =>
            {
                obj.transform.SetParent(content);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3(index % 3 * 200, -index / 3 * 200, 0);
                obj.GetComponent<BagItem>().InitItemInfo(BagMgr.Instance.items[index]);
                if (nowShowItems.ContainsKey(index))
                    nowShowItems[index] = obj;
                else
                    PoolMgr.Instance.PushObj("UI/Bag/BagItem", obj);
            });
        }
    }

    public override void Show()
    {
        base.Show();
        content.sizeDelta = new Vector2(0, Mathf.CeilToInt(BagMgr.Instance.items.Count / 3f) * 200);
        CheckShowAndHide();
    }
}
