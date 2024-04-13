using app_system;
using template;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResultFailedView : GeneralDialog
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject okButtonRoot;
    //================================================================================
    // ローカル
    //================================================================================
    private UnityAction<InGameConstant.ButtonType> clickHandler;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 表示.
    /// </summary>
    public static ResultFailedView Show(ResultFailedView dialogPrefab, UnityAction<InGameConstant.ButtonType> onClickButtonListener)
    {
        var dialog = DialogManager.Instance.Show<ResultFailedView>(
            dialogPrefab,
            string.Empty,
            string.Empty,
            (buttonType) => { },
            DialogManager.ButtonType.YES, DialogManager.ButtonType.NO
        );
        dialog.SetData(onClickButtonListener);
        return dialog;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void SetData(UnityAction<InGameConstant.ButtonType> clickListener)
    {
        clickHandler = clickListener;
        root.SetActive(true);
        okButtonRoot.SetActive(true);
    }

    //================================================================================
    // UIイベント
    //================================================================================
    /// <summary>
    /// ボーナスボタンクリック
    /// </summary>
    public void OnClickKeepBonusButton()
    {
        if (clickHandler != null)
        {
            clickHandler(InGameConstant.ButtonType.Continue);
        }
    }

    /// <summary>
    /// OKボタンクリック
    /// </summary>
    public void OnClickOKButton()
    {
        if (clickHandler != null)
        {
            clickHandler(InGameConstant.ButtonType.Retry);
        }
    }
}
