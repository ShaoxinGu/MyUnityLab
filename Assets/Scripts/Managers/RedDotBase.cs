using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework;


public abstract class RedDotBase
{
    /// <summary>
    /// 是否显示红点(true表示显示,false表示不显示;)
    /// </summary>
    /// <param name=objs></param>
    /// <returns></returns>
    public virtual bool ShowRedDot(object[] objs)
    {
        return false;
    }
}