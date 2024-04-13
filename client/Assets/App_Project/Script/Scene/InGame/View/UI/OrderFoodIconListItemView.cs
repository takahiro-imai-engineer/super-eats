using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderFoodIconListItemView : FoodIconView
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private Material grayscaleMaterial;
    [SerializeField] private TextMeshProUGUI numText;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="foodData"></param>
    public override void Init(FoodData foodData)
    {
        base.Init(foodData);
        SetPossess(false);
    }

    /// <summary>
    /// 個数を設定
    /// </summary>
    /// <param name="necessaryNum"></param>
    public void SetNum(int necessaryNum)
    {
        numText.gameObject.SetActive(necessaryNum >= 2);
        numText.text = $"x{necessaryNum}";
    }

    /// <summary>
    /// 獲得状態を設定
    /// </summary>
    /// <param name="isPossess"></param>
    public void SetPossess(bool isPossess)
    {
        if (isPossess)
        {
            foodIcon.material = null;
        }
        else
        {
            foodIcon.material = grayscaleMaterial;
        }
    }
}