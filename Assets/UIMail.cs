using UnityEngine;
using UnityEngine.UI;
using GFramework;

public class UIMail : UIBase
{
    public Button ButtonEmailSystem;
    public Button ButtonEmailPlayer;
    public Button ButtonClose;

    // Use this for initialization
    void Start()
    {
        // 设置按钮回调;
        ButtonEmailSystem.onClick.AddListener(OnClickEmailSystem);
        ButtonEmailPlayer.onClick.AddListener(OnClickEmailPlayer);
        ButtonClose.onClick.AddListener(Close);
    }

    /// <summary>
    /// 系统按钮的回调;
    /// </summary>
    void OnClickEmailSystem()
    {
        // 点击之后表示邮件读取过了,设置邮件为已读状态;
        // (注意这里不能够设置红点的显隐,因为是有数据控制的,所以要控制数据那边的红点逻辑);
        // RedDot.RedDotManager.Instance.SetData(RedDot.RedDotType.Email_UnRead,false);

        MailManager.Instance.SetSystemRedDot(false);
        RedDotManager.Instance.NotifyAll(RedDotType.Email_UnReadSystem);
    }

    /// <summary>
    /// 玩家按钮的回调;
    /// </summary>
    void OnClickEmailPlayer()
    {
        // 点击之后表示邮件读取过了,设置邮件为已读状态;
        // (注意这里不能够设置红点的显隐,因为是有数据控制的,所以要控制数据那边的红点逻辑);
        // RedDot.RedDotManager.Instance.SetData(RedDot.RedDotType.Email_UnRead,false);

        MailManager.Instance.SetPlayerRedDot(false);
        RedDotManager.Instance.NotifyAll(RedDotType.Email_UnReadPlayer);
    }

    void Close()
    {
        UIMgr.Instance().CloseUI("UIMail");
    }
}