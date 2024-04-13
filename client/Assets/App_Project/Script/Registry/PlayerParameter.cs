using Unity.RemoteConfig;

/// <summary>
/// プレイヤーのパラメータ
/// Remoteconfigと定数で管理
/// </summary>
public class PlayerParameter
{
    //================================================================================
    // 定数
    //================================================================================
    /// <summary>ドラッグ値の最小値(px)</summary>
    public float DRAG_LIMIT_MIN = 0.05f;

    /// <summary>食べ物を獲得した時にバッグが大きくなるサイズ</summary>
    public float PLAYER_INCREASE_BAG_SCALE = 0.4f;
    /// <summary>障害物にぶつかった際にバッグが小さくなるサイズ</summary>
    public float PLAYER_DECREASE_BAG_SCALE = 0.4f;
    /// <summary>バッグの上限サイズ</summary>
    public float PLAYER_MAX_BAG_SCALE = 3.2f;
    //================================================================================
    // 通常状態
    //================================================================================
    /// <summary>滑りの最大値</summary>
    public float LIMIT_INERTIA_X = 1f;
    /// <summary>１フレーム辺りのX軸の移動量の最大値</summary>
    public float LIMIT_PLAYER_X_SPEED = 9f;
    /// <summary>徐々に減っていくX軸の慣性の倍率</summary>
    public float PLAYER_DECREASE_INRTIA_X_RATE = 0f;
    /// <summary>食べ物のスケール1辺りにかかる慣性</summary>
    public float PLAYER_INRTIA_RATE = 0f;
    /// <summary>自機の傾きの制限</summary>
    public float PLAYER_LIMIT_TILT = 30f;
    /// <summary>自機の横の移動速度(ドラッグ操作時)</summary>
    public float PLAYER_X_SPEED = 4.2f;
    /// <summary>空回り中の戻り値</summary>
    public float SPIN_WHEEL_PLAYER_X_SPEED_RATE = 2f;
    //================================================================================
    // 滑り状態
    //================================================================================
    /// <summary>滑りの最大値</summary>
    public float SLIP_LIMIT_INERTIA_X = 5f;
    /// <summary>１フレーム辺りのX軸の移動量の最大値</summary>
    public float SLIP_LIMIT_PLAYER_X_SPEED = 1.5f;
    /// <summary>徐々に減っていくX軸の慣性の倍率</summary>
    public float SLIP_PLAYER_DECREASE_INRTIA_X_RATE = 15f;
    /// <summary>食べ物のスケール1辺りにかかる慣性</summary>
    public float SLIP_PLAYER_INRTIA_RATE = 1.2f;
    /// <summary>自機の傾きの制限</summary>
    public float SLIP_PLAYER_LIMIT_TILT = 40f;
    /// <summary>自機の横の移動速度(ドラッグ操作時)</summary>
    public float SLIP_PLAYER_X_SPEED = 12f;
    /// <summary>空回り中の戻り値</summary>
    public float SLIP_SPIN_WHEEL_PLAYER_X_SPEED_RATE = 2f;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlayerParameter() { }

    /// <summary>
    /// パラメータを更新
    /// </summary>
    public void ApplyParameter()
    {
        // 操作関連
        this.DRAG_LIMIT_MIN = ConfigManager.appConfig.GetFloat("DRAG_LIMIT_MIN");
        // バッグ関連
        this.PLAYER_INCREASE_BAG_SCALE = ConfigManager.appConfig.GetFloat("PLAYER_INCREASE_BAG_SCALE");
        this.PLAYER_DECREASE_BAG_SCALE = ConfigManager.appConfig.GetFloat("PLAYER_DECREASE_BAG_SCALE");
        this.PLAYER_MAX_BAG_SCALE = ConfigManager.appConfig.GetFloat("PLAYER_MAX_BAG_SCALE");
        // 通常状態
        this.LIMIT_INERTIA_X = ConfigManager.appConfig.GetFloat("LIMIT_INERTIA_X");
        this.LIMIT_PLAYER_X_SPEED = ConfigManager.appConfig.GetFloat("LIMIT_PLAYER_X_SPEED");
        this.PLAYER_DECREASE_INRTIA_X_RATE = ConfigManager.appConfig.GetFloat("PLAYER_DECREASE_INRTIA_X_RATE");
        this.PLAYER_INRTIA_RATE = ConfigManager.appConfig.GetFloat("PLAYER_INRTIA_RATE");
        this.PLAYER_LIMIT_TILT = ConfigManager.appConfig.GetFloat("PLAYER_LIMIT_TILT");
        this.PLAYER_X_SPEED = ConfigManager.appConfig.GetFloat("PLAYER_X_SPEED");
        this.SPIN_WHEEL_PLAYER_X_SPEED_RATE = ConfigManager.appConfig.GetFloat("SPIN_WHEEL_PLAYER_X_SPEED_RATE");
        // 滑り状態
        this.SLIP_LIMIT_INERTIA_X = ConfigManager.appConfig.GetFloat("SLIP_LIMIT_INERTIA_X");
        this.SLIP_LIMIT_PLAYER_X_SPEED = ConfigManager.appConfig.GetFloat("SLIP_LIMIT_PLAYER_X_SPEED");
        this.SLIP_PLAYER_DECREASE_INRTIA_X_RATE = ConfigManager.appConfig.GetFloat("SLIP_PLAYER_DECREASE_INRTIA_X_RATE");
        this.SLIP_PLAYER_INRTIA_RATE = ConfigManager.appConfig.GetFloat("SLIP_PLAYER_INRTIA_RATE");
        this.SLIP_PLAYER_LIMIT_TILT = ConfigManager.appConfig.GetFloat("SLIP_PLAYER_LIMIT_TILT");
        this.SLIP_PLAYER_X_SPEED = ConfigManager.appConfig.GetFloat("SLIP_PLAYER_X_SPEED");
        this.SLIP_SPIN_WHEEL_PLAYER_X_SPEED_RATE = ConfigManager.appConfig.GetFloat("SLIP_SPIN_WHEEL_PLAYER_X_SPEED_RATE");
    }
}