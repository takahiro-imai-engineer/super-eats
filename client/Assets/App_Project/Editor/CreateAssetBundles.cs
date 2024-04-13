using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles : MonoBehaviour
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
#if UNITY_ANDROID
        string assetBundleDirectory = "Assets/StreamingAssets/Android";
#elif UNITY_IOS
        string assetBundleDirectory = "Assets/StreamingAssets/iOS";
#else
        string assetBundleDirectory = "Assets/StreamingAssets/Others";
#endif
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(
            assetBundleDirectory,
            BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget
        );
    }
}