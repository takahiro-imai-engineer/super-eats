using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 吹っ飛ばしギミッククラス
/// </summary>
public class BlowGimickView : GimmickView
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [Header("ルート")]
    /// <summary>ルート</summary>
    [SerializeField] private GameObject root;
    [Header("吹っ飛びパスのタイプ(固定/移動)")]
    /// <summary>BlowType</summary>
    [SerializeField] private InGameConstant.BlowType blowType = InGameConstant.BlowType.Immovable;
    [Header("吹っ飛ぶルート")]
    /// <summary>パスルート</summary>
    [SerializeField] private GameObject blowPathRoot;
    [Header("時間経過による動き方")]
    [SerializeField] private Ease ease = Ease.Linear;
    [Header("移動時間")]
    [SerializeField] private float durationTime = 1f;
    //================================================================================
    // ローカル
    //================================================================================
    private bool isActive = false;
    private Vector3[] movePathList;
    //================================================================================
    // プロパティ
    //================================================================================
    public Ease Ease => ease;
    public float DurationTime => durationTime;
    //================================================================================
    // メソッド
    //================================================================================

    public Vector3[] GetBlowPathList()
    {
        var pathList = blowPathRoot.GetComponentsInChildren<Transform>().Where(c => blowPathRoot != c.gameObject).ToList();
        var blowPathList = new Vector3[pathList.Count];

        for (int i = 0; i < pathList.Count; i++)
        {
            if (blowType == InGameConstant.BlowType.Immovable)
            {
                // 吹っ飛ぶパスが固定の場合
                blowPathList[i] = pathList[i].position;
            }
            else if (blowType == InGameConstant.BlowType.Moving)
            {
                // 吹っ飛ぶパスがオブジェクトに追従する場合
                blowPathList[i] = this.transform.position + pathList[i].localPosition;
            }
            Debug.Log(blowPathList[i]);
        }
        movePathList = blowPathList;
        return blowPathList;
    }

    /// <summary>
    /// ギミック開始
    /// </summary>
    private void ActiveGimick()
    {
        isActive = true;
    }

    private void HitEvent()
    {
        if (isActive)
        {
            return;
        }
        else
        {
            ActiveGimick();
            Debug.Log("ぶっ飛びギミック起動");
        }
    }
}