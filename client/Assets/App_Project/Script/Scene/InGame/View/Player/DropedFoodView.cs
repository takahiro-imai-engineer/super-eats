using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 落とす食べ物の描画クラス
/// </summary>
public class DropedFoodView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject root;
    //================================================================================
    // ローカル
    //================================================================================
    //================================================================================
    // プロパティ
    //================================================================================
    //================================================================================
    // 定数
    //================================================================================
    const float DROP_FOOD_FORCE_POWER = 20f;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        root.SetActive(false);
    }

    /// <summary>
    /// 表示
    /// </summary>
    public void Show()
    {
        root.SetActive(true);
    }

    /// <summary>
    /// 食べ物を落とす
    /// </summary>
    /// <param name="foodId"></param>
    public void DropFood(int foodId)
    {
        var foodInfo = AssetManager.Instance.GetFoodInfo(foodId);
        if (foodInfo == null)
        {
            Debug.LogError($"食べ物の取得に失敗：{foodId}");
            return;
        }
        if (foodInfo.Prefab == null)
        {
            Debug.LogError($"食べ物のプレハブの取得に失敗{foodId}");
            return;
        }
        var dropFood = Instantiate(foodInfo.Prefab, this.transform.position, Quaternion.identity);
        var dropPower = (Vector3.up + Vector3.right) * DROP_FOOD_FORCE_POWER;
        dropFood.Drop(dropPower);
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide()
    {

        root.SetActive(false);
    }
}