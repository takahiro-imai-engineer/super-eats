using UnityEngine;
using UnityEngine.UI;
using app_system;

/// <summary>
/// アンセーフエリア（アスペクト比の幅が規定値を超える画面領域）マスク
/// 
/// ※横画面のみ対応
/// </summary>
[RequireComponent( typeof( Canvas ), typeof( CanvasScaler ) )]
public class UnsafeAreaMask : MonoBehaviourSingleton<UnsafeAreaMask> {

    //================================================================================
    // インスペクタ
    //================================================================================

    /// <summary>オリジナル解像度での左上座標</summary>
    [SerializeField]
    private RectTransform origineTrans;

    /// <summary>画面左マスク画像</summary>
    [SerializeField]
    private MaskableGraphic leftMask;

    /// <summary>画面右マスク画像</summary>
    [SerializeField]
    private MaskableGraphic RightMask;

    /// <summary>キャンバス</summary>
    [SerializeField]
    private Canvas canvas;

    /// <summary>
    /// キャラクターのイメージ
    /// </summary>
    [SerializeField]
    private GameObject miniCharacter;

    //================================================================================

    /// <summary>マスク画像の幅</summary>
    private float maskWidth;

    //================================================================================
    // Mono
    //================================================================================

    /// <summary>
    /// 開始
    /// </summary>
    private void Start() {

        // オリジナルの左上座標から画面左マスク画像がどれだけずれたか
        var diff = origineTrans.position.x - leftMask.transform.position.x;

        // マスク画像の幅を変更
        maskWidth = diff / canvas.scaleFactor;
        leftMask.rectTransform.sizeDelta = new Vector2( maskWidth, leftMask.rectTransform.sizeDelta.y );
        RightMask.rectTransform.sizeDelta = new Vector2( maskWidth, RightMask.rectTransform.sizeDelta.y );

#if !UNITY_EDITOR
        // マスク画像が必要ない（アスペクト比が 16:9）なら非表示
        if( Mathf.Abs( maskWidth ) < 1.0f ) {
            gameObject.SetActive( false );
        }
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// 更新
    /// </summary>
    private void Update() {
        Start();
    }
#endif

    //================================================================================
    // メソッド
    //================================================================================

    /// <summary>
    /// マスク画像が表示されているかどうか
    /// </summary>
    public bool IsMasked {
        get { return gameObject.activeSelf; }
    }

    /// <summary>
    /// マスク画像の幅
    /// </summary>
    public float MaskWidth {
        get {
            if( IsMasked == false ) {
                return 0;
            }
            return maskWidth;
        }
    }

    /// <summary>
    /// マスク画像のビューポート幅
    /// </summary>
    public float MaskViewPortWidth {
        get {
            if( IsMasked == false ) {
                return 0;
            }
            return ( maskWidth * canvas.scaleFactor ) / Screen.width;
        }
    }
}
