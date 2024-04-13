using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 注文者データ
/// </summary>
//[CreateAssetMenu]
public class OrdererData : ScriptableObject
{

    [Header("キャラクターID")]
    [SerializeField] private int id = 0;

    [Header("名前")]
    [SerializeField] private string name = "Character_00";

    [Header("アイコン名")]
    [SerializeField] private string iconName = null;

    /// <summary>キャラクターID</summary>
    public int Id => id;

    /// <summary>キャラクターの名前</summary>
    public string Name => name;

    /// <summary>キャラクターのアイコン名</summary>
    public string IconName => iconName;
}