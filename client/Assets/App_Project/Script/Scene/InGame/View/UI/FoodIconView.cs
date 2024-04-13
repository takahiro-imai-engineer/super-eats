using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodIconView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] protected Image foodIcon;
    //================================================================================
    // メソッド
    //================================================================================
    public virtual void Init(FoodData foodData)
    {
        foodIcon.sprite = AssetManager.Instance.LoadFoodIconSprite(foodData.IconName);
    }

    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }
}