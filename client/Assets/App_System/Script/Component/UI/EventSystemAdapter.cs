using UnityEngine;
using UnityEngine.EventSystems;

namespace app_system
{
	/// <summary>
	/// EventSystem のアダプタ
	/// EventSystem の補正や調整、拡張などを行う
	/// 
	/// EventSystem オブジェクトにアタッチして使う
	/// </summary>
	public class EventSystemAdapter : MonoBehaviour {

		//==========================================================================
		// インスペクタ
		//==========================================================================

		/// <summary>EventSystem オブジェクト</summary>
		[SerializeField]
		private EventSystem eventSystem;

		/// <summary>基準となる DPI</summary>
		/* 主要な解像度と DPI
			 480 x  800 : 240
			 540 x  960 : 240
			 720 x 1280 : 320
			1080 x 1920 : 480
			1440 x 2560 : 640
		*/
		[SerializeField]
		private int baseDpi = 240;

		/// <summary>端末の DPI と基準となる DPI の比率に掛けるドラッグ係数。大きくするほどドラッグ判定が甘くなるが、ScrollRect 内のボタン等は押しやすくなる</summary>
		[SerializeField]
		private float dpiDragScaleFactor = 7.0f;

		//==========================================================================
		// Mono
		//==========================================================================

		/// <summary>
		/// リセット
		/// エディターモードでインスペクタにデフォルト値を設定する
		/// </summary>
		void Reset() {
			if( eventSystem == null ) {
				eventSystem = GetComponent<EventSystem>();
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		void Awake() {

            // マルチタッチ無効
            Input.multiTouchEnabled = false;

            // DPI に合わせて、ドラッグ判定のしきい値を設定する
            // ※小さいくせにやたらと解像度が高い端末は、DPI が大きいのでちょっと触っただけでドラッグ判定となってしまうので、しきい値を大きくする
            SetPixelDragThreshold( Screen.dpi );

#if UNITY_WEBGL
            // RectMask2D が WebGL で機能しないための処置
            Canvas.GetDefaultCanvasMaterial().EnableKeyword( "UNITY_UI_CLIP_RECT" );
#if !UNITY_EDITOR
            /*
                すべてのキーボード入力をキャプチャーします。
                このプロパティーはキーボードからの入力が WebGL によってキャプチャされたかどうかを決定します。これが有効な場合 (デフォルト)、フォーカスに関係なく WebGL キャンバスによってすべての入力が受信され、
                WEB ページの他のエレメントはキーボードからの入力を受信しません。他の HTML 入力エレメントに入力が必要な場合、このプロパティーを無効にする必要があります。
            */
            WebGLInput.captureAllKeyboardInput = false;
#endif
#endif
        }

        //==========================================================================
        // メソッド
        //==========================================================================

        /// <summary>
        /// ドラッグ判定のしきい値（ピクセル単位でドラッグするためのソフトエリア）を設定
        /// </summary>
        /// <param name="screenDpi">DPI</param>
        public void SetPixelDragThreshold( float screenDpi ) {
			if( eventSystem != null ) {
				if( screenDpi == 0 ) {
					screenDpi = baseDpi;   // DPI が取れなくて 0 の場合
				}
				eventSystem.pixelDragThreshold = Mathf.RoundToInt( screenDpi / baseDpi * dpiDragScaleFactor );
			}
		}
	}
} // app_system
