using System.IO;
using UnityEngine;

public class AssetBundleTest : MonoBehaviour
{
    int currentBaseNum = 0;
    GameObject baseGameObject = null;
    // Start is called before the first frame update
    void Start()
    {

    }

    AssetBundle LoadAssetBundle(string assetBundleName)
    {
#if UNITY_ANDROID
        var baseAssetBundlePath = $"{Application.streamingAssetsPath}/Android";
#elif UNITY_IOS
        var baseAssetBundlePath = $"{Application.streamingAssetsPath}/iOS";
#else
        var baseAssetBundlePath = "";
#endif
        var assetBundle = AssetBundle.LoadFromFile(Path.Combine(baseAssetBundlePath, assetBundleName));
        return assetBundle;
    }

    void LoadBase()
    {
        currentBaseNum = Mathf.Clamp(currentBaseNum++, 1, 4);
        var baseAssetBundle = LoadAssetBundle("title_base");
        var basePrefab = baseAssetBundle.LoadAsset<GameObject>($"Base0{currentBaseNum}");
        if (baseGameObject != null)
        {
            Destroy(baseGameObject);
        }
        baseGameObject = Instantiate(basePrefab);
        baseGameObject.GetComponent<TitleBaseView>().Init(2);
    }

    public void OnClickLoadBase()
    {
        LoadBase();
    }
}
