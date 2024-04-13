using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// オーバーレイオブジェクト
/// </summary>
public class OverlayObject : MonoBehaviour {

    //================================================================================
    // インスペクタ
    //================================================================================

    /// <summary>表示順。大きいほど手前に表示される</summary>
    public int DisplayOrder = 0;

    [SerializeField]
    bool maskEnabled = true;

    //================================================================================

    /// <summary>属しているキャンバスグループ</summary>
    protected CanvasGroup canvasGroup;

    //================================================================================
    // Mono
    //================================================================================

    /// <summary>
    /// コンストラクタ
    /// </summary>
    protected void Awake() {

        if(maskEnabled)
        {
            // タッチイベントをブロックさせる
            gameObject.AddComponent<GraphicCast>();
        }

        // 属しているキャンバスグループ
        canvasGroup = GetComponentInParent<CanvasGroup>();

        // アンセーフエリア・アンカー調整
        if( gameObject.GetComponent<UnsafeAreaAnchor>() == null ) {
            gameObject.AddComponent<UnsafeAreaAnchor>().IsSelfOnly = true;
        }
    }

    //================================================================================
    // メソッド
    //================================================================================

    /// <summary>
    /// 表示順を変更
    /// </summary>
    /// <param name="displayOrder">表示順</param>
    /// <param name="isApply">直ちに適用するかどうか</param>
    public void ChangeDisplayOrder( int displayOrder, bool isApply = true ) {

        // 表示順
        DisplayOrder = displayOrder;

        // 表示順で再ソート
        if( isApply ) {
            OverlayGroup.Instance.SortInDisplayOrder();
        }
    }
}
