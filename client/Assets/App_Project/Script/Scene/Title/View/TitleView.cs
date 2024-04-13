using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TitleView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private TitleStageView titleStageView;
    [SerializeField] private ChestView chestView;
    [SerializeField] private TitleUiView titleUiView;
    [SerializeField] private GameObject gotBaseEffect;
    //================================================================================
    // ローカル
    //================================================================================
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="clickListener"></param>
    public void Init(
        UnityAction<TitleConstant.ButtonType, int> clickListener,
        UnityAction<TitleConstant.ShopItemType, int> onPurchaseCallback
    )
    {
        titleStageView.Init();
        chestView.Init();
        titleUiView.Init(clickListener, onPurchaseCallback);
        gotBaseEffect.SetActive(false);
    }

    /// <summary>
    /// 表示
    /// </summary>
    public void Show(StageData selectStageData, List<BaseRewardData.BaseLevelUpReward> currentRewardList, bool isLevelUpBase, UnityAction onCompleteCallback)
    {
        int getMoneyNum = GameVariant.Instance.Get<InGameVariant>().GetMoneyNum;

        titleStageView.Show();
        titleUiView.SetStageData(selectStageData);

        int totalMoneyNum = UserDataProvider.Instance.GetSaveData().TotalMoneyNum;
        if (GameVariant.Instance.Get<InGameVariant>().IsWatchedAd)
        {
            chestView.Show(() =>
            {
                titleUiView.Show(currentRewardList, getMoneyNum, () =>
                {
                    // 拠点獲得演出
                    if (!isLevelUpBase)
                    {
                        ShowOrder();
                    }
                    onCompleteCallback.Invoke();
                });
                GameVariant.Instance.Get<InGameVariant>().IsWatchedAd = false;
            });
        }
        else
        {
            titleUiView.Show(currentRewardList, getMoneyNum, () =>
            {
                ShowOrder();
                onCompleteCallback.Invoke();
            });
        }
    }

    public void ShowOrder()
    {
        titleUiView.ShowOrder();
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide()
    {
        titleUiView.Hide();
    }

    /// <summary>
    /// 表示
    /// </summary>
    public void ShowWarpNextStage(int stageLevel, int totalMoneyNum, UnityAction onCompleteCallback)
    {
        int getMoneyNum = GameVariant.Instance.Get<InGameVariant>().GetMoneyNum;

        titleUiView.SetStageData(null);
        if (GameVariant.Instance.Get<InGameVariant>().IsWatchedAd)
        {
            chestView.Show(() =>
            {
                titleUiView.Show(null, getMoneyNum, onCompleteCallback);
                GameVariant.Instance.Get<InGameVariant>().IsWatchedAd = false;
            });
        }
        else
        {
            titleUiView.Show(null, getMoneyNum, onCompleteCallback);
        }
    }

    public void HideButtonAndOrder()
    {
        titleUiView.HideButtonAndOrder();
    }

    public void ShowMaxProgressEffect()
    {
        titleUiView.ShowMaxProgressEffect();
    }

    public void ShowWarpButton()
    {
        titleUiView.ShowWarpButton();
    }

    public void ShowShop()
    {
        titleUiView.ShowShop();
    }

    public void HideShop()
    {
        titleUiView.HideShop();
    }

    public void UpdateShop()
    {
        titleUiView.UpdateShop();
    }

    public void UpdateCoin(int useCoinNum)
    {
        titleUiView.UpdateCoin(useCoinNum);
    }

    public void ShowBaseEarnStaging(BaseRewardData.BaseLevelUpReward clearReward, UnityAction onCompleteCallback)
    {
        titleUiView.ShowBaseEarnStaging(clearReward, onCompleteCallback);
    }

    public void ShowGetReward(BaseRewardData.BaseLevelUpReward clearReward, UnityAction onCompleteCallback)
    {
        gotBaseEffect.SetActive(clearReward.BaseStep == 0);
        titleUiView.ShowGetReward(clearReward, onCompleteCallback);
    }

    public void UpdateBaseProgress(List<BaseRewardData.BaseLevelUpReward> currentRewardList)
    {
        titleUiView.UpdateBaseProgress(currentRewardList);
    }

    public void LevelUpBase(int baseLevel, UnityAction onChangeBase)
    {
        titleStageView.LevelUpBase(baseLevel, onChangeBase);
    }

    public void UpdatePurchaseTutorial(int tutorialId)
    {
        titleUiView.UpdatePurchaseTutorial(tutorialId);
    }

    /// <summary>
    /// 設定画面を表示
    /// </summary>
    public void ShowSettingWindow()
    {
        titleUiView.ShowSettingWindow();
    }

    /// <summary>
    /// 未実装テキストを表示
    /// </summary>
    public void ShowUnimplementedText()
    {
        titleUiView.ShowUnimplementedText();
    }
}