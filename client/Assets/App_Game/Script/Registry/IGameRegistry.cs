/// <summary>
/// ゲームレジストリのインターフェイス
/// </summary>
public interface IGameRegistry {

    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    void SetupGameSetting( GameSetting gameSetting );

    /// <summary>
    /// シャットダウン
    /// 
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    void Shutdown();
}
