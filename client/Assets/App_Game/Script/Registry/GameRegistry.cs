using UnityEngine;
using app_system;

/// <summary>
/// ゲームレジストリ
/// </summary>
public class GameRegistry : MonoBehaviourSingleton<GameRegistry>
{

    /// <summary>オリジナル解像度・幅</summary>
    private int originalScreenWidth;

    /// <summary>オリジナル解像度・高さ</summary>
    private int originalScreenHeight;

    /// <summary>シーン単独でゲームを起動した時のシーン名</summary>
    private static string selfLaunchedSceneName;

    /// <summary>
    /// シーン単独でゲームを起動したかどうか
    /// </summary>
    public static bool IsSelfLaunched
    {
        get
        {
            return string.IsNullOrEmpty(selfLaunchedSceneName) == false;
        }
        set
        {
            // シーン単独でゲームを起動した時のシーン名を設定
            if (value)
            {
                selfLaunchedSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            }
            else
            {
                selfLaunchedSceneName = null;
            }
        }
    }

    /// <summary>
    /// シーン単独でゲームを起動した時のシーン名
    /// </summary>
    public static string SelfLaunchedSceneName
    {
        get { return selfLaunchedSceneName; }
    }

#if !APP_RELEASE
    /// <summary>開始時の GC 数</summary>
    private int startGcCount;
#endif

    //================================================================================
    // Mono
    //================================================================================

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private new void Awake()
    {
        // 既にインスタンスが存在する場合は何もしない
        if (existInstance())
        {
            return;
        }
        base.Awake();

        // 常駐
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        // オリジナル解像度
        originalScreenWidth = Screen.width;
        originalScreenHeight = Screen.height;

#if APP_RELEASE
        // デバッグログ無効
        UnityEngine.Debug.unityLogger.logEnabled = false;
#elif !UNITY_EDITOR
        // デバッグログ無効
        // UnityEngine.Debug.unityLogger.filterLogType = LogType.Warning;
#endif

#if !APP_RELEASE
        // 開始時の GC 数
        startGcCount = System.GC.CollectionCount(0);
#endif
    }

    //================================================================================
    // メソッド
    //================================================================================

    /// <summary>
    /// メモリクリーン
    /// </summary>
    /// <param name="isGC">GC 有効</param>
    public static void MemoryClean(bool isGC = false)
    {
        if (isGC)
        {
            System.GC.Collect();
        }
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// フレームレートの設定
    /// </summary>
    /// <param name="frameRate">フレームレート</param>
    public static void SetFrameRate(int frameRate)
    {
        QualitySettings.vSyncCount = 0;     // 0 .. Don't Sync
#if UNITY_WEBGL
        Application.targetFrameRate = 0;
#else
        Application.targetFrameRate = frameRate;
#endif
        Debug.Log($"フレームレート変更: {frameRate}");
    }

    /// <summary>
    /// 解像度の設定
    /// </summary>
    /// <param name="width">幅</param>
    public static void SetResolution(int width)
    {
#if !UNITY_WEBGL
        // アスペクト比を維持
        var aspect = (float)Instance.originalScreenHeight / Instance.originalScreenWidth;
        var height = (int)(width * aspect);

        // 解像度
        Screen.SetResolution(width, height, Screen.fullScreen, Application.targetFrameRate);
#endif
    }

    /// <summary>
    /// オリジナル解像度・幅
    /// </summary>
    public static int OriginalScreenWidth
    {
        get { return Instance.originalScreenWidth; }
    }

    /// <summary>
    /// オリジナル解像度・幅
    /// </summary>
    public static int OriginalScreenHeight
    {
        get { return Instance.originalScreenHeight; }
    }

#if !APP_RELEASE
    /// <summary>
    /// GC 回数
    /// </summary>
    public static int GCCount
    {
        get { return System.GC.CollectionCount(0) - Instance.startGcCount; }
    }
#endif

    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting(GameSetting gameSetting)
    {
        if (gameSetting == null)
        {
            Debug.LogError("No GameSetting!");
            return;
        }

        // フレームレート設定
        SetFrameRate(gameSetting.FrameRate);

        // それぞれのゲームセッティングのセットアップを呼ぶ
        var registries = GetComponentsInChildren<IGameRegistry>();
        foreach (var registry in registries)
        {
            registry.SetupGameSetting(gameSetting);
        }

        // スレッドマネージャは App_System なので直接設定
        ThreadManager.Instance.MaxThreadCount = gameSetting.MaxThreadCount;

        // DOTween 初期化
        DG.Tweening.DOTween.Init();
    }

    /// <summary>
    /// シャットダウン
    /// 
    /// タイトルへ戻る等、全リセットを行う場合に使用する
    /// </summary>
    public void Shutdown()
    {

        // それぞれのシャットダウンを呼ぶ
        var registries = GetComponentsInChildren<IGameRegistry>();
        foreach (var registry in registries)
        {
            registry.Shutdown();
        }

        // メモリクリーン
        MemoryClean(true);    // GC 有効
    }
}
