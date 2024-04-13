using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrderFoodData
{
    public FoodData FoodData;
    public int Num = 1;
    [Header("-1ならFoodDataの価格を使用")]
    public int Price = -1;
}

/// <summary>
/// ステージデータ
/// </summary>
//[CreateAssetMenu]
public class StageData : ScriptableObject
{

    [Header("ステージID")]
    [Space(20)]
    [Header("----------------------------------------------------------")]
    [Header("ステージ設定")]
    [Header("----------------------------------------------------------")]
    [SerializeField] private int id;
    [SerializeField] private int groupId;

    [Header("ステージ名")]
    [SerializeField] private string stageName;

    [Header("1欠片辺りの長さ(m)")]
    [SerializeField] private float stageTipSize = 100;

    [Header("ゴール用のステージ")]
    [SerializeField] private GameObject goalStage;

    [Header("順番に生成するステージのリスト")]
    [SerializeField] private List<GameObject> stageList;

    [Header("プレイヤーを追従するオブジェクト")]
    [SerializeField] private GameObject playerFollowObject = null;

    [Header("CuvedWorldを有効化するかどうか")]
    [SerializeField] private bool isCurvedWorld = false;

    [Header("水平に曲げる大きさ")]
    [SerializeField] private float horizontalSize = 0f;
    [Header("水平に曲げるオフセット")]
    [SerializeField] private float horizontalOffset = 0f;

    [Header("垂直に曲げる大きさ")]
    [SerializeField] private float verticalSize = 0f;
    [Header("垂直に曲げるオフセット")]
    [SerializeField] private float verticalOffset = 0f;

    [Header("注文者データ")]
    [Space(20)]
    [Header("----------------------------------------------------------")]
    [Header("オーダー設定")]
    [Header("----------------------------------------------------------")]
    [Space(40)]
    [Header("通常コインの価格")]
    [SerializeField] private int normalCoinPrice = 10;
    [Header("ボーナスコインの価格")]
    [SerializeField] private int bonusCoinPrice = 10;
    [SerializeField] private OrdererData ordererData = null;
    [Header("ステージで集める食べ物リスト")]
    [SerializeField] private List<OrderFoodData> orderFoodDataList = new List<OrderFoodData>();
    [Header("難易度1~6")]
    [Range(1, 6)]
    [SerializeField] private int difficulty = 1;
    [Header("ボーナスステージか")]
    [SerializeField] private bool isBonusStage = false;

    /// <summary>ステージID</summary>
    public int Id => id;
    /// <summary>ステージグループID</summary>
    public int GroupId => groupId;

    /// <summary>ステージ名</summary>
    public string StageName => stageName;

    /// <summary>ステージ1欠片の長さ</summary>
    public float StageTipSize => stageTipSize;

    /// <summary>最後のステージ</summary>
    public GameObject GoalStage => goalStage;

    /// <summary>生成するステージのリスト</summary>
    public List<GameObject> StageList => stageList;

    /// <summary>プレイヤーを追従するオブジェクト</summary>
    public GameObject PlayerFollowObject => playerFollowObject;

    /// <summary>曲がるステージかどうか</summary>
    public bool IsCurvedWorld => isCurvedWorld;

    /// <summary>水平に曲げる大きさ</summary>
    public float HorizontalSize => horizontalSize;

    /// <summary>水平に曲げるオフセット</summary>
    public float HorizontalOffset => horizontalOffset;

    /// <summary>垂直に曲げる大きさ</summary>
    public float VerticalSize => verticalSize;

    /// <summary>垂直に曲げるオフセット</summary>
    public float VerticalOffset => verticalOffset;

    /// <summary>通常コインの価格</summary>
    public int NormalCoinPrice => normalCoinPrice;
    /// <summary>ボーナスコインの価格</summary>
    public int BonusCoinPrice => bonusCoinPrice;

    /// <summary>注文者データ</summary>
    public OrdererData OrdererData => ordererData;

    /// <summary>ステージで集める食べ物リスト</summary>
    public List<OrderFoodData> OrderFoodDataList => orderFoodDataList;

    /// <summary>難易度</summary>
    public float Difficulty => difficulty;

    /// <summary>ボーナスステージか</summary>
    public bool IsBonusStage => isBonusStage;

    /// <summary>ゴールまでの距離</summary>
    public float StageDistance => stageTipSize * stageList.Count;
}