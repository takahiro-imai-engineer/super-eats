using UnityEngine;
using System.Linq;

namespace app_system
{
    /// <summary>
    /// 乱数ユーティリティ
    /// </summary>
    public static class RandUtil {

        /// <summary>
        /// 指定した確率（パーセント）の範囲内かどうか
        /// </summary>
        /// <param name="percent">確率（パーセント）</param>
        /// <param name="ratio">100パーセントに対する倍率</param>
        /// <returns>範囲内なら true</returns>
        public static bool IsRange( int percent, int ratio = 1 ) {
            var index = GetRandomIndex( percent, 100 * ratio - percent );
            return index == 0;
        }

        /// <summary>
        /// 均等な重み配列を取得
        /// </summary>
        /// <param name="count">要素数</param>
        /// <param name="weight">1要素の重み</param>
        /// <returns>均等な重み配列</returns>
        public static int[] GetEqualWeights( int count, int weight = 100 ) {
            var weightValues = new int[count];
            for( int i = 0; i < count; i++ ) {
                weightValues[i] = weight;
            }
            return weightValues;
        }

        /// <summary>
        /// 重み配列からそのインデックスをランダムで取得
        /// </summary>
        /// <param name="weightValues">重み配列</param>
        /// <returns>重み配列のインデックス</returns>
        public static int GetRandomIndex( params int[] weightValues ) {

            // 重み合計
            var totalWeight = weightValues.Sum();

            // 1 - 重み合計の乱数値
            var randValue = Random.Range( 1, totalWeight + 1 );

            // 重みで判定
            var index = -1;
            for( int i = 0; i < weightValues.Length; i++ ) {

                if( weightValues[i] >= randValue ) {
                    index = i;
                    break;
                }
                randValue -= weightValues[i];
            }
            if( index < 0 ) {
                Debug.LogError( "RandUtil.GetRandomIndex() Index Error! : " + index );
                index = 0;
            }

            return index;
        }
    }
} // app_system
