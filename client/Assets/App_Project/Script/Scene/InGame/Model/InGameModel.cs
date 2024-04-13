using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// インゲームモデルクラス
/// </summary>
public class InGameModel
{
    /// <summary>ゲームの状況</summary>
    public InGameConstant.InGameStatus InGameStatus { get; private set; }

    /// <summary>プレイヤーの情報</summary>
    public PlayerController PlayerInfo { get; private set; }

    /// <summary>選択中のステージ情報</summary>
    public StageData SelectStageData { get; private set; }

    /// <summary>配達食べ物の情報</summary>
    public List<OrderFood> OrderFoodList { get; private set; }

    /// <summary>報酬</summary>
    public int RewardCoin { get; private set; }

    public InGameConstant.InGameStatus InGameResult { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public InGameModel()
    {
        InGameStatus = InGameConstant.InGameStatus.Init;
        PlayerInfo = null;
        SelectStageData = null;
        OrderFoodList = new List<OrderFood>();
        RewardCoin = 0;
        InGameResult = InGameConstant.InGameStatus.None;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        InGameStatus = InGameConstant.InGameStatus.Init;
    }

    /// <summary>
    /// ステージ情報を設定
    /// </summary>
    /// <param name="selectStageData"></param>
    public void SetStageInfo(StageData selectStageData)
    {
        // ステージ設定
        SelectStageData = selectStageData;

        foreach (var foodData in selectStageData.OrderFoodDataList)
        {
            var orderFood = new OrderFood(foodData);
            OrderFoodList.Add(orderFood);
        }
    }

    /// <summary>
    /// 自機を設定
    /// </summary>
    /// <param name="playerInfo"></param>
    public void SetPlayerInfo(PlayerController playerInfo)
    {
        this.PlayerInfo = playerInfo;
    }

    /// <summary>

    /// <summary>
    /// ゲーム準備
    /// </summary>
    public void PrepareGame()
    {
        InGameStatus = InGameConstant.InGameStatus.Prepare;
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void StartGame()
    {
        InGameStatus = InGameConstant.InGameStatus.Play;
    }

    /// <summary>
    /// 終了判定確認
    /// </summary>
    public void CheckGame(PlayerController playerController)
    {

        if (playerController.PlayerStatus == InGameConstant.PlayerStatus.Clash)
        {
            // クラッシュ中
            InGameStatus = InGameConstant.InGameStatus.Clash;
            return;
        }
        else if (playerController.PlayerStatus == InGameConstant.PlayerStatus.Goal)
        {
            // 終了
            InGameStatus = InGameConstant.InGameStatus.Success;
            InGameResult = InGameConstant.InGameStatus.Success;
            return;
        }
        else if (
            playerController.PlayerStatus == InGameConstant.PlayerStatus.Death ||
            playerController.PlayerStatus == InGameConstant.PlayerStatus.FallDeath
        )
        {
            // 死亡
            InGameStatus = InGameConstant.InGameStatus.DeathPlayer;
            InGameResult = InGameConstant.InGameStatus.Failed;
            return;
        }
        InGameStatus = InGameConstant.InGameStatus.Play;
        return;
    }

    /// <summary>
    /// オーダーを達成しているかチェック
    /// </summary>
    public void CheckOrderFood()
    {
        // オーダーのフードを所持しているか判定
        var getFoodIdList = new List<int>(PlayerInfo.GetFoodIdList);
        RewardCoin = 0;
        for (int i = 0; i < OrderFoodList.Count; i++)
        {
            int possessNum = getFoodIdList.Count(getFoodId => getFoodId == OrderFoodList[i].FoodData.Id);
            OrderFoodList[i].PossessNum = possessNum;
            if (OrderFoodList[i].IsPossess)
            {
                getFoodIdList.Remove(OrderFoodList[i].FoodData.Id);
            }
            RewardCoin += OrderFoodList[i].GetPrice() * possessNum;
        }
    }

    public int GetPriceWithDifficultyBonus(int basePrice)
    {
        int resultPrice = basePrice;
        switch (SelectStageData.Difficulty)
        {
            case 2:
                resultPrice += InGameConstant.FoodDifficultyTwoBonus;
                break;
            case 3:
                resultPrice += InGameConstant.FoodDifficultyThreeBonus;
                break;
            case 4:
                resultPrice += InGameConstant.FoodDifficultyFourBonus;
                break;
            case 5:
                resultPrice += InGameConstant.FoodDifficultyFiveBonus;
                break;
        }

        return resultPrice;
    }

    public int GetPickedCoinWithDifficultyBonus()
    {
        int totalCoin = PlayerInfo.CoinCount * SelectStageData.NormalCoinPrice;
        int totalBonusCoin = PlayerInfo.BonusCoinCount * SelectStageData.BonusCoinPrice;
        Debug.Log($"合計コイン獲得量={totalCoin}. 合計ボーナスコイン獲得量={totalBonusCoin}");

        return totalCoin + totalBonusCoin;
    }

    /// <summary>
    /// 結果の評価値を返す
    /// </summary>
    /// <returns></returns>
    public int GetResultStarNum()
    {
        int totalNecessaryNum = OrderFoodList.Select(d => d.NecessaryNum).Sum();
        int totalPossessNum = OrderFoodList.Select(d => d.IsPossess ? d.NecessaryNum : d.PossessNum).Sum();

        float percent = totalNecessaryNum != 0 ? (float)totalPossessNum / (float)totalNecessaryNum : 1f;
        Debug.Log($"<color=orange>食べ物の取得率: {percent}({totalPossessNum}/{totalNecessaryNum})</color>");

        if (0.01f <= percent && percent < 0.5f)
        {
            return 1;
        }
        else if (0.5f <= percent && percent < 1f)
        {
            return 2;
        }
        else if (1f <= percent)
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 合計の報酬を取得
    /// </summary>
    /// <returns></returns>
    public int GetTotalRewardCoin()
    {
        if (InGameResult == InGameConstant.InGameStatus.Success)
        {
            return (int)((RewardCoin + GetPickedCoinWithDifficultyBonus() + PlayerInfo.AvatarBonusCoin));
        }
        else
        {
            // 失敗時は、コインのみ獲得
            return (int)((RewardCoin + GetPickedCoinWithDifficultyBonus()));
        }
    }

    public void FailedGame()
    {
        RewardCoin = 0;
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void FinishGame()
    {
        InGameStatus = InGameConstant.InGameStatus.Finish;

        var sb = new System.Text.StringBuilder();
        sb.Append("<color=yellow>=====オーダー達成状況=====</color>\n");
        for (int i = 0; i < OrderFoodList.Count; i++)
        {
            sb.Append($"ID={OrderFoodList[i].FoodData.Id}. 名前={OrderFoodList[i].FoodData.Name}. 獲得できたか=<color=red>{OrderFoodList[i].IsPossess}</color>\n");
        }
        sb.Append("<color=yellow>=======================</color>\n");
    }
}