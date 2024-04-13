using System;
using UnityEngine;

[Serializable]
public class BicycleData : ShopItemData
{
    /// <summary>
    /// 自機ライフ
    /// </summary>
    [Header("自機ライフ")]
    public int playerLife;

    /// <summary>
    /// 自転車の見た目
    /// </summary>
    [Header("自転車の見た目")]
    public GameObject bicycleObject;
}