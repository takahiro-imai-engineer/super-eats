using app_system;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviourSingleton<StageManager>, IGameRegistry
{
    /// <summary>ステージの順番データ</summary>
    [SerializeField] private StageOrderData stageOrderData;
    [SerializeField] private List<GraphicData> graphicDataList;
    [SerializeField] private List<RivalData> rivalDataList;
    //================================================================================
    // ゲームセッティング
    //================================================================================
    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting(GameSetting gameSetting)
    {

    }

    public StageData GetStageData(int level)
    {
        if (level > stageOrderData.StageOrderDataList.Count)
        {
            Debug.LogError($"ステージデータの取得に失敗. level={level}");
            level = stageOrderData.StageOrderDataList.Count;
        }
        var stageName = stageOrderData.StageOrderDataList[level - 1];
        return AssetManager.Instance.LoadStageData(stageName);
    }

    public bool IsClearAllStage(int level)
    {
        return level > stageOrderData.StageOrderDataList.Count;
    }

    public GraphicData GetGraphicData(int graphicId)
    {
        if (graphicId <= 0 || graphicId > graphicDataList.Count)
        {
            Debug.LogError($"グラフィックデータの取得に失敗. graphicId={graphicId}");
            return null;
        }
        return graphicDataList[graphicId - 1];
    }

    public RivalData GetRandomRivalData()
    {
        return GameUtility.GetRandom(rivalDataList);
    }

    /// <summary>
    /// シャットダウン
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    public void Shutdown()
    {

    }
}
