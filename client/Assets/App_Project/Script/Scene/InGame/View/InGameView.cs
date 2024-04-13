using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using template;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// インゲーム描画クラス
/// </summary>
public class InGameView : MonoBehaviour
{
    //================================================================================
    // 3D関係インスペクタ
    //================================================================================
    [SerializeField] private GameObject characterRoot;
    [SerializeField] private GameObject stageRoot;
    //================================================================================
    // 3D関係プレハブ
    //================================================================================
    //================================================================================
    // UI関係インスペクタ
    //================================================================================
    // UI View
    [SerializeField] private InGameUiView inGameUiView;
    //================================================================================
    // ローカル
    //================================================================================
    //================================================================================
    // getter
    //================================================================================
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(InGameModel inGameModel, UnityAction<InGameConstant.ButtonType> clickListener)
    {
        inGameUiView.Init(inGameModel, clickListener);
    }
    //================================================================================
    // 準備画面
    //================================================================================
    /// <summary>
    /// 準備画面を表示
    /// </summary>
    public void ShowPrepare()
    {
        inGameUiView.ShowPrepare();
    }

    /// <summary>
    /// 準備画面を非表示
    /// </summary>
    public void HidePrepare()
    {
        inGameUiView.HidePrepare();
    }

    public void RetryGame()
    {
        inGameUiView.StartGame();
    }

    //================================================================================
    // ゲーム中
    //================================================================================
    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void StartGame()
    {
        inGameUiView.StartGame();
    }
    /// <summary>
    /// ゲーム中の更新
    /// </summary>
    public void UpdatePlay(InGameModel inGameModel)
    {
        inGameUiView.UpdatePlay(inGameModel);
    }

    /// <summary>
    /// ゲーム中画面を非表示
    /// </summary>
    public void HidePlay(InGameModel inGameModel)
    {
        inGameUiView.HidePlay(inGameModel);
    }

    /// <summary>
    /// 突風情報を表示
    /// </summary>
    /// <param name="pushVector"></param>
    /// <param name="warningTime"></param>
    public void ShowWindInfo(Vector3 pushVector, float warningTime)
    {
        inGameUiView.ShowWindInfo(pushVector, warningTime);
    }

    /// <summary>
    /// オーダー状況更新
    /// </summary>
    /// <param name="inGameModel"></param>
    public void UpdateOrderStatus(InGameModel inGameModel)
    {
        inGameUiView.UpdateOrderStatus(inGameModel);
    }
    //================================================================================
    // 結果画面
    //================================================================================
    /// <summary>
    /// 結果画面を表示
    /// </summary>
    public void ShowResult(InGameModel inGameModel, InGameConstant.ResultStatus resultStatus)
    {
        inGameUiView.ShowResult(inGameModel, resultStatus);
    }
}