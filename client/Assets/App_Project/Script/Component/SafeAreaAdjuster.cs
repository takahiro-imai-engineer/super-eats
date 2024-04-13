using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// セーフエリアアジャスター
/// 参考：https://eiki.hatenablog.jp/entry/2020/06/24/192013
/// </summary>
public class SafeAreaAdjuster : MonoBehaviour
{
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// </summary>
    private void Awake()
    {
        var panel = GetComponent<RectTransform>();
        var area = Screen.safeArea;

        var anchorMin = area.position;
        var anchorMax = area.position + area.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;
    }
}