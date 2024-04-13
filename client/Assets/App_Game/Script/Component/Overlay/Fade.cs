using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// フェード
/// </summary>
public class Fade : OverlayObject {

	//================================================================================
	// インスペクタ
	//================================================================================

    /// <summary>フェードイメージ</summary>
    [SerializeField]
    private Image fadeImage;

    /// <summary>フェード時間</summary>
    [SerializeField]
    private float fadeDuration = 0.5f;

	//================================================================================
	// ローカル
	//================================================================================
	/// <summary>フェード回数</summary>
	private int fadeCount = 0;
	/// <summary>ホワイトイン/アウトか</summary>
	private bool isWhite = false;
	/// <summary>フェードインか</summary>
	private bool isIn = false;

    //================================================================================
    // メソッド
    //================================================================================


    /// <summary>
    /// 既存かどうか（フェードアウト後は既存）
    /// </summary>
    public static bool Exist {
        get { return OverlayGroup.Instance.Get<Fade>() != null; }
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    /// <param name="onCompleteListener">終了コールバック</param>
    /// <param name="isWhite">ホワイトインにするかどうか</param>
    /// <param name="duration">フェード時間。指定したい場合</param>
    /// <returns>インスタンス</returns>
    public static Fade FadeIn( UnityAction onCompleteListener = null, bool isWhite = false, float duration = -1.0f, Canvas canvas = null ) {
        return get( canvas ).fade( onCompleteListener, true, isWhite, duration );
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <param name="onCompleteListener">終了コールバック</param>
    /// <param name="isWhite">ホワイトアウトにするかどうか</param>
    /// <param name="duration">フェード時間。指定したい場合</param>
    /// <returns>インスタンス</returns>
    public static Fade FadeOut( UnityAction onCompleteListener = null, bool isWhite = false, float duration = -1.0f, Canvas canvas = null ) {
        return get( canvas ).fade( onCompleteListener, false, isWhite, duration );
    }

    /// <summary>
    /// ホワイトイン
    /// </summary>
    /// <param name="onCompleteListener">終了コールバック</param>
    /// <param name="duration">フェード時間。指定したい場合</param>
    /// <returns>インスタンス</returns>
    public static Fade WhiteIn( UnityAction onCompleteListener = null, float duration = -1.0f, Canvas canvas = null ) {
        return FadeIn( onCompleteListener, true, duration, canvas );
    }

    /// <summary>
    /// ホワイトアウト
    /// </summary>
    /// <param name="onCompleteListener">終了コールバック</param>
    /// <param name="duration">フェード時間。指定したい場合</param>
    /// <returns>インスタンス</returns>
    public static Fade WhiteOut( UnityAction onCompleteListener = null, float duration = -1.0f, Canvas canvas = null ) {
        return FadeOut( onCompleteListener, true, duration, canvas );
    }

    /// <summary>
    /// 消去
    /// </summary>
    public static void Dismiss() {
        var fadeObj = OverlayGroup.Instance.Get<Fade>();
        if( fadeObj != null ) {
            fadeObj.dismiss();
        }
    }

    //================================================================================
    // ローカル
    //================================================================================

    /// <summary>
    /// フェード取得
    /// </summary>
    /// <returns>インスタンス</returns>
    private static Fade get( Canvas canvas ) {

        // 既存のフェードを取得。なければ作成
        var fadeObj = OverlayGroup.Instance.Get<Fade>();
        if( fadeObj == null ) {
            fadeObj = OverlayGroup.Instance.Show<Fade>( canvas );
        }

        return fadeObj;
    }

    /// <summary>
    /// フェード実行
    /// </summary>
    /// <param name="onCompleteListener">終了コールバック</param>
    /// <param name="isIn">インかどうか</param>
    /// <param name="isWhite">ホワイトイン／アウトにするかどうか</param>
    /// <param name="duration">フェード時間</param>
    private Fade fade( UnityAction onCompleteListener, bool isIn, bool isWhite, float duration ) {

        // カラー
        Color color = isWhite ? Color.white : Color.black;

		if (fadeCount > 0 && this.isIn == isIn && this.isWhite == isWhite) {
			// 前回と同じだったら即終了
			// フェード終了
			endFade( onCompleteListener, isIn );
			return this;
		} else {
			color.a = isIn ? 1.0f : 0.0f;
		}
        fadeImage.color = color;

		this.fadeCount++;
		this.isIn = isIn;
		this.isWhite = isWhite;

        // フェード時間
        if( duration < 0 ) {    // インスペクタの時間を使う
            duration = fadeDuration;
        }

        // 即時（DOTween だと遅れる）
        if( duration == 0 ) {
            color.a = isIn ? 0.0f : 1.0f;
            fadeImage.color = color;

            // フェード終了
            endFade( onCompleteListener, isIn );
            return this;
        }

        // フェード実行
        fadeImage.DOKill();
        fadeImage.DOFade( isIn ? 0.0f : 1.0f, duration ).OnComplete( delegate {

            // フェード終了
            endFade( onCompleteListener, isIn );
        } );

        return this;
    }

    /// <summary>
    /// フェード終了
    /// </summary>
    /// <param name="onCompleteListener"></param>
    /// <param name="isIn"></param>
    private void endFade( UnityAction onCompleteListener, bool isIn ) {

        // 終了コールバック
        if( onCompleteListener != null ) {
            onCompleteListener.Invoke();
        }

        // フェードアウトは残す
        if( isIn ) {

            // 表示終了
            OverlayGroup.Instance.Dismiss<Fade>();
        }
    }

    /// <summary>
    /// フェード消去
    /// </summary>
    private void dismiss() {
        fadeImage.DOKill();
        OverlayGroup.Instance.Dismiss<Fade>();
    }
}
