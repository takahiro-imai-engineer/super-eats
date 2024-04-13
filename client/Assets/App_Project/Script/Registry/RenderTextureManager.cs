using app_system;
using UnityEngine;

public class RenderTextureManager : MonoBehaviourSingleton<RenderTextureManager>, IGameRegistry
{
    /// <summary>レンダーテクスチャー</summary>
    [SerializeField] private RenderTexture renderTexture;
    /// <summary>失敗用絵文字エフェクト</summary>
    [SerializeField] private GameObject failedEmojiRoot;

    public Texture RenderTexture => renderTexture;

    //================================================================================
    // ゲームセッティング
    //================================================================================
    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting(GameSetting gameSetting)
    {
        Hide();
    }

    public Texture GetFailedEmoji()
    {
        failedEmojiRoot.SetActive(true);
        return renderTexture;
    }

    public void Hide()
    {
        renderTexture.Release();
        failedEmojiRoot.SetActive(false);
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
