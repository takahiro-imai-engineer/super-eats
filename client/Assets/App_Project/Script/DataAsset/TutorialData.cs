using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// チュートリアル用データ
/// </summary>
public class TutorialData : ScriptableObject
{
    [Header("チュートリアルID")]
    [SerializeField] private int tutorialId = 0;
    [Header("メモ用/チュートリアル名")]
    [SerializeField] private string tutorialName;
    [Header("チュートリアルタイプ")]
    [SerializeField] private InGameConstant.TutorialType tutorialType;
    [Header("操作タイプ")]
    [SerializeField] private InGameConstant.TutorialOperationType tutorialOperationType;
    [Header("ゲーム再生速度")]
    [SerializeField] private float timeScale = 0.1f;
    [Header("画像表示時間(0指定なら、閉じるボタンを押すまで)")]
    [SerializeField] private float showDuration = 5f;
    [Header("チュートリアル画像")]
    [SerializeField] private string tutorialSpriteName = null;

    /// <summary>
    /// チュートリアルID
    /// </summary>
    public int TutorialId => tutorialId;

    /// <summary>
    /// チュートリアルタイプ
    /// </summary>
    public InGameConstant.TutorialType TutorialType => tutorialType;

    /// <summary>
    /// 操作タイプ
    /// </summary>
    public InGameConstant.TutorialOperationType TutorialOperationType => tutorialOperationType;

    /// <summary>
    /// ゲーム再生時間
    /// </summary>
    public float TimeScale => timeScale;

    /// <summary>
    /// 画像表示時間
    /// </summary>
    public float ShowDuration => showDuration;

    /// <summary>
    /// 画像
    /// </summary>
    public string TutorialSpriteName => tutorialSpriteName;
}