using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームモード
/// </summary>
public enum GameMode {
    // リリース
    Release = 1,
    // デバッグ
    Debug,
    // 動画撮影
    Movie
}

/// <summary>
/// ゲームセッティング
/// </summary>
//[CreateAssetMenu]
public class GameSetting : ScriptableObject {

    //================================================================================
    // アプリ設定
    //================================================================================
    [Header ("[Application Setting]")]
    /// <summary>ゲームモード</summary>
    public GameMode GameMode = GameMode.Debug;

    /// <summary>フレームレート</summary>
    public int FrameRate = 60;

    //================================================================================
    // ダイアログマネージャ設定（DialogManager）
    //================================================================================
    [Header ("[DialogManager Setting]")]

    /// <summary>常駐ダイアログアセット</summary>
    public DialogContainer ResidentialDialogAsset;

    /// <summary>デフォルトのボタンデスクリプタのリスト</summary>
    public List<DialogManager.ButtonDesc> DefaultButtonDescList = new List<DialogManager.ButtonDesc> ();

    //================================================================================
    // オーバーレイグループ設定（OverlayGroup）
    //================================================================================
    [Header ("[OverlayGroup Setting]")]

    /// <summary>常駐オーバーレイアセット</summary>
    public OverlayContainer ResidentialOverlayAsset;

    /// <summary>シャットダウン時に削除しないオーバーレイオブジェクトの名前リスト</summary>
    public List<string> KeepAliveOverlayObjects = new List<string> ();

    //================================================================================
    // サウンドマネージャ設定（SoundManager）
    //================================================================================
    [Header ("[SoundManager Setting]")]

    /// <summary>常駐サウンドアセットのリスト</summary>
    public List<SoundContainer> ResidentialSoundAssetList = new List<SoundContainer> ();

    //================================================================================
    // シーンマネージャ設定（SceneLoadManager）
    //================================================================================
    [Header ("[SceneLoadManager Setting]")]

    /// <summary>履歴をクリアするシーン名のリスト</summary>
    public List<string> HistoryClearSceneNameList = new List<string> ();

    //================================================================================
    // アセットロードマネージャ設定（AssetLoadManager）
    //================================================================================
    [Header ("[AssetLoadManager Setting]")]

    /// <summary>ログインしない場合のアセットバンドルベース URL</summary>
    public string BaseUrlAtNoLogin = "https://s3-ap-northeast-1.amazonaws.com/projecttg/development/v1/assetbundle/";

    /// <summary>アセットバンドルベース URL のプラットフォーム変更名（Unity エディタ時）</summary>
    public string EditorPlatformName = "editor";
    public string EditorPlatformNameForMac = "editor_mac"; // Mac 用

    /// <summary>ダウンロードしたアセットバンドル等をストレージ保存する際のフォルダ名</summary>
    public string SavedFolder = "Cache";

    /// <summary>ストレージ保存するアセットの拡張子</summary>
    public string SavedExtension = ".bin";

    /// <summary>同時に起動できるアセットローダーの最大数</summary>
    public int MaxActiveAssetLoaderCount = 64;

    /// <summary>WWW リクエストのタイムアウト時間</summary>
    public float WwwRequestTimeout = 30.0f;

#if UNITY_EDITOR
    /// <summary>WWW リクエストエラーを強制的に発生させる</summary>
    [HideInInspector]
    [System.NonSerialized]
    public bool IsForceWwwError;

    /// <summary>ローカルアセットバンドルを有効にする</summary>
    [HideInInspector]
    [System.NonSerialized]
    public bool IsLocalAssetBundleEnabled;

    /// <summary>ローカルアセットバンドルのパス</summary>
    public string LocalAssetBundlePath;
#endif

    //================================================================================
    // スレッドマネージャ設定（ThreadManager）
    //================================================================================
    [Header ("[ThreadManager Setting]")]

    /// <summary>最大スレッド数</summary>
    public int MaxThreadCount = 8;

    //================================================================================
    // API マネージャ設定（ApiManager）
    //================================================================================
    [Header ("[ApiManager Setting]")]

    /// <summary>接続先ドメインキー（S3 にドメインが列挙されたテキストがあり、どのドメインを選択するかを決めるキー）</summary>
    public string DomainKey = "Domain Key";

    /// <summary>リクエストタイムアウト時間</summary>
    public int RequestTimeOut = 10;

    /// <summary>UniWeb レスポンスバッファサイズ</summary>
    public int UniWebResponseBufferSize = 4 * 1024 * 1024;

#if UNITY_EDITOR
    /// <summary>サーバーエラーを強制的に発生させる</summary>
    [HideInInspector]
    [System.NonSerialized]
    public bool IsForceServerError;

    /// <summary>サーバーエラーを強制的に発生させる API 名</summary>
    [HideInInspector]
    [System.NonSerialized]
    public string ForceErrorApi = string.Empty;

    /// <summary>サーバーエラーを強制的に発生させた時のリザルトコード</summary>
    [HideInInspector]
    [System.NonSerialized]
    public string ForceErrorResultCode = string.Empty;
#endif
}