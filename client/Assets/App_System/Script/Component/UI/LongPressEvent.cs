using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace app_system
{
    /// <summary>
    /// 長押しイベント
    /// 
    /// Button コンポーネント等と同じようにオブジェクトにアタッチして使う
    /// </summary>
    public class LongPressEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {

        //==========================================================================
        // インスペクタ
        //==========================================================================

        /// <summary>長押し時間</summary>
        public float Duration = 1.0f;

        /// <summary>イベント</summary>
        public UnityEvent OnLongPress = new UnityEvent();

		/// <summary>イベント</summary>
		public UnityEvent OnRelease = new UnityEvent();

        //==========================================================================

        /// <summary>時間コールバックのコルーチン</summary>
        private Coroutine coroutine;

        /// <summary>選択可能な UI コンポーネント</summary>
        private Selectable selectable;

        //==========================================================================
        // Mono
        //==========================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private void Awake() {

            // 選択可能な UI コンポーネント
            selectable = GetComponent<Selectable>();
        }

        //==========================================================================
        // イベントハンドラ
        //==========================================================================

        /// <summary>
        /// ダウン
        /// </summary>
        public void OnPointerDown( PointerEventData eventData ) {

            // 長押し時間後にコールバック実行
            coroutine = this.TimeToAction( Duration, delegate {

                // 実行
                execute();

                // 他の選択状態を解除する
                if( selectable != null ) {
                    //selectable.OnDeselect( eventData );   // これだとうまくいかないので↓
                    selectable.OnPointerUp( eventData );
                    eventData.pointerPress = null;
                }
            } );
        }

        /// <summary>
        /// アップ
        /// </summary>
        public void OnPointerUp( PointerEventData eventData ) {
            // キャンセル
            cancel();
        }

        /// <summary>
        /// アウト
        /// </summary>
        public void OnPointerExit( PointerEventData eventData ) {
            // キャンセル
            cancel();
        }

        //==========================================================================
        // ローカル
        //==========================================================================

        /// <summary>
        /// 実行
        /// </summary>
        private void execute() {
            OnLongPress.Invoke();
            coroutine = null;
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        private void cancel() {
            if( coroutine != null ) {
                StopCoroutine( coroutine );
                coroutine = null;
            }
			OnRelease.Invoke();
        }
    }
} // app_system

