using UnityEngine;
using GFramework;
using System.Collections;

public class GameLauncher : MonoBehaviour
{
    void Awake()
    {
        RedDotManager.Instance.Initilize();
    }

    void Start()
    {
        //this.gameObject.GetComponents<UnityEngine.UI.Text>("txtTest");
        BagMgr.Instance.InitItemInfo();
        UIMgr.Instance.OpenUI("UIMain");

        //test ABMgr
        ABMgr.Instance.LoadResAsync<GameObject>("model", "cube", (obj) =>
        {
            Instantiate(obj);
        });
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