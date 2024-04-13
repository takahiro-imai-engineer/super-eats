using app_system;
using UnityEngine;
using Unity.Services.Core;
using UnityEngine.Events;

public class AdsManager : MonoBehaviourSingleton<AdsManager>, IGameRegistry
{
    //================================================================================
    // インスペクタ
    //================================================================================
#if UNITY_ANDROID
    private string ironSourceAppKey = "XXXXXXXXXXXXXX";
#elif UNITY_IPHONE
    private string ironSourceAppKey = "XXXXXXXXXXXXXX";
#else
    private string ironSourceAppKey = "";
#endif
    //================================================================================
    // ローカル
    //================================================================================
    private bool isDebugMode = false;
    private bool isShowAds = true;
    private UnityAction onCompleteAd = null;
    private UnityAction onFailedAd = null;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting(GameSetting gameSetting)
    {
        isDebugMode = gameSetting.GameMode != GameMode.Release;
    }

    /// <summary>
    /// シャットダウン
    /// 
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    public void Shutdown() { }

    public void SetIsShowAds(bool enable)
    {
        isShowAds = enable;
    }

    protected override void Awake()
    {
        // 既にインスタンスが存在する場合は何もしない
        if (existInstance())
        {
            return;
        }
        base.Awake();
        InitializeAds();
    }

    //================================================================================
    // 広告初期化
    //================================================================================
    public void InitializeAds()
    {
        UnityServices.InitializeAsync();
        if (isDebugMode)
        {
            IronSource.Agent.setMetaData("is_test_suite", "enable");
            IronSource.Agent.validateIntegration();
        }
        // SDK init
        IronSource.Agent.init(ironSourceAppKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.OFFERWALL, IronSourceAdUnits.BANNER);
        InitAdsEvent();
    }

    void InitAdsEvent()
    {
        //Add Init Event
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

        //Add Rewarded Video Events
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;

        // Add Interstitial Events
        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;

        // Add Banner Events
        IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;

        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

        //Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

        //Add AdInfo Banner Events
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;
    }

    void OnApplicationPause(bool isPaused)
    {
        Debug.Log("unity-script: OnApplicationPause = " + isPaused);
        IronSource.Agent.onApplicationPause(isPaused);
    }

    //=================================================================================
    // 初期化コールバック
    //=================================================================================
    /// <summary>
    /// 広告初期化
    /// </summary>
    void SdkInitializationCompletedEvent()
    {
        // NOTE: 一番最初に呼び出される
        Debug.Log("unity-script: I got SdkInitializationCompletedEvent");
    }

    public void LaunchTestSuite()
    {
        if (isDebugMode)
        {
            IronSource.Agent.launchTestSuite();
        }
    }

    //=================================================================================
    // リワード広告イベント
    //=================================================================================
    public void LoadRewardAd(UnityAction onCompleteAdCallback, UnityAction onFailedAd)
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        this.onCompleteAd = onCompleteAdCallback;
        this.onFailedAd = onFailedAd;
        ShowRewardedAd();
    }

    public void ShowRewardedAd()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
#if UNITY_EDITOR
            onCompleteAd?.Invoke();
            onCompleteAd = null;
#else
            onFailedAd?.Invoke();
            onFailedAd = null;
