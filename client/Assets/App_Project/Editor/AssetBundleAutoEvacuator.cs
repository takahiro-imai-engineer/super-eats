using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Build
{
    public class AssetBundleAutoEvacuator : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        /// <summary>移動するAndroidアセットのディレクトリのパス</summary>
        private const string TARGET_ANDROID_ASSET_DIRECTORY_PATH = "Assets/StreamingAssets/Android";
        /// <summary>移動するiOSアセットのディレクトリのパス</summary>
        private const string TARGET_IOS_ASSET_DIRECTORY_PATH = "Assets/StreamingAssets/iOS";
        /// <summary>移動先のディレクトリのパス</summary>
        private const string DESTINATION_DIRECTORY_ANDROID_PATH = "Assets/TempAssets/Android";
        private const string DESTINATION_DIRECTORY_IOS_PATH = "Assets/TempAssets/iOS";

        public int callbackOrder => 0;

        /// <summary>
        /// ビルドする前に実行される
        /// </summary>
        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("AssetPreprocessBuild OnPreprocessBuild");

            try
            {
                var targetPath = report.summary.platform switch
                {
                    BuildTarget.Android => TARGET_IOS_ASSET_DIRECTORY_PATH,
                    BuildTarget.iOS => TARGET_ANDROID_ASSET_DIRECTORY_PATH,
                    _ => ""
                };
                var destinationPath = report.summary.platform switch
                {
                    BuildTarget.Android => DESTINATION_DIRECTORY_IOS_PATH,
                    BuildTarget.iOS => DESTINATION_DIRECTORY_ANDROID_PATH,
                    _ => ""
                };
                if (string.IsNullOrEmpty(targetPath))
                {
                    Debug.LogError("AndroidとiOS以外は未対応です。");
                    return;
                }
                if (!Directory.Exists("Assets/TempAssets"))
                {
                    Directory.CreateDirectory("Assets/TempAssets");
                }
                if (EditorUserBuildSettings.buildAppBundle)
                {
                    // AABビルドなら、PlayAssetDeliveryに含まれるためAndroidも取り除く
                    MoveDirectory(TARGET_ANDROID_ASSET_DIRECTORY_PATH, DESTINATION_DIRECTORY_ANDROID_PATH);
                }
                MoveDirectory(targetPath, destinationPath);

            }
            catch (IOException e)
            {
                Debug.LogWarning(e);
            }
        }

        private static void MoveDirectory(string targetPath, string destinationPath)
        {
            if (File.Exists($"{destinationPath}.meta"))
            {
                File.Delete($"{destinationPath}.meta");
            }
            if (Directory.Exists(destinationPath))
            {
                Directory.Delete(destinationPath);
            }
            // ディレクトリを移動
            Directory.Move(targetPath, destinationPath);
            // metaファイルも移動
            File.Move(targetPath + ".meta", destinationPath + ".meta");
        }

        /// <summary>
        /// ビルドした後に実行される
        /// </summary>
        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log("AssetPostprocessBuild OnPostprocessBuild");
            try
            {
                var targetPath = report.summary.platform switch
                {
                    BuildTarget.Android => TARGET_IOS_ASSET_DIRECTORY_PATH,
                    BuildTarget.iOS => TARGET_ANDROID_ASSET_DIRECTORY_PATH,
                    _ => ""
                };
                var destinationPath = report.summary.platform switch
                {
                    BuildTarget.Android => DESTINATION_DIRECTORY_IOS_PATH,
                    BuildTarget.iOS => DESTINATION_DIRECTORY_ANDROID_PATH,
                    _ => ""
                };
                if (string.IsNullOrEmpty(targetPath))
                {
                    Debug.LogError("AndroidとiOS以外は未対応です。");
                    return;
                }
                if (EditorUserBuildSettings.buildAppBundle)
                {
                    // AABビルドなら、PlayAssetDeliveryに含まれるためAndroidも取り除く
                    ReturnDirectory(TARGET_ANDROID_ASSET_DIRECTORY_PATH, DESTINATION_DIRECTORY_ANDROID_PATH);
                }
                ReturnDirectory(targetPath, destinationPath);
            }
            catch (IOException e)
            {
                Debug.LogWarning(e);
            }
        }

        private static void ReturnDirectory(string targetPath, string destinationPath)
        {
            if (File.Exists($"{targetPath}.meta"))
            {
                File.Delete($"{targetPath}.meta");
            }
            if (Directory.Exists(targetPath))
            {
                Directory.Delete(targetPath);
            }
            // 移動したディレクトリを戻す
            Directory.Move(destinationPath, targetPath);
            // metaファイルも移動
            File.Move(destinationPath + ".meta", targetPath + ".meta");
        }

        [MenuItem("Tools/Test MoveAsset")]
        public static void MoveAsset()
        {

            var targetPath = TARGET_IOS_ASSET_DIRECTORY_PATH;
            var destinationPath = DESTINATION_DIRECTORY_IOS_PATH;
            if (string.IsNullOrEmpty(targetPath))
            {
                Debug.LogError("AndroidとiOS以外は未対応です。");
                return;
            }
            if (EditorUserBuildSettings.buildAppBundle)
            {
                // AABビルドなら、PlayAssetDeliveryに含まれるためAndroidも取り除く
                MoveDirectory(TARGET_ANDROID_ASSET_DIRECTORY_PATH, DESTINATION_DIRECTORY_ANDROID_PATH);
            }
            MoveDirectory(targetPath, destinationPath);
        }

        [MenuItem("Tools/Test ReturnAsset")]
        public static void ReturnAsset()
        {

            var targetPath = TARGET_IOS_ASSET_DIRECTORY_PATH;
            var destinationPath = DESTINATION_DIRECTORY_IOS_PATH;
            if (string.IsNullOrEmpty(targetPath))
            {
                Debug.LogError("AndroidとiOS以外は未対応です。");
                return;
            }
            if (EditorUserBuildSettings.buildAppBundle)
            {
                // AABビルドなら、PlayAssetDeliveryに含まれるためAndroidも取り除く
                ReturnDirectory(TARGET_ANDROID_ASSET_DIRECTORY_PATH, DESTINATION_DIRECTORY_ANDROID_PATH);
            }
            ReturnDirectory(targetPath, destinationPath);
        }
    }
}