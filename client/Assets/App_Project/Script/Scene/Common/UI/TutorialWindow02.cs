using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class TutorialWindow02 : GeneralDialog
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [Header("操作説明用")]
    [SerializeField] private GameObject moveTutorialRoot;
    [SerializeField] private RectTransform moveAnimationFingerIconTransform;
    /// <summary>パスルート</summary>
    [SerializeField] private GameObject moveAnimationPathRoot;
    /// <summary>パスルート</summary>
    [Header("パスの補完した時の動き方。曲線(CatmullRom)/直線(Linear)")]
    [SerializeField] private PathType moveAnimationPathType;
    [Header("時間経過による動き方")]
    [SerializeField] private Ease moveAnimationEase = Ease.Linear;
    [Header("移動時間")]
    [SerializeField] private float moveAnimationDurationTime = 1f;

    [Header("加重説明用")]
    [SerializeField] private GameObject heavyTutorialRoot;
    [SerializeField] private RectTransform heavyAnimationFingerIconTransform;
    /// <summary>パスルート</summary>
    [SerializeField] private GameObject heavyAnimationPathRoot;
    /// <summary>パスルート</summary>
    [Header("パスの補完した時の動き方。曲線(CatmullRom)/直線(Linear)")]
    [SerializeField] private PathType heavyAnimationPathType;
    [Header("時間経過による動き方")]
    [SerializeField] private Ease heavyAnimationEase = Ease.Linear;
    [Header("移動時間")]
    [SerializeField] private float heavyAnimationDurationTime = 1f;
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

        if (tutorialData.TutorialOperationType == InGameConstant.TutorialOperationType.Move)
        {
            moveTutorialRoot.SetActive(true);
            heavyTutorialRoot.SetActive(false);
            var pathList = moveAnimationPathRoot.GetComponentsInChildren<RectTransform>().Where(c => moveAnimationPathRoot != c.gameObject).ToList();
            var movePathList = new Vector3[pathList.Count];
            for (int i = 0; i < pathList.Count; i++)
            {
                movePathList[i] = pathList[i].position;
            }
            moveAnimationFingerIconTransform
                .DOPath(movePathList, moveAnimationDurationTime, moveAnimationPathType)
                .SetUpdate(true)
                .SetEase(moveAnimationEase)
                .SetLoops(-1)
                .SetLink(this.gameObject);
        }
        else if (tutorialData.TutorialOperationType == InGameConstant.TutorialOperationType.Heavy)
        {
            moveTutorialRoot.SetActive(false);
            heavyTutorialRoot.SetActive(true);
            var pathList = heavyAnimationPathRoot.GetComponentsInChildren<RectTransform>().Where(c => heavyAnimationPathRoot != c.gameObject).ToList();
            var movePathList = new Vector3[pathList.Count];
            for (int i = 0; i < pathList.Count; i++)
            {
                movePathList[i] = pathList[i].position;
            }
            heavyAnimationFingerIconTransform
                .DOPath(movePathList, heavyAnimationDurationTime, heavyAnimationPathType)
                .SetUpdate(true)
                .SetEase(heavyAnimationEase)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(this.gameObject);
        }
        else
        {
            moveTutorialRoot.SetActive(false);
            heavyTutorialRoot.SetActive(false);
            Debug.LogWarning($"未実装の操作チュートリアル: {tutorialData.TutorialOperationType}");
        }
    }
}
