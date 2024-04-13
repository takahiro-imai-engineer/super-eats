using System.Collections.Generic;

public class OrderFood
{
    /// <summary>
    /// 食べ物情報
    /// </summary>
    public FoodData FoodData;

    /// <summary>
    /// 必要個数
    /// </summary>
    public int Price;

    /// <summary>
    /// 必要個数
    /// </summary>
    public int NecessaryNum;

    /// <summary>
    /// 所持個数
    /// </summary>
    public int PossessNum = 0;

    /// <summary>
    /// 所持しているか
    /// </summary>
    public bool IsPossess => PossessNum >= NecessaryNum;

    public OrderFood(OrderFoodData orderFoodData)
    {
        this.FoodData = orderFoodData.FoodData;
        this.Price = orderFoodData.Price;
        this.NecessaryNum = orderFoodData.Num;
        this.PossessNum = 0;
    }

    public int GetPrice()
    {
        return this.Price != -1 ? this.Price : this.FoodData.Price;
    }
}