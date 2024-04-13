using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TitleOrderWindowView : GeneralDialog
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject normalOrder;
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private List<Image> normalLevelStarIconList;
    [SerializeField] private List<OrderFoodIconListItemView> orderFoodIconViewList;
    [SerializeField] private GameObject specialOrder;
    [SerializeField] private List<Image> specialLevelStarIconList;
    //================================================================================
    // ローカル
    //================================================================================

    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 表示.
    /// </summary>
    public static TitleOrderWindowView Show(TitleOrderWindowView dialogPrefab, StageData selectStageData, UnityAction<DialogManager.ButtonType> onClickButtonListener)
    {
        var dialog = DialogManager.Instance.Show<TitleOrderWindowView>(
            dialogPrefab,
            string.Empty,
            string.Empty,
            onClickButtonListener,
            DialogManager.ButtonType.YES, DialogManager.ButtonType.NO
        );
        dialog.SetData(selectStageData);
        return dialog;
    }

    /// <summary>
    /// データ設定
    /// </summary>
    public void SetData(StageData selectStageData)
    {
        root.SetActive(true);
        normalOrder.SetActive(false);
        specialOrder.SetActive(false);
        foreach (var item in normalLevelStarIconList)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in orderFoodIconViewList)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in specialLevelStarIconList)
        {
            item.gameObject.SetActive(false);
        }
        if (selectStageData.IsBonusStage)
        {
            SetSpecialStageData(selectStageData);
        }
        else
        {
            SetNormalStageData(selectStageData);
        }
    }

    private void SetNormalStageData(StageData selectStageData)
    {
        normalOrder.SetActive(true);
        characterIcon.sprite = AssetManager.Instance.LoadCharacterIconSprite(selectStageData.OrdererData.IconName);
        characterName.text = $"{selectStageData.OrdererData.Name} Order";
        for (int i = 0; i < selectStageData.Difficulty; i++)
        {
            normalLevelStarIconList[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < orderFoodIconViewList.Count; i++)
        {
            if (selectStageData.OrderFoodDataList.Count < i + 1)
            {
                break;
            }
            orderFoodIconViewList[i].gameObject.SetActive(true);
            orderFoodIconViewList[i].Init(selectStageData.OrderFoodDataList[i].FoodData);
            orderFoodIconViewList[i].SetNum(selectStageData.OrderFoodDataList[i].Num);
            orderFoodIconViewList[i].SetPossess(true);
        }
    }

    private void SetSpecialStageData(StageData selectStageData)
    {
        specialOrder.SetActive(true);
        for (int i = 0; i < selectStageData.Difficulty; i++)
        {
            specialLevelStarIconList[i].gameObject.SetActive(true);
        }
    }

    //================================================================================
    // UIイベント
    //================================================================================
}