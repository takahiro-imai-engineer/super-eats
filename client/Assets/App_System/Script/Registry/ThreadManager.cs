using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;

namespace app_system
{
	/// <summary>
	/// スレッドマネージャ
	/// </summary>
	public class ThreadManager : MonoBehaviourSingleton<ThreadManager> {

		/// <summary>最大スレッド数</summary>
		[SerializeField]
		private int maxThreadCount = 8;

		/// <summary>実行中のスレッド数</summary>
		private static int currentThreadCount = 0;

		/// <summary>メインスレッド実行キュー</summary>
		private List<UnityAction> safeFuncQueue = new List<UnityAction>();
		private List<UnityAction> safeFuncWorks = new List<UnityAction>();

        //================================================================================
        // プロパティ
        //================================================================================

        /// <summary>
        /// 最大スレッド数
        /// </summary>
        public int MaxThreadCount {
            get { return maxThreadCount; }
            set { maxThreadCount = value; }
        }

        //================================================================================
        // Mono
        //================================================================================
#if !UNITY_WEBGL
        /// <summary>
        /// 更新
        /// </summary>
        void Update() {

			// メインスレッド実行キュー
			lock( safeFuncQueue ) {
				if( safeFuncQueue.Count <= 0 ) {
					return;     // ない
				}

				// ワークへ移す
				safeFuncWorks.Clear();
				safeFuncWorks.AddRange( safeFuncQueue );

				// 実行キュークリア
				safeFuncQueue.Clear();
			}

			// メインスレッドで実行
			foreach( var safeFunc in safeFuncWorks ) {
				safeFunc();
			}
		}
#endif
		//================================================================================
		// メソッド
		//================================================================================

		/// <summary>
		/// スレッド開始
		/// </summary>
		/// <param name="threadFunc">スレッドで実行するメソッド</param>
		/// <param name="onCompleteListener">終了コールバック（メインスレッド）</param>
		public void Run( UnityAction threadFunc, UnityAction onCompleteListener ) {
#if !UNITY_WEBGL
            // スレッド開始
            RunAsync( () => {

				// スレッド中
				threadFunc.Invoke();

				// 終了コールバックはメインスレッドで実行
				RunOnMainThread( () => {
					onCompleteListener.Invoke();
				} );
			} );
#else
            // メインスレッドで実行
			threadFunc.Invoke();
			onCompleteListener.Invoke();
#endif
        }
#if !UNITY_WEBGL
        /// <summary>
        /// スレッド開始
        /// </summary>
        /// <param name="threadFunc">スレッドで実行するメソッド</param>
        public void RunAsync( UnityAction threadFunc ) {

			// 最大スレッド数を超えていたら待つ
			while( currentThreadCount >= maxThreadCount ) {
				Thread.Sleep( 1 );
			}

			// 実行中のスレッド数増やす
			Interlocked.Increment( ref currentThreadCount );

			// 開始
			//ThreadPool.QueueUserWorkItem( delegate( object action ) {
			ThreadPool.QueueUserWorkItem( ( object action ) => {
				try {
					// スレッドメソッド実行
					( ( UnityAction )action ).Invoke();
				}
				catch {
				}
				finally {
					// 実行中のスレッド数減らす
					Interlocked.Decrement( ref currentThreadCount );
				}
			}, threadFunc );
		}

		/// <summary>
		/// メインスレッドで実行
		/// </summary>
		/// <param name="safeFunc">メインスレッドで実行するメソッド</param>
		public void RunOnMainThread( UnityAction safeFunc ) {
			lock( safeFuncQueue ) {

				// メインスレッド実行キューに追加
				safeFuncQueue.Add( safeFunc );
			}
		}
#endif
    }
} // app_system
