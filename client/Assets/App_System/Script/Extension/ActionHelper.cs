using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace app_system
{
	/// <summary>
	/// アクションヘルパー
	/// </summary>
	public static class ActionHelper {

        //================================================================================
        // メソッド
        //================================================================================

        /// <summary>
        /// 指定時間後にコールバック
        /// 
        /// MonoBehaviour 拡張
        /// </summary>
        /// <param name="waitTime">待ち時間</param>
        /// <param name="callback">コールバック</param>
        /// <returns>コルーチン</returns>
        public static Coroutine TimeToAction( this MonoBehaviour instance, float waitTime, UnityAction callback ) {
			return instance.StartCoroutine( timeToAction( waitTime, callback ) );
		}

        /// <summary>
        /// 指定フレーム後にコールバック
        /// 
        /// MonoBehaviour 拡張
        /// </summary>
        /// <param name="waitFrame">待ちフレーム</param>
        /// <param name="callback">コールバック</param>
        /// <returns>コルーチン</returns>
        public static Coroutine FrameToAction( this MonoBehaviour instance, int waitFrame, UnityAction callback ) {
            return instance.StartCoroutine( frameToAction( waitFrame, callback ) );
		}

        /// <summary>
        /// フレームの最後にコールバック
        /// 
        /// MonoBehaviour 拡張
        /// </summary>
        /// <param name="callback">コールバック</param>
        public static Coroutine FrameEndToAction( this MonoBehaviour instance, UnityAction callback ) {
            return instance.StartCoroutine( frameEndToAction( callback ) );
        }

        /// <summary>
        /// 次のフレームにコールバック
        /// 
        /// MonoBehaviour 拡張
        /// </summary>
        /// <param name="callback">コールバック</param>
        public static Coroutine FrameNextToAction( this MonoBehaviour instance, UnityAction callback ) {
            return instance.StartCoroutine( frameToAction( 1, callback ) );
        }

        //================================================================================
        // ローカル
        //================================================================================

        /// <summary>
        /// 指定時間後にコールバック
        /// </summary>
        /// <param name="waitTime">待ち時間</param>
        /// <param name="callback">コールバック</param>
        private static IEnumerator timeToAction( float waitTime, UnityAction callback ) {
			if( waitTime > 0.0f ) {
				yield return new WaitForSeconds( waitTime );
			}
			callback.Invoke();
		}

		/// <summary>
		/// 指定フレーム後にコールバック
		/// </summary>
		/// <param name="waitFrame">待ちフレーム</param>
		/// <param name="callback">コールバック</param>
		private static IEnumerator frameToAction( int waitFrame, UnityAction callback ) {
			if( waitFrame > 0 ) {
				int endFrame = Time.frameCount + waitFrame;
				while( endFrame > Time.frameCount ) {
					yield return null;
				}
			}
			callback.Invoke();
		}

        /// <summary>
        /// フレームの最後にコールバック
        /// </summary>
        /// <param name="callback">コールバック</param>
        private static IEnumerator frameEndToAction( UnityAction callback ) {
            yield return new WaitForEndOfFrame();
            callback.Invoke();
        }
    }
} // app_system
