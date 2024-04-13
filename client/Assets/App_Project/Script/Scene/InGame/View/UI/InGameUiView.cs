using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// インゲームUI描画クラス
/// </summary>
public class InGameUiView : MonoBehaviour
{
    //================================================================================
    // UI関係インスペクタ
    //================================================================================
    [Header("準備画面")]
    [SerializeField] private GameObject prepareRoot;
    [Header("プレイ中画面")]
    [SerializeField] private GameObject playRoot;
    [SerializeField] private GameObject lifeRoot;
    [SerializeField] private List<LifeIconView> lifeIconList;
    [SerializeField] private GameObject scoreRoot;
    [SerializeField] private Image totalMoveDistanceGauge;
    [SerializeField] private WindInfoView windInfoView;
    [SerializeField] private OrderFoodListView orderFoodListView;

    // [SerializeField] private TextMeshProUGUI totalMoveDistanceText;

    [Header("結果画面")]
    [SerializeField] private GameObject resultRoot;
    [SerializeField] private ResultOrderView resultOrderViewPrefab;
    [SerializeField] private ResultFailedView resultFailedViewPrefab;

    //================================================================================
    // デバッグ用インスペクタ
    //================================================================================
    [Header("デバッグ情報")]
    [SerializeField] private DebugButton operationDebugButton;
    [SerializeField] private InputField inrtiaInputField;

    [SerializeField] private Text spinWheelText;
    [SerializeField] private Text inertiaXText;

    //================================================================================
    // ローカル
    //================================================================================
    private InGameConstant.InGameStatus resultStatus;
    private UnityAction<InGameConstant.ButtonType> clickHandler;

    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(InGameModel inGameModel, UnityAction<InGameConstant.ButtonType> clickListener)
    {
        resultStatus = InGameConstant.InGameStatus.None;
        clickHandler = clickListener;

        prepareRoot.SetActive(false);
        playRoot.SetActive(false);
        windInfoView.Init();
        orderFoodListView.Init(inGameModel.OrderFoodList);
        resultRoot.SetActive(false);

        for (int i = 0; i < lifeIconList.Count; i++)
        {
            lifeIconList[i].gameObject.SetActive(i + 1 <= inGameModel.PlayerInfo.CurrentLife);
        }
        totalMoveDistanceGauge.fillAmount = 0f;
#if false 
        // デバッグ用
        var inGameVariant = GameVariant.Instance.Get<InGameVariant>();
        if (inGameVariant.IsDragOperation)
        {
            operationDebugButton.UpdateText("ドラッグ操作");
        }
        else
        {
            operationDebugButton.UpdateText("左右タッチ操作");
        }
        inrtiaInputField.placeholder.GetComponent<Text>().text = "現在の滑り値: " + inGameVariant.DecreaseInrtiaXRate;
        spinWheelText.text = $"空回り中か：{inGameModel.PlayerInfo.View.IsSpinWheel}";
        inertiaXText.text = $"現在の慣性量：{inGameModel.PlayerInfo.BeforeInertiaX}";
#endif
    }

    //================================================================================
    // 準備画面
    //================================================================================
    /// <summary>
    /// 準備画面を表示
    /// </summary>
    public void ShowPrepare()
    {
        prepareRoot.SetActive(true);
    }

