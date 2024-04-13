using System;
using UnityEngine;

[Serializable]
public class BagData : ShopItemData
{
    /// <summary>
    /// 引き寄せレベル
    /// </summary>
    [Header("引き寄せレベル")]
    public int magnetLevel;

    /// <summary>
    /// バッグの見た目
    /// </summary>
    [Header("バッグの見た目")]
    public GameObject bagObject;

}