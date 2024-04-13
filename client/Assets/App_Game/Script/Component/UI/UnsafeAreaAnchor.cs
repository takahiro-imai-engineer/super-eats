using UnityEngine;

/// <summary>
/// アンセーフエリア（アスペクト比の幅が規定値を超える画面領域）アンカー調整
/// </summary>
public class UnsafeAreaAnchor : MonoBehaviour
{

    //================================================================================
    // インスペクタ
    //================================================================================

    /// <summary>自身のみを調整するかどうか</summary>
    public bool IsSelfOnly;

    //================================================================================

    /// <summary>左右のパディング</summary>
    private float padding;

    //================================================================================
    // Mono
    //================================================================================

    /// <summary>
    /// 開始
    /// </summary>
    private void Start()
    {
#if !UNITY_EDITOR
        if (UnsafeAreaMask.Instance == null || UnsafeAreaMask.Instance.IsMasked == false)
        {
            return;
        }
#endif
#if false   // MEMO: 20200625動かないのでコメントアウト
        padding = UnsafeAreaMask.Instance.MaskWidth;
#else
        padding = 0;
#endif

        // アンカーリフォーム
        if (IsSelfOnly)
        {

            // キャンバス直下のみ処理
            if (transform.parent.GetComponent<Canvas>() != null)
            {
                anchorReform(transform, padding); // 自身のみ
            }
        }
        else
        {
            // キャンバスがある時のみ処理
            if (GetComponent<Canvas>() != null)
            {
                anchorReform(padding);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 更新
    /// </summary>
    private void Update() {
#if false   // MEMO: 20200625動かないのでコメントアウト
        if( padding != UnsafeAreaMask.Instance.MaskWidth ) {

            // 戻してから実行
            if( IsSelfOnly ) {
                anchorReform( transform, -padding ); // 自身のみ
            }
            else {
                anchorReform( -padding );
            }
            Start();
        }
#endif
    }
#endif

    //================================================================================
    // ローカル
    //================================================================================
    /// <summary>
    /// アンカーリフォーム
    /// </summary>
    /// <param name="size">リフォームサイズ</param>
    private void anchorReform(float size)
    {

        // 直子の RectTransform のみ
        foreach (var childTrans in transform)
        {
            anchorReform((Transform)childTrans, size);
        }
    }

    /// <summary>
    /// アンカーリフォーム
    /// </summary>
    /// <param name="targetTrans">対象の Transform</param>
    /// <param name="size">リフォームサイズ</param>
    private void anchorReform(Transform targetTrans, float size)
    {

        // アンカーの幅がストレッチの RectTransform のサイズを調整する
        if (targetTrans is RectTransform)
        {
            var rectTrans = (RectTransform)targetTrans;

            // 幅がストレッチの場合
            if (rectTrans.anchorMin.x != rectTrans.anchorMax.x)
            {

                // 左右にパディングを入れる
                rectTrans.offsetMin = new Vector2(rectTrans.offsetMin.x + size, rectTrans.offsetMin.y);
                rectTrans.offsetMax = new Vector2(rectTrans.offsetMax.x - size, rectTrans.offsetMax.y);
            }

            // 両端の場合
            else if (rectTrans.anchorMin.x != 0.5f)
            {

                // 左右をずらす
                if (rectTrans.anchorMin.x > 0.5f)
                {
                    size = -size;
                }
                rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x + size, rectTrans.anchoredPosition.y);
            }
        }
    }
}
