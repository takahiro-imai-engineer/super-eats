using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using template;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// インゲーム管理クラス
/// </summary>
public class InGameController : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private InGameCameraController cameraController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private StageController stageController;
    /// <summary>View</summary>
    [SerializeField] private InGameView inGameView;

    //================================================================================
    // ローカル
    //================================================================================
    // インゲームデータ
    private InGameVariant inGameVariant;
    bool isContinue = false;
    bool isShowInterstitialAd = false;
    private string playBgmName = string.Empty;

    //================================================================================
    // メソッド
    //================================================================================
    void Start()
    {
        inGameVariant = GameVariant.Instance.Get<InGameVariant>();

        Init();

        // RemoteConfig取得
        ParameterManager.Instance.Init(() =>
        {
            // NOTE: 読み込み完了後に何かしたい場合、処理追加
        });
        ParameterManager.Instance.FetchParameter();

        TouchUtility.TouchUtil.BaseScreenSizeX = inGameVariant.BaseScreenSize.x;
        TouchUtility.TouchUtil.BaseScreenSizeY = inGameVariant.BaseScreenSize.y;
        GameRegistry.SetFrameRate(UserDataProvider.Instance.GetSaveData().IsFrameRate60 ? 60 : 30);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private async void Init(bool isRetryGame = false)
    {
#if UNITY_EDITOR
        await AssetManager.Instance.SetUpAssetBundle();
#endif

        inGameVariant.InGameModel = new InGameModel();
        inGameVariant.InGameModel.Init();
        isContinue = false;

        playerController.Init(GetFoodEvent, WindEvent);
        inGameVariant.InGameModel.SetPlayerInfo(playerController);
        cameraController.Init(playerController);
        stageController.Init();
        inGameVariant.InGameModel.SetStageInfo(stageController.CurrentStageData);
        // View初期化
        inGameView.Init(inGameVariant.InGameModel, OnClickButton);
        OverlayGroup.Instance.Dismiss<TipsScreen>();
        PlayBgm(stageController.CurrentStageData.GroupId);
        isShowInterstitialAd = UserDataProvider.Instance.GetSaveData().InterstitialAdStagePlayCount + 1 >= ParameterManager.Instance.ShowInterstitialAdCount;
        if (isShowInterstitialAd) AdsManager.Instance.LoadInterstitialAd();
        Fade.FadeIn(() =>
        {
            if (isRetryGame == false)
            {
                // ゲーム準備
                PrepareGame();
            }
            else
            {
                PrepareGame();
            }
        }, duration: 1f);

    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="stageGroupId"></param>
    private void PlayBgm(int stageGroupId)
    {
        SoundManager.Instance.SetVolume<BGMContainer>(0.2f);
        playBgmName = "";
        if (stageGroupId == InGameConstant.CityStageGroupId)
        {
            playBgmName = BGMName.BGM_CITY;
        }
        else if (stageGroupId == InGameConstant.CyberStageGroupId)
        {
            playBgmName = BGMName.BGM_CYBER;
        }
        else if (stageGroupId == InGameConstant.ApoStageGroupId)
        {
            playBgmName = BGMName.BGM_APOCALYPSE;
        }
        else
        {
            playBgmName = BGMName.BGM_CITY;
        }
        if (SoundManager.Instance.IsPlaying<BGMContainer>(playBgmName))
        {
            Debug.Log("BGM再生済み");
            return;
        }
        SoundManager.Instance.Stop<BGMContainer>();
        SoundManager.Instance.Play<BGMContainer>(playBgmName);
    }

    private void Update()
    {
        if (inGameVariant == null || inGameVariant.InGameModel == null)
        {
            return;
        }
        // Debug.Log (inGameVariant.GetGameStatus ());
        // 初期化中
        if (inGameVariant.GetGameStatus() == InGameConstant.InGameStatus.Init)
        {
            return;
        }
        //　ゲーム終了
        if (inGameVariant.GetGameStatus() == InGameConstant.InGameStatus.Finish)
        {
            return;
        }
        // 移動中もしくは、クラッシュ中ではない
        if (
            inGameVariant.GetGameStatus() != InGameConstant.InGameStatus.Play &&
            inGameVariant.GetGameStatus() != InGameConstant.InGameStatus.Clash
        )
        {
            return;
        }
        // ゲーム中
        PlayGame();
    }

    private void LateUpdate()
    {
        if (inGameVariant == null || inGameVariant.InGameModel == null)
        {
            return;
        }
        if (inGameVariant.GetGameStatus() != InGameConstant.InGameStatus.Play)
        {
            return;
        }
        cameraController.UpdateCamera(playerController);
    }

    /// <summary>
    /// ゲーム準備.
    /// </summary>
    private void PrepareGame()
    {

        inGameVariant.InGameModel.PrepareGame();
        // 準備画面表示
        inGameView.ShowPrepare();
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    private void StartGame()
    {
        // NOTE: RemoteConfigのロードが終わっていなければスタートしない
        if (!ParameterManager.Instance.IsLoadConfigComplete)
        {
            return;
        }
        inGameView.HidePrepare();
        inGameVariant.InGameModel.StartGame();
        inGameView.StartGame();
        cameraController.StartGame();
        playerController.StartGame();
    }

    /// <summary>
    /// ゲーム中.
    /// </summary>
    private void PlayGame()
    {
        inGameVariant.InGameModel.CheckGame(playerController);
        if (inGameVariant.GetGameStatus() == InGameConstant.InGameStatus.Play)
        {
            // 移動
            playerController.Move();
            stageController.UpdateStage(playerController.Position);
            inGameView.UpdatePlay(inGameVariant.InGameModel);
        }
        else if (inGameVariant.GetGameStatus() == InGameConstant.InGameStatus.Clash)
        {
            // クラッシュ
        }
        else
        {
            inGameView.HidePlay(inGameVariant.InGameModel);
        }

        if (inGameVariant.GetGameStatus() == InGameConstant.InGameStatus.Success)
        {
            // Debug.Log ("結果: 成功");
            cameraController.ShowResultCamera(playerController.Position);
            playerController.Success();
            FinishGame();
        }
        else if (inGameVariant.GetGameStatus() == InGameConstant.InGameStatus.Failed)
        {
            // Debug.Log ("結果: 失敗");
            FinishGame();
        }
        else if (inGameVariant.GetGameStatus() == InGameConstant.InGameStatus.DeathPlayer)
        {
            // Debug.Log ("結果: プレイヤー死亡");
            cameraController.ShowDeathCamera();
            FinishGame();
        }
    }

    /// <summary>
    /// 食べ物獲得時のイベント
    /// </summary>
    private void GetFoodEvent()
    {
        inGameVariant.InGameModel.CheckOrderFood();
        // UI更新
        inGameView.UpdateOrderStatus(inGameVariant.InGameModel);
    }

    /// <summary>
    /// 突風時のイベント
    /// </summary>
    /// <param name="pushVector"></param>
    /// <param name="warningTime"></param>
    private void WindEvent(Vector3 pushVector, float warningTime)
    {
        inGameView.ShowWindInfo(pushVector, warningTime);
    }

    /// <summary>
    /// ゲーム終了.
    /// </summary>
    private void FinishGame()
    {
        Debug.Log(inGameVariant.InGameModel.InGameStatus);
        if (
            inGameVariant.InGameModel.InGameResult == InGameConstant.InGameStatus.Success
        )
        {
            // 成功
            SoundManager.Instance.Play<SEContainer>(SEName.SE_GOAL);
            SoundManager.Instance.Play<SEContainer>(SEName.SE_STAGE_CLEAR);
            DOVirtual.DelayedCall(
                2.0f,
                () =>
                {
                    inGameView.ShowResult(inGameVariant.InGameModel, InGameConstant.ResultStatus.SuccessResult);
                }
            );
        }
        else if (
            (inGameVariant.InGameModel.InGameResult == InGameConstant.InGameStatus.Failed && isContinue) ||
            (inGameVariant.InGameModel.InGameResult == InGameConstant.InGameStatus.Failed && playerController.PlayerStatus == InGameConstant.PlayerStatus.FallDeath)
        )
        {
            float waitTime = 1f;
            playerController.Death();
            playerController.ResetGetFood();
            inGameVariant.InGameModel.FailedGame();
            // 失敗かつContinue or 落下死
            DOVirtual.DelayedCall(
                waitTime,
                () =>
                {
                    inGameView.ShowResult(inGameVariant.InGameModel, InGameConstant.ResultStatus.FailedResult);
                }
            );
        }
        else
        {
            float waitTime = 2f;
            playerController.Death();
            DOVirtual.DelayedCall(
                waitTime,
                () =>
                {
                    inGameView.ShowResult(inGameVariant.InGameModel, InGameConstant.ResultStatus.Continue);
                }
            );
        }
        inGameVariant.InGameModel.FinishGame();

    }

    //================================================================================
    // UIイベント
    //================================================================================
    /// <summary>
    /// ボタンクリック
    /// </summary>
    public void OnClickButton(InGameConstant.ButtonType buttonType)
    {
        if (buttonType == InGameConstant.ButtonType.Start)
        {
            StartGame();
        }
        else if (buttonType == InGameConstant.ButtonType.Retry)
        {
            RetryGame(isContinue: false);
            SoundManager.Instance.Play<SEContainer>(SEName.SE_TAP_BUTTON);
        }
        else if (buttonType == InGameConstant.ButtonType.NextGame)
        {
            SoundManager.Instance.Play<SEContainer>(SEName.SE_TAP_BUTTON);
            if (isShowInterstitialAd)
            {
                // 強制広告
                SoundManager.Instance.Stop<BGMContainer>();
                AdsManager.Instance.ShowInterstitialAd(() =>
                {
                    NextGame(isAdsBonus: false);
                    UserDataProvider.Instance.GetSaveData().InterstitialAdStagePlayCount = 0;
                    UserDataProvider.Instance.WriteSaveData();
                }, () =>
                {
                    NextGame(isAdsBonus: false);
                });
            }
            else
            {
                NextGame(isAdsBonus: false);
            }
        }
        else if (buttonType == InGameConstant.ButtonType.Bonus)
        {
            OverlayGroup.Instance.Show<LoadingScreen>();
            // NOTE:　広告ボタンクリック
            SoundManager.Instance.Stop<BGMContainer>();
            AdsManager.Instance.LoadRewardAd(() =>
            {
                OverlayGroup.Instance.Dismiss<LoadingScreen>();
                FirebaseManager.Instance.LogAnalyticsAdCloseEvent(
                    inGameVariant.InGameModel.SelectStageData.Id,
                    inGameVariant.InGameModel.SelectStageData.StageName,
                    isGetReward: true,
                    adType: FirebaseManager.AdType.Reward
                );
                NextGame(isAdsBonus: true);
            }, () =>
            {
                OverlayGroup.Instance.Dismiss<LoadingScreen>();
                FirebaseManager.Instance.LogAnalyticsAdCloseEvent(
                    inGameVariant.InGameModel.SelectStageData.Id,
                    inGameVariant.InGameModel.SelectStageData.StageName,
                    isGetReward: false,
                    adType: FirebaseManager.AdType.Reward
                );
                NextGame(isAdsBonus: false);
            });
            SoundManager.Instance.Play<SEContainer>(SEName.SE_TAP_BUTTON);
        }
        else if (buttonType == InGameConstant.ButtonType.Continue)
        {
            OverlayGroup.Instance.Show<LoadingScreen>();
            SoundManager.Instance.Stop<BGMContainer>();
            AdsManager.Instance.LoadRewardAd(() =>
            {
                OverlayGroup.Instance.Dismiss<LoadingScreen>();
                FirebaseManager.Instance.LogAnalyticsAdCloseEvent(
                    inGameVariant.InGameModel.SelectStageData.Id,
                    inGameVariant.InGameModel.SelectStageData.StageName,
                    isGetReward: true,
                    adType: FirebaseManager.AdType.KeepCombo
                );
                RetryGame(isContinue: true);
            }, () =>
            {
                OverlayGroup.Instance.Dismiss<LoadingScreen>();
                FirebaseManager.Instance.LogAnalyticsAdCloseEvent(
                    inGameVariant.InGameModel.SelectStageData.Id,
                    inGameVariant.InGameModel.SelectStageData.StageName,
                    isGetReward: false,
                    adType: FirebaseManager.AdType.KeepCombo
                );
                RetryGame(isContinue: false);
            });
            SoundManager.Instance.Play<SEContainer>(SEName.SE_TAP_BUTTON);
        }
        else if (buttonType == InGameConstant.ButtonType.Title)
        {
            StartCoroutine(ToTitleScene());
            SoundManager.Instance.Play<SEContainer>(SEName.SE_TAP_BUTTON);
        }
        else
        {
            Debug.LogError($"UnknownButtonType: {buttonType}");
        }
    }

    /// <summary>
    /// ゲームのリトライ
    /// </summary>
    private void RetryGame(bool isContinue)
    {
        FirebaseManager.Instance.LogAnalyticsStageFailEvent(
            inGameVariant.InGameModel.SelectStageData.Id,
            inGameVariant.InGameModel.SelectStageData.StageName,
            isContinue
        );
        if (isContinue.IsTrue())
        {
            this.isContinue = true;
            SoundManager.Instance.Play<BGMContainer>(playBgmName);
            cameraController.Init(playerController);
            playerController.Revive();
            PrepareGame();
            return;
        }
        inGameView.ShowResult(inGameVariant.InGameModel, InGameConstant.ResultStatus.FailedResult);
    }

    /// <summary>
    /// 次のゲームへ
    /// </summary>
    private void NextGame(bool isAdsBonus)
    {
        if (inGameVariant.GetGameStatus() != InGameConstant.InGameStatus.Finish)
        {
            return;
        }
        var saveData = UserDataProvider.Instance.GetSaveData();
        int currentStageLevel = saveData.StageLevel;
        if (
            inGameVariant.InGameModel.InGameResult == InGameConstant.InGameStatus.Success &&
            !inGameVariant.IsDebugStage &&
            inGameVariant.SelectStageData != null
        )
        {
            Debug.Log($"ステージ進行：{currentStageLevel} → {currentStageLevel + 1}");
            currentStageLevel++;
            saveData.StageLevel = currentStageLevel;
            saveData.TotalStageLevel++;
        }
        var addMoneyNum = inGameVariant.InGameModel.GetTotalRewardCoin();
        // NOTE: 広告視聴でコインがx倍
        if (isAdsBonus)
        {
            addMoneyNum *= InGameConstant.RESULT_ADS_BONUS_RATE;
            inGameVariant.IsWatchedAd = true;
        }
        inGameVariant.GetMoneyNum = addMoneyNum;
        inGameVariant.GetJewelNum = playerController.JewelCount;
        inGameVariant.IsClearStage = inGameVariant.InGameModel.InGameResult == InGameConstant.InGameStatus.Success;
        saveData.MoneyNum += addMoneyNum;
        saveData.TotalMoneyNum += addMoneyNum;
        saveData.JewelNum += playerController.JewelCount;
        // 強制広告カウント加算
        saveData.AddInterstitialAdStagePlayCount();
        UserDataProvider.Instance.WriteSaveData();
        // NOTE: 計測
        FirebaseManager.Instance.LogAnalyticsStageClearEvent(
            inGameVariant.InGameModel.SelectStageData.Id,
            inGameVariant.InGameModel.SelectStageData.StageName,
            addMoneyNum,
            isAdsBonus
        );
        if (stageController.IsDebugRepeatStage.IsTrue())
        {
            Init();
            return;
        }
        if (StageManager.Instance.IsClearAllStage(saveData.StageLevel))
        {
            // 最後のステージクリアでエンディングに移行
            ToEnding();
            return;
        }
        StartCoroutine(ToTitleScene());
    }

    /// <summary>
    /// エンディング
    /// </summary>
    private void ToEnding()
    {
        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(GameConstant.Scene.WarpNextStage.ToString());
        asyncLoadScene.allowSceneActivation = false;
        var saveData = UserDataProvider.Instance.GetSaveData();
        Fade.FadeOut(() =>
        {
            saveData.StageLevel = InGameConstant.INIT_STAGE_LEVEL;
            saveData.AllClearCount++;
            saveData.Save();
            inGameVariant.IsShowEnding = true;
            asyncLoadScene.allowSceneActivation = true;
            FirebaseManager.Instance.LogAnalyticsWorldClearEvent(saveData.AllClearCount);
        }, duration: 0.5f);
    }

    /// <summary>
    /// タイトルに戻る
    /// </summary>
    private IEnumerator ToTitleScene()
    {
        var tipsScreen = OverlayGroup.Instance.Show<TipsScreen>();
        tipsScreen.Init();

        yield return null;

        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(GameConstant.Scene.Title.ToString());
        asyncLoadScene.allowSceneActivation = false;
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
}