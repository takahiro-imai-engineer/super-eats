using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderFoodListView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject root;
    [SerializeField] private List<OrderFoodIconListItemView> orderFoodListItemViewList;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="orderFoodList"></param>
    public void Init(List<OrderFood> orderFoodList)
    {
        root.SetActive(false);
        for (int i = 0; i < orderFoodListItemViewList.Count; i++)
        {
            if (i + 1 <= orderFoodList.Count)
            {
                orderFoodListItemViewList[i].Init(orderFoodList[i].FoodData);
                orderFoodListItemViewList[i].SetNum(orderFoodList[i].NecessaryNum);
            }
            else
            {
                orderFoodListItemViewList[i].Hide();
            }
        }
    }

    /// <summary>
    /// 表示
    /// </summary>
    public void Show()
    {
        root.SetActive(true);
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide()
    {
        root.SetActive(false);
    }

    /// <summary>
    /// オーダーフード更新
    /// </summary>
    /// <param name="orderFoodList"></param>
    public void UpdateOrderFood(List<OrderFood> orderFoodList)
    {
        for (int i = 0; i < orderFoodListItemViewList.Count; i++)
        {
            if (i + 1 <= orderFoodList.Count)
            {
                orderFoodListItemViewList[i].SetPossess(orderFoodList[i].IsPossess);
            }
            else
            {
                orderFoodListItemViewList[i].Hide();
            }
        }
    }
}