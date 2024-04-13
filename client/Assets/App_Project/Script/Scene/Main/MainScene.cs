using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
using UnityEngine.iOS;
#endif
using Cysharp.Threading.Tasks;

/// <summary>
/// メインシーン
/// NOTE: ATT対応参考
/// https://baba-s.hatenablog.com/entry/2021/07/26/090000
/// </summary>
public class MainScene : SceneBase
{
#if UNITY_IOS && !UNITY_EDITOR
    private IEnumerator Start()
    {
        // まだ許可ダイアログを表示したことがない場合
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
             ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            // 許可ダイアログを表示します
            ATTrackingStatusBinding.RequestAuthorizationTracking();

            // 許可ダイアログで「App にトラッキングしないように要求」か
            // 「トラッキングを許可」のどちらかが選択されるまで待機します
            while (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                    ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                yield return null;
            }
        }

        // IDFA（広告 ID）をログ出力します
        // トラッキングが許可されている場合は IDFA が文字列で出力されます
        // 許可されていない場合は「00000000-0000-0000-0000-000000000000」が出力されます  
        Debug.Log($"IDFA: {Device.advertisingIdentifier}");
        Init();
    }
#else
    private void Start()
    {
        Init();
    }
#endif

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        // 解像度の補正
        float screenRate = (float)1350 / Screen.height;
        if (screenRate > 1) screenRate = 1;
        int width = (int)(Screen.width * screenRate);
        int height = (int)(Screen.height * screenRate);
        Screen.SetResolution(width, height, true, 30);

        // リリースモードの場合、ログの抑制
        if (gameSetting.GameMode == GameMode.Release)
        {
            Debug.unityLogger.logEnabled = false;
        }

        // TenjinのSDK対応(インストールイベント対応)
        string apiKey =
            gameSetting.GameMode == GameMode.Release ?
            GameConstant.RELEASE_TENJIN_API_KEY :
            GameConstant.DEV_TENJIN_API_KEY;
        BaseTenjin instance = Tenjin.getInstance(apiKey);
#if UNITY_IOS

        // Registers SKAdNetwork app for attribution
        instance.RegisterAppForAdNetworkAttribution();

        // Sends install/open event to Tenjin
        instance.Connect();

        // Sets SKAdNetwork Conversion Value
        // You will need to use a value between 0-63 for <YOUR 6 bit value>.
        // instance.UpdateConversionValue ( < YOUR 6 bit value >);

#elif UNITY_ANDROID
        // Sends install/open event to Tenjin
        instance.Connect();
#endif
        // Firebase初期化
        FirebaseManager.Instance.Init(() =>
        {
            FirebaseManager.Instance.LogAnalyticsAppOpenEvent();
        });
        ToTitleScene(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask ToTitleScene(CancellationToken ct)
    {
        var gameLoadingScreen = OverlayGroup.Instance.Show<GameLoadingScreen>();
        gameLoadingScreen.Init();

        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(GameConstant.Scene.Title.ToString());
        asyncLoadScene.allowSceneActivation = false;
        AssetManager.Instance.ReleaseStageAsset();

        await UniTask.WaitUntil(() => asyncLoadScene.progress >= 0.9f);
#if !(UNITY_ANDROID && !UNITY_EDITOR)
        gameLoadingScreen.SetProgress(1f, 1f);
        await UniTask.Delay(System.TimeSpan.FromSeconds(1f));
#endif
        asyncLoadScene.allowSceneActivation = true;
    }
}