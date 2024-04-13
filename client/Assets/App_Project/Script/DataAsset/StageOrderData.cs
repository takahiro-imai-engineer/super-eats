using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステージ順番データ
/// </summary>
// [CreateAssetMenu]
public class StageOrderData : ScriptableObject
{

    [Header("順番にプレイするステージデータ")]
    [SerializeField] private List<string> stageOrderDataList;

    /// <summary>ステージ抽選データリスト</summary>
    public List<string> StageOrderDataList => stageOrderDataList;
}