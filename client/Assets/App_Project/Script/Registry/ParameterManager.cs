using app_system;
using Unity.RemoteConfig;
using UnityEngine;
using UnityEngine.Events;

public class ParameterManager : MonoBehaviourSingleton<ParameterManager>, IGameRegistry
{

    //================================================================================
    // ローカル
    //================================================================================
    /// <summary></summary>
    private PlayerParameter playerParameter = new PlayerParameter();

    private UnityAction loadParameterCompleteCallback = null;
    //================================================================================
    // プロパティ
    //================================================================================
    public PlayerParameter PlayerParameter => playerParameter;
    public int ShowInterstitialAdCount { get; private set; } = 2;
    /// <summary>RemoteConfigのロード完了したか</summary>
    public bool IsLoadConfigComplete { get; private set; }

    //================================================================================
    // メソッド
    //================================================================================
    protected override void Awake()
    {
        // 既にインスタンスが存在する場合は何もしない
        if (existInstance())
        {
            return;
        }
        base.Awake();
        ConfigManager.FetchCompleted += ApplyRemoteSettings;
    }

    private void OnDestroy()
    {
        ConfigManager.FetchCompleted -= ApplyRemoteSettings;
    }

    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting(GameSetting gameSetting)
    {

    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(UnityAction loadParameterCompleteCallback)
    {
        this.playerParameter = new PlayerParameter();
        this.loadParameterCompleteCallback = loadParameterCompleteCallback;
        IsLoadConfigComplete = false;
    }

    private struct userAttributes { }
    private struct appAttributes { }

    public void FetchParameter()
    {
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    private void ApplyRemoteSettings(ConfigResponse configResponse)
    {

        // Conditionally update settings, depending on the response's origin:
        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                break;
            case ConfigOrigin.Cached:
            case ConfigOrigin.Remote:
                playerParameter.ApplyParameter();
                ShowInterstitialAdCount = ConfigManager.appConfig.GetInt("SHOW_INTERSTITIAL_AD_COUNT");
                Debug.Log("RemoteConfig" + configResponse.requestOrigin + "Complete");
                break;
        }
        IsLoadConfigComplete = true;
        loadParameterCompleteCallback.Invoke();
    }

    /// <summary>
    /// シャットダウン
    /// 
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    public void Shutdown()
    {

    }
}