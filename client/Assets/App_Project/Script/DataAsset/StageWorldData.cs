using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステージグループデータ
/// </summary>
//[CreateAssetMenu]
public class StageWorldData : ScriptableObject
{

    [Header("ステージグループID")]
    [SerializeField] private int worldId;

    [Header("次のワールドに進行するのに必要な累計獲得コイン数")]
    [SerializeField] private int nextWorldNecessaryTotalMoneyNum = 10000;

    [Header("全クリア時に繰り返す進行度(0~)")]
    [SerializeField] private int reapeatStageProgressDegrees = 0;

    [Header("ステージ抽選データリスト")]
    [SerializeField] private List<StageDrawingData> stageDrawingDataList;

    [Header("グラフィック関連データ")]
    [SerializeField] private GraphicData graphicData;

    [Header("ステージグループ名")]
    [SerializeField] private string stageGroupName;

    [Header("ステージアイコン")]
    [SerializeField] private Sprite stageIcon;

    /// <summary>ステージワールドID</summary>
    public int WorldId => worldId;

    /// <summary>次のワールドに進行するのに必要な累計獲得コイン数</summary>
    public int NextWorldNecessaryTotalMoneyNum => nextWorldNecessaryTotalMoneyNum;

    /// <summary>全クリア時に繰り返す進行度(0~)</summary>
    public int ReapeatStageProgressDegrees => reapeatStageProgressDegrees;

    /// <summary>ステージ抽選データリスト</summary>
    public List<StageDrawingData> StageDrawingDataList => stageDrawingDataList;

    /// <summary>グラフィック関連データ</summary>
    public GraphicData GraphicData => graphicData;

    /// <summary>ステージグループ名</summary>
    public string StageGroupName => stageGroupName;

    /// <summary>ステージアイコン</summary>
    public Sprite StageIcon => stageIcon;
}


/// <summary>
/// ステージ抽選テーブルデータ
/// </summary>
[System.Serializable]
public class StageDrawingData
{
    [Header("進行度")]
    public int ProgressDegrees;
    [Header("スロットデータ")]
    public List<StageSlotData> StageSlotDataList;
    [Header("チュートリアルステージか")]
    public bool IsTutorialStage = false;
}

/// <summary>
/// ステージデータリスト
/// </summary>
[System.Serializable]
public class StageSlotData
{
    public List<string> StageDataList;
}