using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Google.Android.AppBundle.Editor;
using Google.Android.AppBundle.Editor.AssetPacks;
using Google.Android.AppBundle.Editor.Internal;

namespace Build
{

    public class PublishPlayAssetDelivery : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        /// <summary>
        /// ビルドする前に実行される
        /// </summary>
        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("PublishPlayAssetDelivery OnPreprocessBuild");
            if (EditorUserBuildSettings.buildAppBundle && report.summary.platform == BuildTarget.Android)
            {
                BuildRuntimeAssets.BuildRTAssets_AssetPacks_Scripted();
            }
        }

        /// <summary>
        /// ビルドした後に実行される
        /// </summary>
        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
            {
                return;
            }

            Debug.Log("PublishPlayAssetDelivery OnPostprocessBuild");
            if (EditorUserBuildSettings.buildAppBundle && report.summary.platform == BuildTarget.Android)
            {
                var config = AssetPackConfigSerializer.LoadConfig();
                AppBundlePublisher.PackAsset(report.summary.outputPath, config);
            }
        }
    }
}