#endif
        }
    }

    void RewardedVideoAdOpenedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
    }

    void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
    {
        // ログの例
        //  unity-script: I got RewardedVideoAdRewardedEvent, amount = 1 name = Virtual Item
        Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
    }

    void RewardedVideoAdClosedEvent()
    {
        // NOTE: リワード広告の右上のバツを押した時に呼ばれる
        Debug.Log("unity-script: I got RewardedVideoAdClosedEvent");
        onCompleteAd?.Invoke();
        onCompleteAd = null;
    }

    void RewardedVideoAdStartedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
    }

    void RewardedVideoAdEndedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
    }

    void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
    {
        // 利用可能な広告があるかどうか
        // 初期化時とリワード広告の時に呼ばれる
        Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
    }

    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
        onFailedAd?.Invoke();
        onFailedAd = null;
    }

    void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
    {
        Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
    }

    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdOpenedEvent With AdInfo " + adInfo.ToString());
    }
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdClosedEvent With AdInfo " + adInfo.ToString());
    }
    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdAvailable With AdInfo " + adInfo.ToString());
    }
    void RewardedVideoOnAdUnavailable()
    {
        // NOTE: リワード広告を用意できなかった時に呼び出される(通信接続なし等)
        Debug.Log("unity-script: I got RewardedVideoOnAdUnavailable");
    }
    void RewardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent With Error" + ironSourceError.ToString() + "And AdInfo " + adInfo.ToString());
        onFailedAd?.Invoke();
        onFailedAd = null;
    }
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        // ログの例
        // unity-script: I got ReardedVideoOnAdAvailable With AdInfo IronSourceAdInfo {auctionId='088414f0-db94-11ed-9ef3-814f4df973b9_558563228', adUnit='rewarded_video', country='JP', ab='A', segmentName='', adNetwork='ironsource', instanceName='Bidding', instanceId='13499851', revenue=0.099, precision='BID', lifetimeRevenue=0.2976825, encryptedCPM=''}
        Debug.Log("unity-script: I got RewardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement.ToString() + "And AdInfo " + adInfo.ToString());
    }
    void RewardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdClickedEvent With Placement" + ironSourcePlacement.ToString() + "And AdInfo " + adInfo.ToString());
    }

    //=================================================================================
    // 強制広告イベント
    //=================================================================================
    public void LoadInterstitialAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        IronSource.Agent.loadInterstitial();
    }

    public void ShowInterstitialAd(UnityAction onCompleteAdCallback, UnityAction onFailedAd)
    {
        if (IronSource.Agent.isInterstitialReady())
        {
            this.onCompleteAd = onCompleteAdCallback;
            this.onFailedAd = onFailedAd;
            IronSource.Agent.showInterstitial();
        }
        else
        {
            Debug.Log("unity-script: IronSource.Agent.isInterstitialReady - False");
            onFailedAd?.Invoke();
            onFailedAd = null;
        }
    }

    void InterstitialAdReadyEvent()
    {
        // 強制広告開始時に呼び出される
        Debug.Log("unity-script: I got InterstitialAdReadyEvent");
    }

    void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got InterstitialAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
        onFailedAd?.Invoke();
        onFailedAd = null;
    }

    void InterstitialAdShowSucceededEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdShowSucceededEvent");
    }

    void InterstitialAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got InterstitialAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
        onFailedAd?.Invoke();
        onFailedAd = null;
    }

    void InterstitialAdClickedEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdClickedEvent");
    }

    void InterstitialAdOpenedEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdOpenedEvent");
    }

    void InterstitialAdClosedEvent()
    {
        // 強制広告の右上の×ボタンを押した時に呼び出される
        Debug.Log("unity-script: I got InterstitialAdClosedEvent");
        onCompleteAd?.Invoke();
        onCompleteAd = null;
    }

    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {
        // NOTE: 強制広告開始時に呼び出される
        // ログの例
        // unity-script: I got InterstitialOnAdReadyEvent With AdInfo IronSourceAdInfo {auctionId='53a62c50-db96-11ed-8837-59c0b9849e65_933730263', adUnit='interstitial', country='JP', ab='A', segmentName='', adNetwork='unityads', instanceName='Default', instanceId='Interstitial_Android', revenue=0.0011658, precision='CPM', lifetimeRevenue=0.2001808, encryptedCPM=''}
        Debug.Log("unity-script: I got InterstitialOnAdReadyEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        // NOTE: 強制広告が用意できなかった時に呼び出される(通信接続なし等)
        // ログの例
        // unity-script: I got InterstitialOnAdLoadFailed With Error 508 : Interstitial - init() had failed
        Debug.Log("unity-script: I got InterstitialOnAdLoadFailed With Error " + ironSourceError.ToString());
    }

    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        // NOTE: 強制広告を閉じる時に呼び出される？
        // ログの例
        // unity-script: I got InterstitialOnAdOpenedEvent With AdInfo IronSourceAdInfo {auctionId='53a62c50-db96-11ed-8837-59c0b9849e65_933730263', adUnit='interstitial', country='JP', ab='A', segmentName='', adNetwork='unityads', instanceName='Default', instanceId='Interstitial_Android', revenue=0.0011658, precision='CPM', lifetimeRevenue=0.2001808, encryptedCPM=''}
        Debug.Log("unity-script: I got InterstitialOnAdOpenedEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {
        // NOTE: 強制広告が閉じる時に呼び出される
        Debug.Log("unity-script: I got InterstitialOnAdShowSucceededEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdShowFailedEvent With Error " + ironSourceError.ToString() + " And AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        // NOTE: 閉じる時に呼び出される
        // ログの例
        // unity-script: I got InterstitialOnAdClosedEvent With AdInfo IronSourceAdInfo {auctionId='53a62c50-db96-11ed-8837-59c0b9849e65_933730263', adUnit='interstitial', country='JP', ab='A', segmentName='', adNetwork='unityads', instanceName='Default', instanceId='Interstitial_Android', revenue=0.0011658, precision='CPM', lifetimeRevenue=0.2001808, encryptedCPM=''}
        Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo.ToString());
    }

    //=================================================================================
    // バナー広告イベント
    //=================================================================================
    public void LoadBanner(IronSourceBannerPosition bannerPosition = IronSourceBannerPosition.BOTTOM)
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, bannerPosition);
    }

    public void HideBannerAd()
    {
        IronSource.Agent.destroyBanner();
    }

    void BannerAdLoadedEvent()
    {
        Debug.Log("unity-script: I got BannerAdLoadedEvent");
    }

    void BannerAdLoadFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got BannerAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
    }

    void BannerAdClickedEvent()
    {
        Debug.Log("unity-script: I got BannerAdClickedEvent");
    }

    void BannerAdScreenPresentedEvent()
    {
        Debug.Log("unity-script: I got BannerAdScreenPresentedEvent");
    }

    void BannerAdScreenDismissedEvent()
    {
        Debug.Log("unity-script: I got BannerAdScreenDismissedEvent");
    }

    void BannerAdLeftApplicationEvent()
    {
        Debug.Log("unity-script: I got BannerAdLeftApplicationEvent");
    }

    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdLoadedEvent With AdInfo " + adInfo.ToString());
    }

    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
    {
        Debug.Log("unity-script: I got BannerOnAdLoadFailedEvent With Error " + ironSourceError.ToString());
    }

    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdClickedEvent With AdInfo " + adInfo.ToString());
        FirebaseManager.Instance.LogAnalyticsAdClickEvent(FirebaseManager.AdType.Banner);
    }

    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdScreenPresentedEvent With AdInfo " + adInfo.ToString());
    }

    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdScreenDismissedEvent With AdInfo " + adInfo.ToString());
    }

    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdLeftApplicationEvent With AdInfo " + adInfo.ToString());
    }
}
