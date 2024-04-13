using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// インゲームシーン・ゲーム変数
/// </summary>
public class InGameVariant : UserVariant
{
    /// <summary>基準とする画面サイズ</summary>
    public Vector2 BaseScreenSize;

    /// <summary>ドラッグ操作</summary>
    public bool IsDragOperation = true;

    /// <summary>徐々に減っていくX軸の慣性の倍率</summary>
    public float DecreaseInrtiaXRate = 1f;

    /// <summary>選択中のステージデータ</summary>
    public bool IsDebugStage = false;

    /// <summary>選択中コンテンツID</summary>
    public int SelectContentId = 1;

    /// <summary>獲得したコイン数</summary>
    public int GetMoneyNum = 0;

    /// <summary>獲得したジュエル数</summary>
    public int GetJewelNum = 0;

    /// <summary>ステージをクリアしたか</summary>
    public bool IsClearStage = false;

    /// <summary>広告を視聴したか</summary>
    public bool IsWatchedAd = false;

    /// <summary>ワープが終了したか</summary>
    public bool IsEndWarpStage = false;

    /// <summary>エンディング開始するか</summary>
    public bool IsShowEnding = false;

    /// <summary>広告を表示するか</summary>
    public bool IsShowAds = true;

    /// <summary>選択中のステージデータ</summary>
    public StageData SelectStageData = null;
    /// <summary>選択中のグラフィックデータ</summary>
    public GraphicData SelectGraphicData = null;

    /// <summary>インゲームモデル</summary>
    public InGameModel InGameModel;

    /// <summary>
    /// インゲームステータス
    /// </summary>
    /// <returns></returns>
    public InGameConstant.InGameStatus GetGameStatus()
    {
        return InGameModel.InGameStatus;
    }
}