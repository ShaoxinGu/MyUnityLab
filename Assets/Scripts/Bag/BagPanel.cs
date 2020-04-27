using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework;

public class BagPanel : BasePanel
{
    public RectTransform content;
    public int viewportHight;

    public void CheckShowAndHide()
    {
        int minIndex = (int)(content.anchoredPosition.y / 190 * 3);
        int maxIndex = (int)((content.anchoredPosition.y + viewportHight) / 190 * 3) + 2;

        for (int i = minIndex; i <= maxIndex; i++)
        {
            int index = i;
            PoolMgr.Instance.GetObj("UI/Bag/BagItem", (obj) =>
            {
                obj.transform.SetParent(content);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3(index % 3 * 200, -index / 3 * 200, 0);
                obj.GetComponent<BagItem>().InitItemInfo(BagMgr.Instance.items[index]);
            });
        }
    }

    public override void Show()
    {
        base.Show();
        CheckShowAndHide();
    }
}
