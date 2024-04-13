using UnityEngine;

namespace app_system
{
    /// <summary>
    /// Math ユーティリティ
    /// </summary>
    public static class MathUtil {

        //================================================================================
        // メソッド
        //================================================================================

        /// <summary>
        /// 線と線の交点を求める
        /// </summary>
        /// <param name="intersection">交点</param>
        /// <param name="linePoint1"></param>
        /// <param name="lineVec1"></param>
        /// <param name="linePoint2"></param>
        /// <param name="lineVec2"></param>
        /// <returns>交差していれば true</returns>
        public static bool LineLineIntersection( out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2 ) {

            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross( lineVec1, lineVec2 );
            Vector3 crossVec3and2 = Vector3.Cross( lineVec3, lineVec2 );

            float planarFactor = Vector3.Dot( lineVec3, crossVec1and2 );

            //is coplanar, and not parrallel
            if( Mathf.Abs( planarFactor ) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f ) {
                float s = Vector3.Dot( crossVec3and2, crossVec1and2 ) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + ( lineVec1 * s );
                return true;
            }
            else {
                intersection = Vector3.zero;
                return false;
            }
        }

        //--------------------------------------------------------------------------------
        // DPI
        //--------------------------------------------------------------------------------

        /// <summary>
        /// サイズを DPI サイズに変換
        /// </summary>
        /// <param name="size">サイズ</param>
        /// <returns>DPI サイズ</returns>
        public static int SizeToDpi( int size ) {
            return size / Mathf.RoundToInt( Screen.dpi / 160 );
        }

        /// <summary>
        /// DPI サイズをサイズに変換
        /// </summary>
        /// <param name="dpiSize">DPI サイズ</param>
        /// <returns>サイズ</returns>
        public static int DpiToSize( int dpiSize ) {
            return dpiSize * Mathf.RoundToInt( Screen.dpi / 160 );
        }

        //--------------------------------------------------------------------------------
        // イージング
        //--------------------------------------------------------------------------------

        /// <summary>
        /// InQuad
        /// </summary>
        /// <param name="start">開始値</param>
        /// <param name="end">終了値</param>
        /// <param name="time">経過時間（0-1）</param>
        /// <returns>経過時間での値</returns>
        public static float EaseInQuad( float start, float end, float time ) {
            end -= start;
            return end * time * time + start;
        }

        /// <summary>
        /// OutQuad
        /// </summary>
        /// <param name="start">開始値</param>
        /// <param name="end">終了値</param>
        /// <param name="time">経過時間（0-1）</param>
        /// <returns>経過時間での値</returns>
        public static float EaseOutQuad( float start, float end, float time ) {
            end -= start;
            return -end * time * ( time - 2 ) + start;
        }

        /// <summary>
        /// OutQuart
        /// </summary>
        /// <param name="start">開始値</param>
        /// <param name="end">終了値</param>
        /// <param name="time">経過時間（0-1）</param>
        /// <returns>経過時間での値</returns>
        public static float EaseOutQuart( float start, float end, float time ) {
            time--;
            end -= start;
            return -end * ( time * time * time * time - 1 ) + start;
        }

        /// <summary>
        /// InOutQuad
        /// </summary>
        /// <param name="start">開始値</param>
        /// <param name="end">終了値</param>
        /// <param name="time">経過時間（0-1）</param>
        /// <returns>経過時間での値</returns>
        public static float EaseInOutQuad( float start, float end, float value ) {
            value /= .5f;
            end -= start;
            if( value < 1 )
                return end / 2 * value * value + start;
            value--;
            return -end / 2 * ( value * ( value - 2 ) - 1 ) + start;
        }

        /// <summary>
        /// InOutQuint
        /// </summary>
        /// <param name="start">開始値</param>
        /// <param name="end">終了値</param>
        /// <param name="time">経過時間（0-1）</param>
        /// <returns>経過時間での値</returns>
        public static float EaseInOutQuint( float start, float end, float value ) {
            value /= .5f;
            end -= start;
            if( value < 1 )
                return end / 2 * value * value * value * value * value + start;
            value -= 2;
            return end / 2 * ( value * value * value * value * value + 2 ) + start;
        }

#if UNITY_EDITOR
        //--------------------------------------------------------------------------------
        // Stats
        //--------------------------------------------------------------------------------

        /// <summary>
        /// 頂点数とトライアングル数を計算
        /// </summary>
        /// <param name="rootTrans">ルート</param>
        /// <param name="vertexCount">頂点数。出力</param>
        /// <param name="triangleCount">トライアングル数。出力</param>
        public static void CalcVertexAndTriangleCount( Transform rootTrans, out int vertexCount, out int triangleCount ) {
            vertexCount = 0;
            triangleCount = 0;
            if( rootTrans == null ) {
                return;
            }

            var transforms = rootTrans.GetComponentsInChildren<Transform>();
            foreach( var trnas in transforms ) {
                if( trnas.gameObject.activeInHierarchy == false ) {
                    continue;
                }

                // スキン
                var smr = trnas.GetComponent<SkinnedMeshRenderer>();
                if( smr != null && smr.enabled ) {
                    // vertexCount += smr.sharedMesh.vertices.Length;
                    // triangleCount += smr.sharedMesh.triangles.Length / 3;
                    vertexCount += smr.sharedMesh.vertexCount;
                    // triangleCount += smr.sharedMesh.triangles.Length / 3;
                    continue;
                }

                // メッシュ
                var meshfilter = trnas.GetComponent<MeshFilter>();
                var renderer = trnas.GetComponent<MeshRenderer>();
                if( meshfilter != null && renderer.enabled ) {
                    // vertexCount += meshfilter.sharedMesh.vertices.Length;
                    // triangleCount += meshfilter.sharedMesh.triangles.Length / 3;
                    vertexCount += meshfilter.mesh.vertexCount;
                    // triangleCount += meshfilter.mesh.triangles.Length / 3;
                }
            }
        }
#endif
    }
} // app_system
