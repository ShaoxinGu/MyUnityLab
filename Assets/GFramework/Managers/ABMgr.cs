using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework;
using UnityEngine.Events;

public class ABMgr : Singleton<ABMgr>
{
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    private string MainABName
    {
        get
        {
#if UNITY_IOS
            return "iOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "StandaloneWindows";
#endif
        }
    }

    //主包
    private AssetBundle mainAB = null;
    //依赖包获取用的配置文件
    private AssetBundleManifest manifest = null;
    //AB包字典
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    //加载AB包
    public void LoadAB(string abName)
    {
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        AssetBundle ab;
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i], ab);
            }
        }
        if (!abDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            abDic.Add(abName, ab);
        }
    }

    //同步加载AB包资源 普通模式
    public Object LoadRes(string abName, string resName)
    {
        LoadAB(abName);
        Object obj = abDic[abName].LoadAsset(resName);
        return obj;
    }

    //同步加载AB包资源 指定类型模式
    public Object LoadRes(string abName, string resName, System.Type type)
    {
        LoadAB(abName);
        Object obj = abDic[abName].LoadAsset(resName, type);
        return obj;
    }

    //同步加载AB包资源 泛型模式
    public T LoadRes<T>(string abName, string resName, System.Type type) where T : Object
    {
        LoadAB(abName);
        T obj = abDic[abName].LoadAsset<T>(resName);
        return obj;
    }

    #region 异步加载AB包资源 普通模式
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        MonoMgr.Instance.StartCoroutine(LoadResAsyncHelper(abName, resName, callBack));
    }

    private IEnumerator LoadResAsyncHelper(string abName, string resName, UnityAction<Object> callBack)
    {
        LoadAB(abName);
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
        yield return abr;
        callBack(abr.asset);
    }
    #endregion

    #region 异步加载AB包资源 指定类型模式
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        MonoMgr.Instance.StartCoroutine(LoadResAsyncHelper(abName, resName, type, callBack));
    }

    private IEnumerator LoadResAsyncHelper(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        LoadAB(abName);
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName, type);
        yield return abr;
        callBack(abr.asset);
    }
    #endregion

    #region 异步加载AB包资源 泛型模式
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        MonoMgr.Instance.StartCoroutine(LoadResAsyncHelper<T>(abName, resName, callBack));
    }

    private IEnumerator LoadResAsyncHelper<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        LoadAB(abName);
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        callBack(abr.asset as T);
    }
    #endregion

    //单个AB包卸载
    public void UnLoad(string abName)
    {
        if(abDic.ContainsKey(abName))
        {
            abDic[abName].Unload(false);
            abDic.Remove(abName);
        }
    }

    //所有包的卸载
    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        manifest = null;
    }
}
