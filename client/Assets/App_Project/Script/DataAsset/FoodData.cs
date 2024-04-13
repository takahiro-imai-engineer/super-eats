using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 食べ物データ
/// </summary>
//[CreateAssetMenu]
public class FoodData : ScriptableObject
{

    [Header("食べ物ID")]
    [SerializeField] private int id = 0;

    [Header("名前")]
    [SerializeField] private string name = "Food";

    [Header("アイコン名")]
    [SerializeField] private string iconName = "";
    [Header("値段(ワールド毎に設定)")]
    [SerializeField] private int price = 0;

    /// <summary>食べ物ID</summary>
    public int Id => id;

    /// <summary>食べ物の名前</summary>
    public string Name => name;

    /// <summary>食べ物のアイコン名</summary>
    public string IconName => iconName;
    /// <summary>食べ物の値段</summary>
    public int Price => price;
}