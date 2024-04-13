using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TitleBaseView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] List<GameObject> defaultLevelObjectList;
    [SerializeField] List<FurnitureView> firstLevelObjectList;
    [SerializeField] List<FurnitureView> secondLevelObjectList;
    [SerializeField] List<FurnitureView> thirdLevelObjectList;

    //================================================================================
    // ローカル変数
    //================================================================================
    private int currentStep = 0;

    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="step"></param>
    public void Init(int step)
    {
        currentStep = step;
        foreach (var item in defaultLevelObjectList)
        {
            item.SetActive(true);
        }
        foreach (var item in firstLevelObjectList)
        {
            item.Init();
            if (step >= 1)
            {
                item.Show(isPurchase: false);
            }
        }
        foreach (var item in secondLevelObjectList)
        {
            item.Init();
            if (step >= 2)
            {
                item.Show(isPurchase: false);
            }
        }
        foreach (var item in thirdLevelObjectList)
        {
            item.Init();
            if (step >= 3)
            {
                item.Show(isPurchase: false);
            }
        }
    }

    /// <summary>
    /// 拠点レベルアップ表示
    /// </summary>
    /// <param name="step"></param>
    /// <param name="onCompleteCallback"></param>
    public void ShowLevelUp(int step, UnityAction onCompleteCallback)
    {
        if (currentStep == step)
        {
            onCompleteCallback?.Invoke();
            return;
        }
        currentStep = step;
        var furnitureList = step switch
        {
            1 => firstLevelObjectList,
            2 => secondLevelObjectList,
            3 => thirdLevelObjectList,
            _ => null
        };

        if (furnitureList == null)
        {
            Debug.LogError($"家具の取得に失敗: step={step}");
            onCompleteCallback?.Invoke();
            return;
        }

        const float FURNITURE_ENTRY_INTERVAL = 0.3f;
        Sequence sequence = DOTween.Sequence();
        foreach (var furniture in furnitureList)
        {
            sequence.AppendCallback(() =>
            {
                furniture.Show(isPurchase: true);
            })
            .AppendInterval(FURNITURE_ENTRY_INTERVAL);
        }
        sequence
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                onCompleteCallback?.Invoke();
            });
    }
}