    /// <summary>
    /// 準備画面を非表示
    /// </summary>
    public void HidePrepare()
    {
        prepareRoot.SetActive(false);
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void StartGame()
    {
        orderFoodListView.Show();
    }

    //================================================================================
    // ゲーム中
    //================================================================================
    /// <summary>
    /// ゲーム中の更新
    /// </summary>
    public void UpdatePlay(InGameModel inGameModel)
    {

        playRoot.SetActive(true);
        Vector3 playerPosition = inGameModel.PlayerInfo.Position;
        // NOTE: ライフ表示
        for (int i = 0; i < lifeIconList.Count; i++)
        {
            lifeIconList[i].Init();
            if (i + 1 <= inGameModel.PlayerInfo.CurrentLife)
            {
                lifeIconList[i].Show();
            }
            else
            {
                lifeIconList[i].Hide();
            }
        }
        totalMoveDistanceGauge.fillAmount = playerPosition.z / inGameModel.SelectStageData.StageDistance;

        // NOTE: デバッグ
        spinWheelText.text = $"空回り中か：{inGameModel.PlayerInfo.View.IsSpinWheel}";
        inertiaXText.text = $"現在の慣性量：{inGameModel.PlayerInfo.BeforeInertiaX}";
    }

    /// <summary>
    /// ゲーム中画面を非表示
    /// </summary>
    public void HidePlay(InGameModel inGameModel)
    {
        playRoot.SetActive(false);
        orderFoodListView.Hide();
    }

    /// <summary>
    /// 突風情報を表示
    /// </summary>
    /// <param name="pushVector"></param>
    /// <param name="warningTime"></param>
    public void ShowWindInfo(Vector3 pushVector, float warningTime)
    {
        windInfoView.Show(pushVector);
        DOVirtual.DelayedCall(
            warningTime,
            () =>
            {
                windInfoView.Hide();
            }
        );
    }

    /// <summary>
    /// オーダー状況更新
    /// </summary>
    /// <param name="inGameModel"></param>
    public void UpdateOrderStatus(InGameModel inGameModel)
    {
        orderFoodListView.UpdateOrderFood(inGameModel.OrderFoodList);
    }
    //================================================================================
    // 結果画面
    //================================================================================
    /// <summary>
    /// 結果画面を表示
    /// </summary>
    public void ShowResult(InGameModel inGameModel, InGameConstant.ResultStatus resultStatus)
    {
        playRoot.SetActive(false);
        resultRoot.SetActive(true);
        if (resultStatus == InGameConstant.ResultStatus.SuccessResult || resultStatus == InGameConstant.ResultStatus.FailedResult)
        {
            ResultOrderView.Show(resultOrderViewPrefab, inGameModel, (buttonType) =>
            {
                clickHandler.Invoke(buttonType);
                DialogManager.Instance.Dismiss(DialogManager.Instance.Get<ResultOrderView>(), null);
            });
        }
        else if (resultStatus == InGameConstant.ResultStatus.Continue)
        {
            ResultFailedView.Show(resultFailedViewPrefab, (buttonType) =>
            {
                clickHandler.Invoke(buttonType);
                DialogManager.Instance.Dismiss(DialogManager.Instance.Get<ResultFailedView>(), null);
            });
        }
    }

    //================================================================================
    // UIイベント
    //================================================================================
    /// <summary>
    /// ゲーム開始ボタンクリック
    /// </summary>
    public void OnClickStartButton()
    {
        if (clickHandler != null)
        {
            clickHandler(InGameConstant.ButtonType.Start);
        }
    }

    /// <summary>
    /// 設定ボタンクリック
    /// </summary>
    public void OnClickSettingButton()
    {
        if (clickHandler != null)
        {
            clickHandler(InGameConstant.ButtonType.Setting);
        }
    }

    public void OnClickTitleButton()
    {
        if (clickHandler != null)
        {
            clickHandler(InGameConstant.ButtonType.Title);
        }
    }

    /// <summary>
    /// リトライボタンクリック
    /// </summary>
    public void OnClickRetryButton()
    {
        if (clickHandler != null)
        {
            clickHandler(InGameConstant.ButtonType.Retry);
        }
    }

    /// <summary>
    /// 結果ボタンクリック
    /// </summary>
    public void OnClickResultButton()
    {
        if (clickHandler != null)
        {
            if (resultStatus == InGameConstant.InGameStatus.Success)
            {
                clickHandler(InGameConstant.ButtonType.NextGame);
            }
            else
            {
                clickHandler(InGameConstant.ButtonType.Retry);
            }
        }
    }

    #region 
    //================================================================================
    // デバッグ用
    //================================================================================
    /// <summary>
    /// 操作切り替えデバッグボタン
    /// </summary>
    public void OnClickDebugSwitchOperation()
    {
        var inGameVariant = GameVariant.Instance.Get<InGameVariant>();
        inGameVariant.IsDragOperation = !inGameVariant.IsDragOperation;
        if (inGameVariant.IsDragOperation)
        {
            operationDebugButton.UpdateText("ドラッグ操作");
        }
        else
        {
            operationDebugButton.UpdateText("左右タッチ操作");
        }
    }
    /// <summary>
    /// 滑り値デバッグボタン
    /// </summary>
    public void OnClickDebugInrtia()
    {
        string inputValue = inrtiaInputField.text;
        var inGameVariant = GameVariant.Instance.Get<InGameVariant>();
        float value = 0f;
        bool result = float.TryParse(inputValue, out value);
        if (!result)
        {
            Debug.LogError("型変換に失敗: " + inputValue);
            return;
        }
        inGameVariant.DecreaseInrtiaXRate = value;
        inrtiaInputField.placeholder.GetComponent<Text>().text = "現在の滑り値: " + value;
        Debug.Log(inGameVariant.DecreaseInrtiaXRate);
    }
    #endregion
}