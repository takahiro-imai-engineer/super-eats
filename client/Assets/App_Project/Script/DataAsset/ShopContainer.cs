using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ショップコンテナ
/// </summary>
public class ShopContainer : ScriptableObject
{
    //================================================================================
    // メンバ変数
    //================================================================================
    [Header("バッグデータ")]
    public List<BagData> BagDataList = new List<BagData>();
    [Header("アバターデータ")]
    public List<AvatarData> AvatarDataList = new List<AvatarData>();
    [Header("自転車データ")]
    public List<BicycleData> BicycleDataList = new List<BicycleData>();
}