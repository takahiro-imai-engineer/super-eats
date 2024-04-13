using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// オーバーレイ UI
/// </summary>
public class OverlayUI : OverlayObject {

    /// <summary>コンテナ（UI の Transform, 元の親の Transform）</summary>
    private Dictionary<Transform, Transform> containers = new Dictionary<Transform, Transform>();

    //================================================================================
    // Mono
    //================================================================================

    /// <summary>
    /// 開始
    /// </summary>
    private void Start() {

        // タッチイベントのブロックをやめる
        var cast = gameObject.GetComponent<GraphicCast>();
        if( cast != null ) {
            cast.enabled = false;
        }
    }

    //================================================================================
    // メソッド
    //================================================================================

    /// <summary>
    /// ON
    /// </summary>
    /// <param name="canvas">キャンバス指定</param>
    public static OverlayUI On( Canvas canvas = null ) {
        if( OverlayGroup.Instance == null ) {
            return null;
        }
        return OverlayGroup.Instance.Show<OverlayUI>( canvas );
    }

    /// <summary>
    /// OFF
    /// </summary>
    public static void Off( bool isReturn = true ) {
        if( OverlayGroup.Instance == null ) {
            return;
        }
        var overlayUi = OverlayGroup.Instance.Get<OverlayUI>();
        if( overlayUi != null && isReturn ) {

            // uI を元の親に戻す
            foreach( var container in overlayUi.containers ) {
                container.Key.SetParent( container.Value, true );
            }
        }
        OverlayGroup.Instance.Dismiss<OverlayUI>();
    }

    /// <summary>
    /// UI 追加
    /// </summary>
    /// <param name="uiTransform">UI の Transform</param>
    public static void AddUI( Transform uiTransform ) {
        var overlayUi = OverlayGroup.Instance.Get<OverlayUI>();
        if( overlayUi != null ) {

            // 既につなげているなら何もしない
            if( overlayUi.containers.ContainsKey( uiTransform ) || uiTransform.parent == overlayUi.transform ) {
                return;
            }

            // コンテナへ
            overlayUi.containers[uiTransform] = uiTransform.parent;

            // つなげる
            uiTransform.SetParent( overlayUi.transform, true );
        }
    }
}
