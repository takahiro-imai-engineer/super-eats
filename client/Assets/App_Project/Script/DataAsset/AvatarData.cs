using System;
using UnityEngine;

[Serializable]
public class AvatarData : ShopItemData
{
    /// <summary>
    /// アバター番号
    /// </summary>
    [Header("アバター番号")]
    public int avatarId;

    /// <summary>
    /// おまけコイン
    /// </summary>
    [Header("おまけコイン")]
    public int bonusCoin;
}