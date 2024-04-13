using System.Collections;
using UnityEngine.Events;

namespace template
{
	/// <summary>
	/// カスタムダイアログ
	/// </summary>
	public class CustomDialog : GeneralDialog {

		//================================================================================
		// Mono
		//================================================================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected new void Awake() {
			base.Awake();

			// 何かあれば
		}

		//================================================================================
		// メソッド
		//================================================================================

		//--------------------------------------------------------------------------------
		// アニメーション
		//--------------------------------------------------------------------------------

		/// <summary>
		/// Enter アニメ
		/// </summary>
		public override IEnumerator EnterAnimation() {
            yield return StartCoroutine( base.EnterAnimation() );
        }

        /// <summary>
        /// Exit アニメ
        /// </summary>
        public override IEnumerator ExitAnimation( UnityAction onCompleteListener ) {
            yield return StartCoroutine( base.ExitAnimation( onCompleteListener ) );
		}
	}
} // template
