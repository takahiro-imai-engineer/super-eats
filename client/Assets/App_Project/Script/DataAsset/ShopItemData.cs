using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ShopItemData : ScriptableObject
{
    /// <summary>
    /// ID
    /// </summary>
    public int id;

    /// <summary>
    /// 名前
    /// </summary>
    public string name;

    /// <summary>
    /// アイコン
    /// </summary>
    public string iconName;

    /// <summary>
    /// 必要コイン
    /// </summary>
    [Header("必要コイン")]
    public int needCoin;

    /// <summary>
    /// 必要ジュエル
    /// </summary>
    [Header("必要ジュエル")]
    public int needJewel;

    /// <summary>
    /// デフォルトで解放済みか
    /// </summary>
    [Header("デフォルトで解放済みか")]
    public bool isDefaultOpen = false;
}