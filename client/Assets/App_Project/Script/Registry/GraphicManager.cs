using app_system;
using UnityEngine;

public class GraphicManager : MonoBehaviourSingleton<GraphicManager>, IGameRegistry
{

    //================================================================================
    // ゲームセッティング
    //================================================================================
    private void Awake()
    {
        Application.lowMemory += OnLowMemory;
    }

    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting(GameSetting gameSetting)
    {

    }

    public void SetGraphic(int graphicLevel)
    {
        Debug.Log($"グラフィックレベル変更: {graphicLevel}");
        QualitySettings.SetQualityLevel(graphicLevel);
    }

    public void SetAutoGraphic()
    {
        // NOTE: 
        if (SystemInfo.systemMemorySize < 2200)
        {
            SetGraphic(0);
        }
        else
        {
            SetGraphic(1);
        }
    }

    private void OnLowMemory()
    {
        Debug.LogError("メモリ不足");
        // Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// シャットダウン
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    public void Shutdown()
    {

    }
}
