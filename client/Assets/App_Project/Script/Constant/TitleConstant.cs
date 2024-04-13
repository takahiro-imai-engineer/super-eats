[System.Serializable]
public class TitleConstant
{

    /// <summary>
    /// InGameStatus.
    /// </summary>
    public enum InGameStatus
    {
        // 初期化
        Init = 0,
    }

    /// <summary> ボタンタイプ </summary>
    public enum ButtonType
    {
        None = 0,
        Coin,
        Jewel,
        Shop,
        Rank,
        RightTownSelect,
        LeftTownSelect,
        Order,
        Setting,
        WarpNextStage,
    }

    /// <summary> ショップタブタイプ </summary>
    public enum ShopItemType
    {
        None = -1,
        Bag = 0,
        Avatar,
        Bicycle,
    }

    /// <summary> 通貨タイプ </summary>
    public enum CurrencyType
    {
        None = 0,
        Coin,
        Jewel,
    }
}