using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Firebase管理クラス
/// </summary>
public class FirebaseManager : MonoBehaviour
{
    // 参考：https://firebase.google.com/docs/unity/setup?hl=ja
    //
    // 組み込みにおける注意
    // ・SDKはFirebaseAnalytics、FirebaseRemoteConfigのように機能毎にUnityPackageで分かれているが組み込むときはバージョンを合わせること。
    // 　バージョンが違うことでUnity上で確認しようと起動したら古いバージョンの機能の処理を呼ぶとUnityが落ちることがあった。
    // ・Git100MB以上のサイズのファイルをコミットできないが、パッケージの中に100MBを超える FirebaseCppApp-7_0_2.so が含まれている。(7_0_2はバージョン)
    // 　このファイルはUnity、iOS、Androidでは必要ないため削除して良い。
    //

    //================================================================================
    // Enum
    //================================================================================
    public enum AdType
    {
        // インタースティシャル広告
        Interstitial,
        // リワード広告
        Reward,
        KeepCombo,
        // バナー
        Banner
    }

    //================================================================================
    // 定数
    //================================================================================

    //================================================================================
    // ローカル
    //================================================================================
    /// <summary>インスタンス</summary>
    private static FirebaseManager instance = null;

    //================================================================================
    // プロパティ
    //================================================================================
    /// <summary>インスタンス</summary>
    public static FirebaseManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FirebaseManager>();
                if (instance == null)
                {
                    var obj = new GameObject();
                    obj.name = "FirebaseManager";
                    instance = obj.AddComponent<FirebaseManager>();
                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }
    /// <summary>初期化済みか</summary>
    public bool Initialized
    {
        get;
        private set;
    }
    /// <summary>有効か</summary>
    public bool IsEnable
    {
        get;
        private set;
    }
    /// <summary>ユーザーID</summary>
    public string UserId
    {
        get;
        private set;
    }

    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// コンストラクタ
    /// </summary>
    private void Awake()
    {
        // 既存なら削除
        if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// デストラクタ
    /// </summary>
    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Init(UnityAction callback)
    {
#if !UNITY_EDITOR
        Debug.Log("FirebaseManager.Init()");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            Initialized = true;
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Debug.Log("Firebase Success");
                IsEnable = true;
                //FirebaseAnalytics.LogEvent (FirebaseAnalytics.EventAppOpen);
                InitFirebaseAnalytics();
            }
            else
            {
                IsEnable = false;
                callback?.Invoke();
                Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
#else
        IsEnable = false;
#endif
    }

    /// <summary>
    /// 広告タイプを文字列へ変換する
    /// </summary>
    /// <param name="adType"></param>
    /// <returns></returns>
    private string AdTypeToName(AdType adType)
    {
        return adType.ToString().ToLower();
    }

    //================================================================================
    // FirebaseAnalytics関連
    //================================================================================
    /// <summary>
    /// FirebaseAnalytics初期化処理.
    /// </summary>
    private void InitFirebaseAnalytics()
    {
        //Debug.Log("FirebaseManager.InitFirebaseAnalytics() " + IsEnable);
        if (!IsEnable) return;
        //FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        //FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));

        FirebaseAnalytics.GetAnalyticsInstanceIdAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                UserId = task.Result;
            }
        });
    }

    /// <summary>
    /// AnalyticsEventロギング処理.
    /// </summary>
    /// パラメータは以下を参考に。
    /// https://firebase.google.com/docs/reference/unity/class/firebase/analytics/firebase-analytics?hl=ja
    public void LogAnalyticsEvent(string analyticsEvent)
    {
        if (!IsEnable) return;
        FirebaseAnalytics.LogEvent(analyticsEvent);
    }

    /// <summary>
    /// アプリオープンイベントロギング処理.
    /// </summary>
    public void LogAnalyticsAppOpenEvent()
    {
        LogAnalyticsEvent(FirebaseAnalytics.EventAppOpen);
    }

    /// <summary>
    /// ゲーム開始イベントロギング処理.
    /// </summary>
    public void LogAnalyticsGameStartEvent(int level)
    {
        if (!IsEnable) return;
        FirebaseAnalytics.LogEvent("level_start", "stage_id", level);
    }

    /// <summary>
    /// ゲーム成功イベントロギング処理.
    /// </summary>
    public void LogAnalyticsGameSucceedEvent(int level)
    {
        if (!IsEnable) return;
        FirebaseAnalytics.LogEvent("level_succeed", "stage_id", level);
    }

    /// <summary>
    /// ゲーム失敗イベントロギング処理.
    /// </summary>
    public void LogAnalyticsGameFailedEvent(int level)
    {
        if (!IsEnable) return;
        FirebaseAnalytics.LogEvent("level_fail", "stage_id", level);
    }

    /// <summary>
    /// ゲームクリア時のイベントロギング処理
    /// </summary>
    public void LogAnalyticsStageClearEvent(int stageId, string stageName, int rewardCoin, bool isAdsBonus)
    {
        if (!IsEnable || string.IsNullOrEmpty(UserId))
        {
            return;
        }

        FirebaseAnalytics.LogEvent(
            "stage_clear",
            new Parameter("user_id", UserId),
            new Parameter("stage_id", stageId),
            new Parameter("stage_name", stageName),
            new Parameter("reward_coin", rewardCoin),
            new Parameter("is_ads_bonus", isAdsBonus ? 1 : 0)
        );
    }

    /// <summary>
    /// ゲーム失敗時のイベントロギング処理
    /// </summary>
    public void LogAnalyticsStageFailEvent(int stageId, string stageName, bool isKeepCombo)
    {
        if (!IsEnable || string.IsNullOrEmpty(UserId))
        {
            return;
        }

        FirebaseAnalytics.LogEvent(
            "stage_fail",
            new Parameter("user_id", UserId),
            new Parameter("stage_id", stageId),
            new Parameter("stage_name", stageName),
            new Parameter("is_keep_combo", isKeepCombo ? 1 : 0)
        );
    }

    /// <summary>
    /// ワールドクリア時のイベントロギング処理
    /// </summary>
    public void LogAnalyticsWorldClearEvent(int allClearCount)
    {
        if (!IsEnable || string.IsNullOrEmpty(UserId))
        {
            return;
        }

        FirebaseAnalytics.LogEvent(
            "world_clear",
            new Parameter("user_id", UserId),
            new Parameter("clear_count", allClearCount)
        );
    }

    /// <summary>
    /// アバター購入時のイベントロギング処理
    /// </summary>
    public void LogAnalyticsPurchaseItemEvent(TitleConstant.ShopItemType shopItemType, int contentId)
    {
        if (!IsEnable || string.IsNullOrEmpty(UserId))
        {
            return;
        }

        FirebaseAnalytics.LogEvent(
            "purchase_item",
            new Parameter("shop_item_type", (int)shopItemType),
            new Parameter("content_id", contentId)
        );
    }

    /// <summary>
    /// 画面遷移した時のイベントロギング処理
    /// </summary>
    /// <param name="screenId"></param>
    /// <param name="screenName"></param>
    public void LogAnalyticsScreenTransitionEvent(int screenId)
    {
        if (!IsEnable || string.IsNullOrEmpty(UserId))
        {
            return;
        }

        FirebaseAnalytics.LogEvent(
            "screen_transition",
            new Parameter("user_id", UserId),
            new Parameter("screen_id", screenId)
        );
    }

    /// <summary>
    /// 広告表示タイミング（インタースティシャル、リワード両方）のイベントロギング処理
    /// </summary>
    public void LogAnalyticsAdDisplayEvent(int stageId, AdType adType)
    {
        if (!IsEnable || string.IsNullOrEmpty(UserId))
        {
            return;
        }

        FirebaseAnalytics.LogEvent(
            "ad_display",
            new Parameter("user_id", UserId),
            new Parameter("stage_id", stageId),
            new Parameter("ad_type", AdTypeToName(adType))
        );
    }

    /// <summary>
    /// 広告再生終了時のイベントロギング処理
    /// </summary>
    /// <param name="stageId"></param>
    /// <param name="stageName"></param>
    /// <param name="isGetReward"></param>
    /// <param name="adType"></param>
    public void LogAnalyticsAdCloseEvent(int stageId, string stageName, bool isGetReward, AdType adType)
    {
        if (!IsEnable || string.IsNullOrEmpty(UserId))
        {
            return;
        }
        FirebaseAnalytics.LogEvent(
            "ad_close",
            new Parameter("user_id", UserId),
            new Parameter("stage_id", stageId),
            new Parameter("stage_name", stageName),
            new Parameter("close_type", isGetReward ? 1 : 0),
            new Parameter("ad_type", AdTypeToName(adType))
        );
    }

    /// <summary>
    /// 広告をクリックしたタイミングのイベントロギング処理
    /// </summary>
    /// <param name="screenId"></param>
    /// <param name="adType"></param>
    public void LogAnalyticsAdClickEvent(AdType adType)
    {
        if (!IsEnable || string.IsNullOrEmpty(UserId))
        {
            return;
        }
        FirebaseAnalytics.LogEvent(
            "ad_click",
            new Parameter("user_id", UserId),
            new Parameter("ad_type", AdTypeToName(adType))
        );
    }
}
