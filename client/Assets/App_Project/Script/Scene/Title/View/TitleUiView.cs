using DG.Tweening;
using System.Collections.Generic;
using template;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TitleUiView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject root;

    [Header("Common")]
    [SerializeField] private CanvasGroup commonRoot;
    [SerializeField] private Button coinButton;
    [SerializeField] private TextMeshProUGUI getMoneyText;

    [Header("Main")]
    [SerializeField] private CanvasGroup mainRoot;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button warpButton;
    [SerializeField] private SpriteAnimation warpButtonAnimation;
    [SerializeField] private TitleBaseProgressView titleBaseProgressView;
    [Header("拠点獲得演出関連")]
    [SerializeField] private GameObject levelUpBaseRoot;
    [SerializeField] private GameObject levelUpBaseBackgroundRoot;
    [SerializeField] private GameObject gotBaseTitleImage;
    [SerializeField] private GameObject gotFurnitureTitleImage;
    [SerializeField] private Image gotFurnitureImage;
    [Header("ボタン関連")]
    [SerializeField] private Image playButtonImage;
    [SerializeField] private Sprite[] playButtonSprites;
    [SerializeField] private TextMeshProUGUI stageLevelText;
    [SerializeField] private TextMeshProUGUI playButtonText;
    [SerializeField] private Color[] playButtonTextColors;
    [SerializeField] private TextMeshProUGUI unimplementedText;
    [SerializeField] private GameObject tutorialPointer;
    [Header("コイン演出用")]
    [SerializeField] private RectTransform generateCoinIconRoot;
    [SerializeField] private List<RectTransform> generateCoinSpawnPoints;
    [SerializeField] private CoinIconView coinIconPrefab;

    [Header("Shop")]
    [SerializeField] GameObject shopRoot;
    [SerializeField] ShopItemView bagItemView;
    [SerializeField] ShopItemView avatarItemView;
    [SerializeField] ShopItemView bicycleItemView;

    [Header("Debug")]
    [SerializeField] private DebugWindowView debugWindowViewPrefab;
    //================================================================================
    // ローカル
    //================================================================================
    private UnityAction<TitleConstant.ButtonType, int> clickHandler;
    private Tween unimplementedTween;

    //================================================================================
    // 定数
    //================================================================================
    private float STAGE_CLEAR_SE_DELAY = 0.3f;
    private float SHOW_GET_REWARD_DURATION = 2f;

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
        clickHandler = clickListener;
        mainRoot.alpha = 1f;
        mainRoot.interactable = true;
        root.SetActive(false);

        moneyText.text = $"{UserDataProvider.Instance.GetSaveData().MoneyNum / 1000f}K";
        unimplementedText.gameObject.SetActive(false);
        warpButton.gameObject.SetActive(false);
        warpButtonAnimation.Init();

        coinButton.gameObject.SetActive(true);
        playButtonImage.gameObject.SetActive(false);
        levelUpBaseRoot.SetActive(false);
        shopRoot.SetActive(false);
        getMoneyText.gameObject.SetActive(false);
        getMoneyText.text = string.Empty;
        tutorialPointer.SetActive(false);

        var bagData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Bag, UserDataProvider.Instance.GetNextShotContentId(TitleConstant.ShopItemType.Bag));
        bagItemView.Init(
            bagData,
            null,
            (int contentId) => onPurchaseCallback?.Invoke(TitleConstant.ShopItemType.Bag, contentId)
        );
        var avatarData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Avatar, UserDataProvider.Instance.GetNextShotContentId(TitleConstant.ShopItemType.Avatar));
        avatarItemView.Init(
            avatarData,
            null,
            (int contentId) => onPurchaseCallback?.Invoke(TitleConstant.ShopItemType.Avatar, contentId)
        );
        var bicycleData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Bicycle, UserDataProvider.Instance.GetNextShotContentId(TitleConstant.ShopItemType.Bicycle));
        bicycleItemView.Init(
            bicycleData,
            null,
            (int contentId) => onPurchaseCallback?.Invoke(TitleConstant.ShopItemType.Bicycle, contentId)
        );
    }

    /// <summary>
    /// ステージデータ設定
    /// </summary>
    public void SetStageData(StageData selectStageData)
    {
        var saveData = UserDataProvider.Instance.GetSaveData();
        playButtonImage.sprite = playButtonSprites[(int)selectStageData.Difficulty - 1];
        stageLevelText.text = $"Level {saveData.TotalStageLevel}";
        stageLevelText.outlineColor = playButtonTextColors[(int)selectStageData.Difficulty - 1];
        playButtonText.outlineColor = playButtonTextColors[(int)selectStageData.Difficulty - 1];
        playButtonText.text = selectStageData.Difficulty switch
        {
            4 => "Hard",
            5 => "<cspace=-7.5>Super Hard</cspace>",
            6 => "Bonus",
            _ => "Play"
        };
    }

    /// <summary>
    /// 表示
    /// </summary>
    public void Show(List<BaseRewardData.BaseLevelUpReward> currentRewardList, int getMoneyNum, UnityAction onCompleteCallback)
    {
        root.SetActive(true);
        var saveData = UserDataProvider.Instance.GetSaveData();
        titleBaseProgressView.Init(saveData.BaseLevel, saveData.TotalStageLevel - 1, currentRewardList, GameVariant.Instance.Get<InGameVariant>().IsClearStage);
        GenerateCoinIcon(getMoneyNum, () =>
        {
            ShowGetMoney(getMoneyNum, () =>
            {
                titleBaseProgressView.UpdateProgress(onCompleteCallback);
            });
        });
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide()
    {
        root.SetActive(false);
    }

    /// <summary>
    /// コイン獲得演出再生
    /// </summary>
    /// <param name="getMoneyNum"></param>
    public void GenerateCoinIcon(int getMoneyNum, UnityAction onCompleteCallback)
    {
        if (getMoneyNum == 0)
        {
            onCompleteCallback.Invoke();
            return;
        }
        bool isWatchedAd = GameVariant.Instance.Get<InGameVariant>().IsWatchedAd;
        const int MIN_GENERATE_COIN_ICON_NUM = 3;
        const int MAX_GENERATE_COIN_ICON_NUM = 15;
        const int GENERATE_COIN_ICON_NUM_RATE = 25;
        int generateCoinIconNum = Mathf.Clamp(getMoneyNum / GENERATE_COIN_ICON_NUM_RATE, MIN_GENERATE_COIN_ICON_NUM, MAX_GENERATE_COIN_ICON_NUM);
        int spawnPoint = isWatchedAd ? 2 : 1;
        Sequence sequence = DOTween.Sequence()
            .SetLink(gameObject)
            .OnStart(() =>
            {
                moneyText.gameObject.SetActive(false);
            });
        // コインの生成
        const float GENERATE_COIN_DEURATION = 0.5f;
        var coinInconViewList = new List<CoinIconView>();
        for (int i = 0; i < spawnPoint; i++)
        {
            var baseSpawnPoint = isWatchedAd ? generateCoinSpawnPoints[i].anchoredPosition : generateCoinIconRoot.anchoredPosition;
            for (int j = 0; j < generateCoinIconNum; j++)
            {
                float generateDelayDuration = Random.Range(0f, GENERATE_COIN_DEURATION);
                sequence.InsertCallback(generateDelayDuration, () =>
                {
                    var coinIcon = Instantiate(coinIconPrefab, generateCoinIconRoot);
                    coinIcon.transform.localScale = Vector3.zero;
                    coinIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        baseSpawnPoint.x + Random.Range(-100f, 100f),
                        baseSpawnPoint.y + Random.Range(-100f, 100f)
                    );
                    coinIcon.transform.DOScale(Vector3.one, 0.2f).SetLink(gameObject);
                    coinInconViewList.Add(coinIcon);
                    SoundManager.Instance.Play<SEContainer>(SEName.SE_GENERATE_COIN, isStopPlaying: true);
                });
            }
        }
        sequence.AppendInterval(0.5f);

        // コイン移動
        sequence.AppendCallback(() =>
        {
            for (int i = 0; i < coinInconViewList.Count; i++)
            {
                float moveDuration = Random.Range(0.25f, 1f);
                coinInconViewList[i].Move(coinButton.transform.position, moveDuration, () =>
                {
                    coinButton.transform.localScale = Vector3.one;
                    coinButton.transform.DOScale(Vector3.one * 1.5f, 0.1f).SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            coinButton.transform.localScale = Vector3.one;
                        });
                });
            }
        });

        sequence.AppendInterval(1.2f);

        sequence.OnComplete(() =>
        {
            moneyText.gameObject.SetActive(true);
            onCompleteCallback.Invoke();
        });
    }

    /// <summary>
    /// コイン獲得演出を表示
    /// </summary>
    /// <param name="getMoneyNum"></param>
    /// <param name="onCompleteCallback"></param>
    public void ShowGetMoney(int getMoneyNum, UnityAction onCompleteCallback)
    {
        if (getMoneyNum == 0)
        {
            onCompleteCallback.Invoke();
            return;
        }
        var getMoneyRectTransform = getMoneyText.GetComponent<RectTransform>();
        const float GET_COIN_TEXT_MOVE_DURATION = 1f;
        var saveData = UserDataProvider.Instance.GetSaveData();
        Sequence sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                // 獲得コインの表示
                getMoneyText.gameObject.SetActive(getMoneyNum != 0);
                getMoneyText.text = $"+{getMoneyNum}$";
                SoundManager.Instance.Play<SEContainer>(SEName.SE_MONEY_RESULT);
            })
            .Join(
                // 獲得コイン数を表示
                getMoneyRectTransform
                    .DOLocalMove(Vector3.up * 100f, GET_COIN_TEXT_MOVE_DURATION)
                    .SetRelative()
                    .OnComplete(() =>
                    {
                        getMoneyText.gameObject.SetActive(false);
                    })
            )
            .OnComplete(() =>
            {
                onCompleteCallback.Invoke();
            });
    }

    public void ShowMaxProgressEffect()
    {
        // titleTownView.ShowMaxProgress();
    }

    /// <summary>
    /// オーダーの表示
    /// </summary>
    public void ShowOrder()
    {
        playButtonImage.gameObject.SetActive(true);
        shopRoot.SetActive(true);
        if (UserDataProvider.Instance.IsTutorialStage())
        {
            tutorialPointer.SetActive(true);
        }
    }

    public void HideButtonAndOrder()
    {
        commonRoot.alpha = 0f;
        commonRoot.interactable = false;
    }

    public void ShowShop()
    {
        mainRoot.alpha = 0f;
        mainRoot.interactable = false;

        coinButton.gameObject.SetActive(true);
    }

    public void UpdateShop()
    {
        var bagData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Bag, UserDataProvider.Instance.GetNextShotContentId(TitleConstant.ShopItemType.Bag));
        bagItemView.SetData(bagData);
        var avatarData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Avatar, UserDataProvider.Instance.GetNextShotContentId(TitleConstant.ShopItemType.Avatar));
        avatarItemView.SetData(avatarData);
        var bicycleData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Bicycle, UserDataProvider.Instance.GetNextShotContentId(TitleConstant.ShopItemType.Bicycle));
        bicycleItemView.SetData(bicycleData);
    }

    public void HideShop()
    {
        mainRoot.alpha = 1f;
        mainRoot.interactable = true;

        coinButton.gameObject.SetActive(true);
    }

    public void ShowWarpButton()
    {
        warpButton.gameObject.SetActive(true);
        warpButtonAnimation.Show();
        SoundManager.Instance.Play<SEContainer>(SEName.SE_ORDER);
    }


    public void UpdateCoin(int useCoinNum)
    {
        if (useCoinNum == 0)
        {
            return;
        }
        var saveData = UserDataProvider.Instance.GetSaveData();
        var getMoneyRectTransform = getMoneyText.GetComponent<RectTransform>();
        Sequence sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                getMoneyRectTransform.anchoredPosition = Vector2.zero;
                getMoneyText.gameObject.SetActive(true);
                getMoneyText.text = $"-{useCoinNum}$";
            })
            .Append(
                // 消費コイン数を表示
                getMoneyRectTransform
                    .DOLocalMove(Vector3.down * 100f, 1f)
                    .SetRelative()
                    .OnComplete(() =>
                    {
                        getMoneyText.gameObject.SetActive(false);
                    })
            )
            .OnComplete(() =>
            {
                moneyText.text = $"{saveData.MoneyNum / 1000f}K";
            });
    }

    /// <summary>
    /// 拠点獲得演出を表示
    /// </summary>
    /// <param name="clearReward"></param>
    /// <param name="onCompleteCallback"></param>
    public void ShowBaseEarnStaging(BaseRewardData.BaseLevelUpReward clearReward, UnityAction onCompleteCallback)
    {
        titleBaseProgressView.ShowBaseEarnStaging(clearReward, () =>
        {
            // 拠点獲得なら演出スキップ
            if (clearReward.BaseStep == 0)
            {
                onCompleteCallback.Invoke();
                return;
            }

            ShowGetReward(clearReward, onCompleteCallback);
        });
    }

    /// <summary>
    /// 獲得した報酬を表示
    /// </summary>
    /// <param name="clearReward"></param>
    /// <param name="onCompleteCallback"></param>
    public void ShowGetReward(BaseRewardData.BaseLevelUpReward clearReward, UnityAction onCompleteCallback)
    {
        // 獲得した家具の一枚絵を表示
        levelUpBaseRoot.SetActive(true);
        levelUpBaseBackgroundRoot.SetActive(clearReward.BaseStep != 0);
        gotBaseTitleImage.SetActive(clearReward.BaseStep == 0);
        gotFurnitureTitleImage.SetActive(clearReward.BaseStep != 0);
        SoundManager.Instance.Play<SEContainer>(SEName.SE_GOAL);
        DOVirtual.DelayedCall(STAGE_CLEAR_SE_DELAY, () =>
        {
            SoundManager.Instance.Play<SEContainer>(SEName.SE_STAGE_CLEAR);
        });
        if (clearReward.FurnitureSprite != null)
        {
            gotFurnitureImage.gameObject.SetActive(true);
            gotFurnitureImage.sprite = clearReward.FurnitureSprite;
        }
        else
        {
            gotFurnitureImage.gameObject.SetActive(false);

        }
        // 一定秒数後に完了コールバック
        DOVirtual.DelayedCall(SHOW_GET_REWARD_DURATION, () =>
        {
            levelUpBaseRoot.SetActive(false);
            onCompleteCallback?.Invoke();
        });
    }

    /// <summary>
    /// 拠点の進捗を更新
    /// </summary>
    /// <param name="currentRewardList"></param>
    public void UpdateBaseProgress(List<BaseRewardData.BaseLevelUpReward> currentRewardList)
    {
        var saveData = UserDataProvider.Instance.GetSaveData();
        titleBaseProgressView.Init(saveData.BaseLevel, saveData.StageLevel - 1, currentRewardList, false);
    }

    /// <summary>
    /// 購入チュートリアルの更新
    /// </summary>
    /// <param name="tutorialId"></param>
    public void UpdatePurchaseTutorial(int tutorialId)
    {
        switch (tutorialId)
        {
            case InGameConstant.BAG_TUTORIAL_ID:
                bagItemView.ShowTutorialCursor();
                avatarItemView.HideTutorialCursor();
                bicycleItemView.HideTutorialCursor();
                break;
            case InGameConstant.AVATAR_TUTORIAL_ID:
                bagItemView.HideTutorialCursor();
                avatarItemView.ShowTutorialCursor();
                bicycleItemView.HideTutorialCursor();
                break;
            case InGameConstant.BICYCLE_TUTORIAL_ID:
                bagItemView.HideTutorialCursor();
                avatarItemView.HideTutorialCursor();
                bicycleItemView.ShowTutorialCursor();
                break;
            default:
                bagItemView.HideTutorialCursor(isLockButton: false);
                avatarItemView.HideTutorialCursor(isLockButton: false);
                bicycleItemView.HideTutorialCursor(isLockButton: false);
                break;
        }
    }

    /// <summary>
    /// 設定画面を表示する
    /// </summary>
    public void ShowSettingWindow()
    {
        var dialog = DialogManager.Instance.Show<SettingWindowView>(
            string.Empty,
            string.Empty,
            null,
            DialogManager.ButtonType.YES, DialogManager.ButtonType.NO
        );
        dialog.SetData();
        dialog.SetButtonText(DialogManager.ButtonType.YES, "OK");
    }

    /// <summary>
    /// 未実装テキストを表示
    /// </summary>
    public void ShowUnimplementedText()
    {
        var unimplementedRectTransform = unimplementedText.GetComponent<RectTransform>();
        const float UNIMPLEMENTED_TEXT_MOVE_DURATION = 1f;
        unimplementedText.gameObject.SetActive(true);
        unimplementedRectTransform.anchoredPosition = new Vector2(0, -100f);
        unimplementedTween.Kill();
        unimplementedTween = unimplementedRectTransform
            .DOLocalMove(Vector3.zero, UNIMPLEMENTED_TEXT_MOVE_DURATION)
            .OnComplete(() =>
            {
                unimplementedRectTransform.gameObject.SetActive(false);
            });
    }

    //================================================================================
    // UIイベント
    //================================================================================
    /// <summary>
    /// ボタンクリック
    /// </summary>
    public void OnClickButton(int type)
    {
        if (clickHandler != null)
        {
            clickHandler((TitleConstant.ButtonType)type, 0);
        }
    }

    /// <summary>
    /// オーダーボタンクリック
    /// </summary>
    public void OnClickOrderButton()
    {
        if (clickHandler != null)
        {
            clickHandler(TitleConstant.ButtonType.Order, 0);
        }
    }

    /// <summary>
    /// 設定ボタンクリック
    /// </summary>
    public void OnClickSettingButton()
    {
        if (clickHandler != null)
        {
            clickHandler(TitleConstant.ButtonType.Setting, 0);
        }
    }

    /// <summary>
    /// デバッグボタンクリック
    /// </summary>
    public void OnClickDebugButton()
    {
        DebugWindowView.Show(debugWindowViewPrefab);
    }
}