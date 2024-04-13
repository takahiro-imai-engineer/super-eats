using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using app_system;
using template;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResultOrderView : GeneralDialog
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject orderWindowRoot;
    [SerializeField] private List<ResultFootIconView> resultFoodIconViewList;
    [SerializeField] private GameObject moneyRoot;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject avatarBonusRoot;
    [SerializeField] private Image avatarIcon;
    [SerializeField] private TextMeshProUGUI avatarBonusText;
    [SerializeField] private GameObject rewardRoot;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private RectTransform adButtonRoot;
    [SerializeField] private List<GameObject> adCharacterImages;
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
    public static ResultOrderView Show(ResultOrderView dialogPrefab, InGameModel inGameModel, UnityAction<InGameConstant.ButtonType> onClickButtonListener)
    {
        var dialog = DialogManager.Instance.Show<ResultOrderView>(
            dialogPrefab,
            string.Empty,
            string.Empty,
            (buttonType) => { },
            DialogManager.ButtonType.YES, DialogManager.ButtonType.NO
        );
        dialog.SetData(inGameModel, onClickButtonListener);
        return dialog;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void SetData(InGameModel inGameModel, UnityAction<InGameConstant.ButtonType> clickListener)
    {
        clickHandler = clickListener;
        root.SetActive(false);
        orderWindowRoot.SetActive(false);
        foreach (var item in resultFoodIconViewList)
        {
            item.Hide();
        }
        moneyRoot.SetActive(false);
        moneyText.text = "0$";
        avatarBonusRoot.SetActive(false);
        avatarBonusText.text = "0$";
        rewardRoot.SetActive(false);
        rewardText.text = $"0$";
        okButtonRoot.SetActive(false);

        Show(inGameModel);
    }

    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="inGameModel"></param>
    public void Show(InGameModel inGameModel)
    {
        root.SetActive(true);

        ShowWindow(inGameModel);
    }

    /// <summary>
    /// 通常メニュー表示
    /// </summary>
    /// <param name="inGameModel"></param>
    private void ShowWindow(InGameModel inGameModel)
    {
        bool isFailed = inGameModel.InGameResult == InGameConstant.InGameStatus.Failed;
        for (int i = 0; i < inGameModel.OrderFoodList.Count; i++)
        {
            if (i > resultFoodIconViewList.Count - 1)
            {
                Debug.LogWarning($"結果画面の表示数を超えています。{i + 1}個。(上限数: {resultFoodIconViewList.Count})");
                continue;
            }
            resultFoodIconViewList[i].Init(inGameModel.OrderFoodList[i].FoodData);
            resultFoodIconViewList[i].SetNum(inGameModel.OrderFoodList[i].NecessaryNum);
            resultFoodIconViewList[i].SetPrice(inGameModel.OrderFoodList[i].GetPrice() * inGameModel.OrderFoodList[i].PossessNum);
            resultFoodIconViewList[i].SetPossess(inGameModel.OrderFoodList[i].PossessNum > 0);
            resultFoodIconViewList[i].Show();
            if (isFailed) resultFoodIconViewList[i].ShowFailedLabel();
        }
        int resultStarNum = inGameModel.GetResultStarNum();
        moneyText.text = $"{inGameModel.GetPickedCoinWithDifficultyBonus()}$";
        var avatarData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Avatar, UserDataProvider.Instance.GetSaveData().SelectAvatarId) as AvatarData;
        avatarIcon.sprite = AssetManager.Instance.LoadShopIconSprite(avatarData.iconName);
        avatarBonusText.text = $"{avatarData.bonusCoin}$";
        rewardText.text = $"{inGameModel.GetTotalRewardCoin()}$";

        // 広告ボタンのアニメーション
        float difficulty = inGameModel.SelectStageData.Difficulty;
        adCharacterImages[0].SetActive(false);
        adCharacterImages[1].SetActive(false);
        if (difficulty == 5 || inGameModel.SelectStageData.IsBonusStage)
        {
            // 難易度5もしくはボーナスステージなら、演出2段階目
            adButtonRoot.DOPunchScale(
                    new Vector3(0.4f, 0.4f),
                    0.3f,
                    1,
                    0.049f
                )
                .SetEase(Ease.OutQuad)
                .SetLoops(-1)
                .SetLink(gameObject);
            adCharacterImages[0].SetActive(true);
            adCharacterImages[1].SetActive(true);
        }
        else if (difficulty == 3 || difficulty == 4)
        {
            // 難易度3か難易度4なら、演出1段階目
            adButtonRoot.DOPunchScale(
                    new Vector3(0.3f, 0.3f),
                    0.5f,
                    1,
                    0.049f
                )
                .SetEase(Ease.OutQuad)
                .SetLoops(-1)
                .SetLink(gameObject);
            adCharacterImages[0].SetActive(true);
        }

        Sequence sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                orderWindowRoot.SetActive(true);
                moneyRoot.SetActive(true);
                avatarBonusRoot.SetActive(inGameModel.InGameResult == InGameConstant.InGameStatus.Success);
            })
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                // comboBonusView.gameObject.SetActive(true);
            })
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                rewardRoot.SetActive(true);
                SoundManager.Instance.Play<SEContainer>(SEName.SE_MONEY_RESULT);
            })
            .AppendInterval(0.5f)
            .OnComplete(() =>
            {
                okButtonRoot.SetActive(true);
            });
        sequence.Play();
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide()
    {
        root.SetActive(false);
    }

    //================================================================================
    // UIイベント
    //================================================================================
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
    /// ボーナスボタンクリック
    /// </summary>
    public void OnClickBonusButton()
    {
        if (clickHandler != null)
        {
            clickHandler(InGameConstant.ButtonType.Bonus);
        }
    }

    /// <summary>
    /// OKボタンクリック
    /// </summary>
    public void OnClickOKButton()
    {
        if (clickHandler != null)
        {
            clickHandler(InGameConstant.ButtonType.NextGame);
        }
    }
}