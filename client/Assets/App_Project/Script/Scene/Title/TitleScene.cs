using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AmazingAssets.CurvedWorld;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using template;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : SceneBase
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField]
    private Light light;

    [SerializeField]
    private TitleCameraController cameraController;

    [SerializeField]
    private TitlePlayerController playerController;

    [SerializeField]
    private TitleView titleView;

    [SerializeField]
    private BaseRewardData baseRewardData;

    //================================================================================
    // ローカル
    //================================================================================
    /// <summary>セーブデータ</summary>
    private SaveData saveData;

    /// <summary>選択されたステージデータ</summary>
    [NonReorderable]
    private StageData selectStageData;

    //================================================================================
    // 定数
    //================================================================================

    //--------------------------------------------------------------------------------
    // static
    //--------------------------------------------------------------------------------
    public static InGameVariant Variant => GameVariant.Instance.Get<InGameVariant>();

    //================================================================================
    // メソッド
    //================================================================================
    private void Start()
    {
        saveData = UserDataProvider.Instance.GetSaveData();
        SoundManager.Instance.SetMute<BGMContainer>(saveData.IsMuteBgm);
        SoundManager.Instance.SetMute<SEContainer>(saveData.IsMuteSe);
        GameRegistry.SetFrameRate(saveData.IsFrameRate60 ? 60 : 30);

        Init(this.GetCancellationTokenOnDestroy()).Forget();
    }

    public async UniTask Init(CancellationToken ct)
    {
        DeInputUI.On();
#if UNITY_ANDROID && !UNITY_EDITOR
        await AssetManager.Instance.SetUpPlayAssetDelivery(ct);
#else
        await AssetManager.Instance.SetUpAssetBundle();
#endif
        playerController.Init();
        cameraController.ShowHome();
        selectStageData = GetStageData(saveData.StageLevel);
        if (selectStageData != null)
            Variant.SelectGraphicData = StageManager.Instance.GetGraphicData(selectStageData.GroupId);
        var currentRewardList = baseRewardData.GetCurrentRewardList(saveData.BaseLevel);
        var isLevelUpBase = IsLevelUpBase(currentRewardList);
        titleView.Init(OnClickButton, PurchaseShopContent);
        playerController.ChangeAvatar(saveData.SelectAvatarId);
        playerController.ChangeBag(saveData.SelectBagId);
        playerController.ChangeBicycle(saveData.SelectBicycleId);
        AdsManager.Instance.LoadBanner();

        if (CurvedWorldController.Instance != null) CurvedWorldController.Instance.enabled = false;

        OverlayGroup.Instance.Dismiss<GameLoadingScreen>();
        OverlayGroup.Instance.Dismiss<TipsScreen>();
        Fade.FadeIn(() =>
        {
            titleView.Show(
                selectStageData,
                currentRewardList,
                isLevelUpBase,
                () =>
                {
                    if (isLevelUpBase)
                        LevelUpBase(currentRewardList);
                    else
                        ShowShopTutorial();
                }
            );
            PlayBgm();
        }, duration: 1f);
    }

    private bool IsLevelUpBase(List<BaseRewardData.BaseLevelUpReward> baseLevelUpRewardList)
    {
        var currentBaseLevel = saveData.BaseLevel;
        foreach (var item in baseLevelUpRewardList)
            if (item != null && saveData.StageLevel - 1 >= item.StageLevel && item.BaseLevel > currentBaseLevel)
                return true;
        return false;
    }

    private StageData GetStageData(int level)
    {
        return StageManager.Instance.GetStageData(level);
    }

    /// <summary>
    /// オーダー選択
    /// </summary>
    private void SelectOrder()
    {
        if (Variant.IsDebugStage.IsFalse()) Variant.SelectStageData = selectStageData;
        Debug.Log(Variant.SelectStageData.StageName);
        StartCoroutine(ToInGame());
    }

    /// <summary>
    /// インゲームの遷移
    /// </summary>
    private IEnumerator ToInGame()
    {
        var tipsScreen = OverlayGroup.Instance.Show<TipsScreen>();
        tipsScreen.Init();

        yield return null;

        var asyncLoadScene = SceneManager.LoadSceneAsync(GameConstant.Scene.InGame.ToString());
        asyncLoadScene.allowSceneActivation = false;
        Variant.GetMoneyNum = 0;
        Variant.GetJewelNum = 0;
        AssetManager.Instance.ReleaseStageAsset();

        while (true)
        {
            yield return null;

            // tipsScreen.SetProgress(asyncLoadScene.progress);
            Debug.Log($"ロード進捗: {asyncLoadScene.progress}");

            if (asyncLoadScene.progress >= 0.9f)
            {
                tipsScreen.SetProgress(1f, 1f);
                yield return new WaitForSeconds(1f);
                asyncLoadScene.allowSceneActivation = true;
                break;
            }
        }
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    private void PlayBgm()
    {
        SoundManager.Instance.SetVolume<BGMContainer>(0.2f);
        var playBgmName = BGMName.BGM_GARAGE;
        if (SoundManager.Instance.IsPlaying<BGMContainer>(playBgmName))
        {
            Debug.Log("BGM再生済み");
            return;
        }

        SoundManager.Instance.Stop<BGMContainer>();
        SoundManager.Instance.Play<BGMContainer>(playBgmName);
    }

    //================================================================================
    // ステージ進行関連
    //================================================================================

    /// <summary>
    /// 次のステージに遷移
    /// </summary>
    /// <param name="currentStageWorldData"></param>
    /// <param name="nextStageWorldData"></param>
    private void StartWarpNextStage()
    {
        DeInputUI.On();
        Fade.FadeIn(() =>
        {
            titleView.ShowWarpNextStage(
                saveData.StageLevel,
                saveData.TotalMoneyNum,
                ShowWarpPerformance
            );
            PlayBgm();
        }, duration: 1f);
    }

    /// <summary>
    /// ステージワープ演出再生
    /// </summary>
    private void ShowWarpPerformance()
    {
        var tempLightIntensity = light.intensity;
        const float targetLightIntensity = 0.75f;
        var sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                titleView.ShowMaxProgressEffect();
                // BGM停止
                SoundManager.Instance.Stop<BGMContainer>();
                cameraController.ShowShop(TitleConstant.ShopItemType.Bicycle);
                titleView.HideButtonAndOrder();
            })
            .Append(
                // NOTE: ライトを徐々に暗くする
                DOTween.To(
                        () => tempLightIntensity,
                        num => tempLightIntensity = num,
                        targetLightIntensity,
                        1f
                    )
                    .OnUpdate(() => light.intensity = tempLightIntensity)
            )
            .AppendCallback(() =>
            {
                playerController.ShowBicycleEffect();
                SoundManager.Instance.Play<SEContainer>(SEName.SE_TIME_TO_WARP);
            })
            .AppendInterval(3f)
            .OnComplete(() =>
            {
                DeInputUI.Off();
                titleView.ShowWarpButton();
            });
    }

    /// <summary>
    /// 次のステージにワープ
    /// </summary>
    private void WarpNextStage()
    {
        SoundManager.Instance.Stop<SEContainer>(SEName.SE_TIME_TO_WARP);
        SoundManager.Instance.Stop<BGMContainer>();
        SoundManager.Instance.Play<SEContainer>(SEName.SE_MAX_GAUGE);
        var asyncLoadScene = SceneManager.LoadSceneAsync(GameConstant.Scene.WarpNextStage.ToString());
        asyncLoadScene.allowSceneActivation = false;

        Fade.FadeOut(() =>
        {
            FirebaseManager.Instance.LogAnalyticsWorldClearEvent(1);
            saveData.TotalMoneyNum = 0;
            saveData.Save();
            Variant.GetMoneyNum = 0;
            Variant.GetJewelNum = 0;
            asyncLoadScene.allowSceneActivation = true;
        }, duration: 0.5f);
    }

    //================================================================================
    // ショップ関連
    //================================================================================
    /// <summary>
    /// ショップ表示
    /// </summary>
    private void ShowShop()
    {
        cameraController.ShowShop(TitleConstant.ShopItemType.Bag);
        titleView.ShowShop();
    }

    /// <summary>
    /// ショップチュートリアル表示
    /// </summary>
    private void ShowShopTutorial()
    {
        // 購入可能なアイテムタイプを取得
        var shopItemType = TutorialManager.Instance.GetPurchaseTutorialShopItemType();
        var isShowTutorial = false;
        var showTutorialId = 0;
        if (shopItemType == TitleConstant.ShopItemType.None)
        {
            titleView.UpdatePurchaseTutorial(showTutorialId);
            DeInputUI.Off();
            return;
        }

        // NOTE: チュートリアルの表示優先度は、バッグ  > 自転車 > アバター
        if (shopItemType == TitleConstant.ShopItemType.Bag)
        {
            showTutorialId = InGameConstant.BAG_TUTORIAL_ID;
            isShowTutorial = !saveData.ClearedTutorialIds.Contains(showTutorialId);
        }
        else if (shopItemType == TitleConstant.ShopItemType.Bicycle)
        {
            showTutorialId = InGameConstant.BICYCLE_TUTORIAL_ID;
            isShowTutorial = !saveData.ClearedTutorialIds.Contains(showTutorialId);
        }
        else if (shopItemType == TitleConstant.ShopItemType.Avatar)
        {
            showTutorialId = InGameConstant.AVATAR_TUTORIAL_ID;
            isShowTutorial = !saveData.ClearedTutorialIds.Contains(showTutorialId);
        }

        if (isShowTutorial)
        {
            TutorialManager.Instance.StartPurchaseTutorial(showTutorialId);
            titleView.UpdatePurchaseTutorial(showTutorialId);
        }
        else
        {
            titleView.UpdatePurchaseTutorial(showTutorialId);
        }

        DeInputUI.Off();
    }

    /// <summary>
    /// ショップコンテンツ購入
    /// </summary>
    private async void PurchaseShopContent(TitleConstant.ShopItemType shopItemType, int contentId)
    {
        Debug.Log($"コンテンツ購入。Type={shopItemType} ContentId={contentId}");
        var shopItemData = AssetManager.Instance.GetShopItemData(shopItemType, contentId);
        if (shopItemData == null)
        {
            Debug.LogError($"コンテンツの取得に失敗. shopItemType={shopItemType}. contentId={contentId}");
            return;
        }

        // 購入に必要な通貨が足りるか判定
        if (saveData.MoneyNum >= shopItemData.needCoin)
        {
            saveData.MoneyNum -= shopItemData.needCoin;
        }
        else
        {
            Debug.Log("購入に必要な通貨が足りませんでした。");
            return;
        }

        // コンテンツが購入済みの場合、実行しない
        if (shopItemType == TitleConstant.ShopItemType.Bag)
        {
            if (saveData.PurchaseBagIds.Contains(contentId))
            {
                Debug.LogError($"購入済みのバッグ:{contentId}");
                return;
            }

            saveData.PurchaseBagIds.Add(contentId);
            playerController.ChangeBag(contentId, true);
            saveData.SelectBagId = contentId;
        }
        else if (shopItemType == TitleConstant.ShopItemType.Avatar)
        {
            if (saveData.PurchaseAvatarIds.Contains(contentId))
            {
                Debug.LogError($"購入済みのアバター:{contentId}");
                return;
            }

            saveData.PurchaseAvatarIds.Add(contentId);
            playerController.ChangeAvatar(contentId, true);
            saveData.SelectAvatarId = contentId;
        }
        else if (shopItemType == TitleConstant.ShopItemType.Bicycle)
        {
            if (saveData.PurchaseBicycleIds.Contains(contentId))
            {
                Debug.LogError($"購入済みの自転車:{contentId}");
                return;
            }

            saveData.PurchaseBicycleIds.Add(contentId);
            playerController.ChangeBicycle(contentId, true);
            saveData.SelectBicycleId = contentId;
        }
        else
        {
            return;
        }

        SoundManager.Instance.Play<SEContainer>(SEName.SE_PURCHASE_ITEM);
        saveData.Save();
        FirebaseManager.Instance.LogAnalyticsPurchaseItemEvent(shopItemType, contentId);
        Variant.SelectContentId = contentId;
        titleView.UpdateShop();
        titleView.UpdateCoin(shopItemData.needCoin);
        if (TutorialManager.Instance.CurrentTutorialId != 0)
        {
            DeInputUI.On();
            await UniTask.Delay(1000);
            TutorialManager.Instance.EndPurchaseTutorial();
            // ショップチュートリアルを更新
            ShowShopTutorial();
        }
    }

    //================================================================================
    // 拠点
    //================================================================================
    private void LevelUpBase(List<BaseRewardData.BaseLevelUpReward> currentRewardList)
    {
        var clearReward = currentRewardList.FirstOrDefault(d =>
            saveData.StageLevel - 1 >= d.StageLevel && d.BaseLevel > saveData.BaseLevel);
        saveData.BaseLevel = clearReward.BaseLevel;
        saveData.Save();
        if (clearReward.BaseStep == 0)
            // 拠点変更演出
            titleView.ShowBaseEarnStaging(clearReward, () =>
            {
                // 画面切り替え
                var shutterScreen = OverlayGroup.Instance.Show<ShutterScreen>();
                shutterScreen.Show(
                    () =>
                    {
                        var currentRewardList = baseRewardData.GetCurrentRewardList(saveData.BaseLevel);
                        titleView.UpdateBaseProgress(currentRewardList);
                        titleView.LevelUpBase(saveData.BaseLevel, titleView.ShowOrder);
                    },
                    () =>
                    {
                        // YouGet表示＆花火エフェクト＆SE再生
                        titleView.ShowGetReward(clearReward, () => { ShowShopTutorial(); });
                    }
                );
            });
        else
            // 拠点レベルアップ演出
            titleView.ShowBaseEarnStaging(clearReward, () =>
            {
                // 画面切り替え
                titleView.LevelUpBase(saveData.BaseLevel, titleView.ShowOrder);
                ShowShopTutorial();
            });
    }

    //================================================================================
    // UIイベント
    //================================================================================
    /// <summary>
    /// ボタンクリック
    /// </summary>
    public void OnClickButton(TitleConstant.ButtonType buttonType, int value)
    {
        if (TutorialManager.Instance.IsPurchaseTutorial)
            // チュートリアル中は、ぼたんを押せないようにする
            return;
        SoundManager.Instance.Play<SEContainer>(SEName.SE_TAP_BUTTON);
        if (buttonType == TitleConstant.ButtonType.Coin)
        {
            Debug.Log("コインボタンクリック");
            Debug.Log(saveData.MoneyNum);
        }
        else if (buttonType == TitleConstant.ButtonType.Jewel)
        {
            Debug.Log("ジュエルボタンクリック");
            Debug.Log(saveData.JewelNum);
        }
        else if (buttonType == TitleConstant.ButtonType.Shop)
        {
            Debug.Log("ショップボタンクリック");
            ShowShop();
        }
        else if (buttonType == TitleConstant.ButtonType.Rank)
        {
            Debug.Log("ランクボタンクリック");
            titleView.ShowUnimplementedText();
        }
        else if (buttonType == TitleConstant.ButtonType.RightTownSelect)
        {
            Debug.Log("左タウンセレクトボタンクリック");
        }
        else if (buttonType == TitleConstant.ButtonType.LeftTownSelect)
        {
            Debug.Log("右タウンセレクトボタンクリック");
        }
        else if (buttonType == TitleConstant.ButtonType.Order)
        {
            Debug.Log("オーダーボタンクリック");
            SelectOrder();
        }
        else if (buttonType == TitleConstant.ButtonType.Setting)
        {
            Debug.Log("設定ボタンクリック");
            titleView.ShowSettingWindow();
        }
        else if (buttonType == TitleConstant.ButtonType.WarpNextStage)
        {
            Debug.Log("ワープボタンクリック");
            WarpNextStage();
        }
        else
        {
            Debug.LogError($"UnknownButtonType: {buttonType}");
        }
    }
}
