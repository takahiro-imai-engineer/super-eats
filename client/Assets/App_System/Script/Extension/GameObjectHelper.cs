using UnityEngine;

namespace app_system
{
	/// <summary>
	/// GameObject ヘルパー
	/// </summary>
	public static class GameObjectHelper {

        //================================================================================
        // メソッド
        //================================================================================

        /// <summary>
        /// レイヤー設定
        /// </summary>
        /// <param name="layer">レイヤー（0-31）</param>
        public static void SetLayerRecursively( this GameObject gameObj, int layer ) {
            if( gameObj == null ) {
                return;
            }

            // レイヤーを設定
            gameObj.layer = layer;

            // 子も変更していく
            foreach( Transform child in gameObj.transform ) {
                if( child == null ) {
                    continue;
                }
                SetLayerRecursively( child.gameObject, layer );
            }
        }
    }
} // app_system
