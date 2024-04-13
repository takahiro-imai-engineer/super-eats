using DG.Tweening;
using app_system;
using template;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialWindow01 : GeneralDialog
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private Image tutorialImage;
    //================================================================================
    // ローカル
    //================================================================================
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 表示.
    /// </summary>
    public static TutorialWindow01 Show(TutorialWindow01 dialogPrefab, TutorialData tutorialData, UnityAction onCompleteCallback)
    {
        var dialog = DialogManager.Instance.Show<TutorialWindow01>(
            dialogPrefab,
            string.Empty,
            string.Empty,
            (buttonType) => { onCompleteCallback?.Invoke(); },
            DialogManager.ButtonType.YES, DialogManager.ButtonType.NO
        );
        dialog.SetData(tutorialData);
        return dialog;
    }

    public void SetData(TutorialData tutorialData)
    {
        tutorialImage.sprite = AssetManager.Instance.LoadTutorialImageSprite(tutorialData.TutorialSpriteName);
    }
}
