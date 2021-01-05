using UnityEngine;
using GFramework;
using System.Collections;
using XLua;

public class GameLauncher : MonoBehaviour
{
    private LuaEnv env;
    void Awake()
    {
        RedDotManager.Instance.Initilize();
    }

    void Start()
    {
        BagMgr.Instance.InitItemInfo();
        UIMgr.Instance.OpenUI("UIMain");

        //test ABMgr
        ABMgr.Instance.LoadResAsync<GameObject>("model", "cube", (obj) =>
        {
            Instantiate(obj);
        });
        
        env = new LuaEnv();
        env.DoString("require 'Lua/Main'");
    }

    IEnumerator LoadABAsync(string ABName, string assetName)
    {
        AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/" + ABName);
        yield return abcr;
        AssetBundleRequest abr = abcr.assetBundle.LoadAssetAsync<GameObject>(assetName);
        yield return abr;
        GameObject go = abr.asset as GameObject;
        Instantiate(go);
    }
}