using DG.Tweening;
using app_system;
using template;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialWindow03 : GeneralDialog
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject howToPlayRoot;
    [SerializeField] private Image howToPlayTutorialImage;
    [SerializeField] private GameObject shopTutorialRoot;
    [SerializeField] private Image shopTutorialImage;
    //================================================================================
    // ローカル
    //================================================================================
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 表示.
    /// </summary>
    public static TutorialWindow03 Show(TutorialWindow03 dialogPrefab, TutorialData tutorialData, UnityAction onCompleteCallback)
    {
        var dialog = DialogManager.Instance.Show<TutorialWindow03>(
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
        const int HOW_TO_PLAY_TUTORIAL_ID = 100;
        if (tutorialData.TutorialId == HOW_TO_PLAY_TUTORIAL_ID)
        {
            howToPlayRoot.SetActive(true);
            shopTutorialRoot.SetActive(false);
            howToPlayTutorialImage.sprite = AssetManager.Instance.LoadTutorialImageSprite(tutorialData.TutorialSpriteName); ;
        }
        else
        {
            howToPlayRoot.SetActive(false);
            shopTutorialRoot.SetActive(true);
            shopTutorialImage.sprite = AssetManager.Instance.LoadTutorialImageSprite(tutorialData.TutorialSpriteName); ;
        }
    }
}
