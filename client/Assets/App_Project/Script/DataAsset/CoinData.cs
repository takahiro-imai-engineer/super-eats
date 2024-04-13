using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// コインデータ
/// </summary>
[CreateAssetMenu(menuName = "[Game]/Data Asset/Create CoinData", fileName = "CoinData")]
public class CoinData : ScriptableObject
{

    [Header("名前")]
    [SerializeField] private string name = "Coin";

    [Header("値段(ワールド毎に設定)")]
    [SerializeField] private List<CoinPrice> priceList = new List<CoinPrice>();

    [System.Serializable]
    public class CoinPrice
    {
        public int difficulty1 = 1;
        public int difficulty2 = 2;
        public int difficulty3 = 3;
        public int difficulty4 = 4;
        public int difficulty5 = 5;
    }

    /// <summary>名前</summary>
    public string Name
    {
        get { return name; }
    }

    /// <summary>値段</summary>
    public List<CoinPrice> PriceList
    {
        get { return priceList; }
    }

    /// <summary>
    /// ワールドIDと難易度からコインの価格を取得
    /// </summary>
    /// <param name="selectStageWorldId"></param>
    /// <returns></returns>
    public int GetPrice(int selectStageWorldId, int difficulty)
    {
        if (priceList.Count < selectStageWorldId)
        {
            Debug.LogError($"ワールドID={selectStageWorldId}。{name}の価格が設定されていません。");
            return 0;
        }
        var coinPrice = priceList[selectStageWorldId - 1];
        return difficulty switch
        {
            1 => coinPrice.difficulty1,
            2 => coinPrice.difficulty2,
            3 => coinPrice.difficulty3,
            4 => coinPrice.difficulty4,
            5 => coinPrice.difficulty5,
            _ => 0
        };
    }
}