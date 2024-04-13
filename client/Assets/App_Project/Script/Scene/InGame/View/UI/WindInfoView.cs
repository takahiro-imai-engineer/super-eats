using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindInfoView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject leftWindRoot;
    [SerializeField] private GameObject rightWindRoot;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        leftWindRoot.SetActive(false);
        rightWindRoot.SetActive(false);
    }

    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="pushVector"></param>
    public void Show(Vector3 pushVector)
    {
        if (pushVector.x > 0)
        {
            rightWindRoot.SetActive(true);
        }
        else
        {
            leftWindRoot.SetActive(true);
        }
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide()
    {
        leftWindRoot.SetActive(false);
        rightWindRoot.SetActive(false);
    }
}
