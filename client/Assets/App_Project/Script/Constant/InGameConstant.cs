public class InGameConstant
{

    public const int CityStageGroupId = 1;
    public const int CyberStageGroupId = 2;
    public const int ApoStageGroupId = 3;
    public const int LastBaseGroupId = 4;
    /// <summary>
    /// InGameStatus.
    /// </summary>
    public enum InGameStatus
    {
        // 初期化
        Init = 0,
        // トップ画面
        Top,
        // 準備中
        Prepare,
        // ゲーム中
        Play,
        // クラッシュ中
        Clash,
        //　待機状態
        Wait,
        // プレイヤー死亡
        DeathPlayer,
        // 成功
        Success,
        // 失敗
        Failed,
        // ゲーム結果
        Result,
        // ゲーム終了
        Finish,
        None = 999
    }

    public enum ResultStatus
    {
        None = 0,
        SuccessResult,
        FailedResult,
        Continue
    }

    //================================================================================
    // 自機関連の定数
    //================================================================================
    // 自機の移動速度
    public const float PLAYER_SPEED = 40.0f;
    // 自機の回転速度(左右タッチ操作時)
    public const float PLAYER_ROTATE_SPEED = 10.0f;
    // 自機の横の移動速度(ドラッグ操作時)
    public const float PLAYER_X_SPEED = 2.8f;
    // 食べ物のスケール1辺りにかかる慣性(ドラッグ操作時)
    public const float PLAYER_INRTIA_RATE = 0.1f;

    // 自機の回転制限値
    public const float PLAYER_MAX_ROTATION = 40.0f;
    // 食べ物を獲得した時にバッグが大きくなるサイズ
    public const float PLAYER_INCREASE_BAG_SCALE = 0.5f;
    // 障害物にぶつかった際にバッグが小さくなるサイズ
    public const float PLAYER_DECREASE_BAG_SCALE = 2f;
    // バッグの上限サイズ
    public const float PLAYER_MAX_BAG_SCALE = 5f;

    // ゲームオーバーになる高さ
    public const float PLAYER_GAME_OVER_HEIGHT = -10f;

    // ふっとびが発生しないクラッシュ時の復帰までの時間
    public const float PLAYER_NO_BLOW_CLASH_WAIT_TIME = 1f;

    // クラッシュ時の復帰までの時間
    public const float PLAYER_CLASH_WAIT_TIME = 3f;
    // クラッシュ時、全回収に必要なタッチ回数
    public const int PLAYER_CLASH_NECESSARY_TOUCH_COUNT = 15;
    // クラッシュした際に最大回収できるサイズ
    public const float PLAYER_MAX_RECOVERY_BAG_SIZE = 1.5f;

    // ラグドール停止まで(未使用)
    public const float CHARACTER_STOP_DEATH_TIME = 2.0f;

    // 難易度により上昇する食べ物価格
    public const int FoodDifficultyTwoBonus = 2;
    public const int FoodDifficultyThreeBonus = 6;
    public const int FoodDifficultyFourBonus = 10;
    public const int FoodDifficultyFiveBonus = 15;

    // 広告による獲得コインの増加倍率
    public const int RESULT_ADS_BONUS_RATE = 2;

    public const int INIT_STAGE_LEVEL = 1;

    // 強制広告表示カウント開始のステージ数
    public const int INTERSTITIAL_AD_STAGE_THRESHOLD_VALUE = 3;

    public const int BAG_TUTORIAL_ID = 101;
    public const int AVATAR_TUTORIAL_ID = 102;
    public const int BICYCLE_TUTORIAL_ID = 103;
    public const int FIRST_PURCHASE_CONTENT_ID = 2;


    //================================================================================
    // 自機状態
    //================================================================================
    /// <summary>
    /// プレイヤーの状況
    /// </summary>
    public enum PlayerStatus
    {
        None = 0,
        Prepare, // 準備
        Move, // 移動中
        Clash, // クラッシュ
        Blow, // 吹っ飛び
        Death, // 死亡
        FallDeath, // 落下死
        Goal, // ゴール
        Retry, // リトライ
        Finish // 終了
    }

    /// <summary>
    /// 衝突イベント
    /// </summary>
    public enum HitEvent
    {
        None,
        Coin,
        BonusCoin,
        Jewel,
        Food,
        Obstacle,
        Goal,
    }

    /// <summary>
    /// 吹っ飛びタイプ
    /// </summary>
    public enum BlowType
    {
        None,
        Immovable, // 動かない
        Moving, // 動く
    }

    /// <summary>
    /// ライバル配達員のタイプ
    /// </summary>
    public enum RivalType
    {
        None,
        FoodDelivery,
    }


    /// <summary>
    /// ギミックSE
    /// </summary>
    public enum GimickSE
    {
        None = 0,
        CarHorn,
        Silen,
        PoliceCar,
        TrafficJam,
        ConcreteFall,
        CarIn,
        RivalIn,
        BoxFall,
        ContainerMove,
        ContainerFall,
        TruckMove,
        TruckHorn,
        HoverCarFall,
        EnemyDrone,
        RivalDrone,
        BigShipAlarm,
        BigShipEngine,
        DoorClose,
        CyberCarIn,
        CrossHover,
        HoverCarStart,
        Robot,
        TimeToWarp
    }

    public enum TutorialType
    {
        None,
        PopUpDialog, // 画像ポップアップ
        OperationInstruction, // 操作説明
        Home, // ホーム
    }

    public enum TutorialOperationType
    {
        None,
        Move,
        Heavy
    }

    /// <summary>
    /// タッチイベント
    /// </summary>
    public enum TouchEvent
    {
        None,
        Right,
        Left
    }

    //================================================================================
    // ボタン関連
    //================================================================================
    /// <summary> ボタンタイプ </summary>
    public enum ButtonType
    {
        None = 0,
        Start,
        Setting,
        Retry,
        Continue,
        Bonus,
        NextGame,
        Title
    }

    /// <summary> 設定ボタンタイプ </summary>
    public enum SettingButtonType
    {
        None = 0,
        Bgm,
        Se,
        // お問い合わせ
        Contact,
        // 利用規約
        TermsOfService,
        // プライバシーポリシー
        PrivacyPolicy
    }
}