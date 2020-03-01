using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework;

public class RedDot_EmailUnReadSystem : RedDotBase
{
    public override bool ShowRedDot(object[] objs)
    {
        return MailManager.Instance.IsSystemRedDot();
    }
}