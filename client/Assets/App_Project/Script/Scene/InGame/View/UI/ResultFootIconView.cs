using System.Collections;
using TMPro;
using UnityEngine;

public class ResultFootIconView : FoodIconView
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private Material grayscaleMaterial;
    [SerializeField] private TextMeshProUGUI numText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject failedLabelRoot;
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
        failedLabelRoot.SetActive(false);
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
    /// 値段を設定
    /// </summary>
    /// <param name="price"></param>
    public void SetPrice(int price)
    {
        priceText.text = $"{price}$";
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

    /// <summary>
    /// 失敗ラベルを表示
    /// </summary>
    public void ShowFailedLabel()
    {
        failedLabelRoot.SetActive(true);
    }
}