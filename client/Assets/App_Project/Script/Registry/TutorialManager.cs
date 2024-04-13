using app_system;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviourSingleton<TutorialManager>, IGameRegistry
{

    /// <summary>チュートリアルデータのリスト</summary>
    [SerializeField] private List<TutorialData> tutorialDataList;

    //================================================================================
    // ローカル
    //================================================================================

    //================================================================================
    // プロパティ
    //================================================================================
    public int CurrentTutorialId { get; private set; } = 0;
    public bool IsPurchaseTutorial => CurrentTutorialId != 0;

    //================================================================================
    // ゲームセッティング
    //================================================================================
    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting(GameSetting gameSetting)
    {
        CurrentTutorialId = 0;
    }

    public TutorialData GetTutorialData(int tutorialId)
    {
        var tutorialData = tutorialDataList.FirstOrDefault(d => d.TutorialId == tutorialId);
        if (tutorialData == null)
        {
            Debug.LogError($"チュートリアルデータが見つかりませんでした。TutorialId={tutorialId}");
        }
        return tutorialData;
    }

    /// <summary>
    /// インゲームチュートリアル
    /// </summary>
    /// <param name="tutorialData"></param>
    /// <param name="completeCallback"></param>
    /// <returns></returns>
    public async void ShowInGameTutorial(TutorialData tutorialData, UnityAction completeCallback)
    {
        Debug.Log($"チュートリアル開始.ID={tutorialData.TutorialId}.Type={tutorialData.TutorialType}");
        bool isHideDialog = false;
        GeneralDialog generalDialog = null;
        if (tutorialData.TutorialType == InGameConstant.TutorialType.PopUpDialog)
        {
            // ポップアップ
            var dialog = DialogManager.Instance.Show<TutorialWindow01>(
                string.Empty,
                string.Empty,
                (buttonType) =>
                {
                    isHideDialog = true;
                    completeCallback?.Invoke();
                },
                DialogManager.ButtonType.YES, DialogManager.ButtonType.NO
            );
            dialog.SetData(tutorialData);
            generalDialog = dialog;
        }
        else if (tutorialData.TutorialType == InGameConstant.TutorialType.OperationInstruction)
        {
            //　操作説明
            var dialog = DialogManager.Instance.Show<TutorialWindow02>(
                string.Empty,
                string.Empty,
                (buttonType) =>
                {
                    isHideDialog = true;
                    completeCallback?.Invoke();
                },
                DialogManager.ButtonType.YES, DialogManager.ButtonType.NO
            );
            dialog.SetData(tutorialData);
            generalDialog = dialog;
        }
        else
        {
            Debug.LogError($"未対応のチュートリアルタイプ:{tutorialData.TutorialType}");
            return;
        }



        if (tutorialData.ShowDuration <= 0)
        {
            return;
        }

        // 一定時間経過で自動消去
        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(tutorialData.ShowDuration));

        if (isHideDialog) return;

        DialogManager.Instance.Dismiss(generalDialog, null);
        completeCallback?.Invoke();
    }

    /// <summary>
    /// ホームチュートリアル
    /// </summary>
    /// <param name="tutorialData"></param>
    /// <param name="completeCallback"></param>
    /// <returns></returns>
    public void ShowHomeTutorial(TutorialData tutorialData, UnityAction completeCallback = null)
    {
        var dialog = DialogManager.Instance.Show<TutorialWindow03>(
            string.Empty,
            string.Empty,
            (buttonType) =>
            {
                completeCallback?.Invoke();
            },
            DialogManager.ButtonType.YES, DialogManager.ButtonType.NO
        );
        dialog.SetData(tutorialData);
    }

    /// <summary>
    /// チュートリアル用
    /// 購入できるアイテムタイプを取得する
    /// </summary>
    /// <returns></returns>
    public TitleConstant.ShopItemType GetPurchaseTutorialShopItemType()
    {
        var saveData = UserDataProvider.Instance.GetSaveData();
        // バッグ
        var bagData = AssetManager.Instance.GetShopItemDataList(TitleConstant.ShopItemType.Bag).FirstOrDefault(d => d.id == InGameConstant.FIRST_PURCHASE_CONTENT_ID);
        if (
            bagData != null &&
            saveData.PurchaseBagIds.Contains(bagData.id).IsFalse() &&
            saveData.MoneyNum >= bagData.needCoin
        )
        {
            return TitleConstant.ShopItemType.Bag;
        }
        // 自転車
        var bicycleData = AssetManager.Instance.GetShopItemDataList(TitleConstant.ShopItemType.Bicycle).FirstOrDefault(d => d.id == InGameConstant.FIRST_PURCHASE_CONTENT_ID);
        if (
            bicycleData != null &&
            saveData.PurchaseBicycleIds.Contains(bicycleData.id).IsFalse() &&
            saveData.MoneyNum >= bicycleData.needCoin
        )
        {
            return TitleConstant.ShopItemType.Bicycle;
        }
        // アバター
        var avatarData = AssetManager.Instance.GetShopItemDataList(TitleConstant.ShopItemType.Avatar).FirstOrDefault(d => d.id == InGameConstant.FIRST_PURCHASE_CONTENT_ID);
        if (
            avatarData != null &&
            saveData.PurchaseAvatarIds.Contains(avatarData.id).IsFalse() &&
            saveData.MoneyNum >= avatarData.needCoin
        )
        {
            return TitleConstant.ShopItemType.Avatar;
        }
        return TitleConstant.ShopItemType.None;
    }

    public void StartPurchaseTutorial(int tutorialId)
    {
        CurrentTutorialId = tutorialId;
    }

    public void EndPurchaseTutorial()
    {
        // チュートリアル未クリアなら、表示
        var tutorialData = TutorialManager.Instance.GetTutorialData(CurrentTutorialId);
        if (tutorialData == null)
        {
            Debug.LogError($"チュートリアルデータの取得に失敗。TutorialId={CurrentTutorialId}");
            CurrentTutorialId = 0;
            return;
        }
        // 購入完了まで、他のボタンを押せないようにする
        TutorialManager.Instance.ShowHomeTutorial(tutorialData, () =>
        {
            var saveData = UserDataProvider.Instance.GetSaveData();
            saveData.ClearedTutorialIds.Add(tutorialData.TutorialId);
            saveData.Save();
        });
        CurrentTutorialId = 0;
    }

    /// <summary>
    /// シャットダウン
    /// 
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    public void Shutdown()
    {

    }
}