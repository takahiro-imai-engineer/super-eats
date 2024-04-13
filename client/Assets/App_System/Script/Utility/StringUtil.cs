using System;
using System.Linq;

namespace app_system
{
    /// <summary>
    /// 文字列ユーティリティ
    /// </summary>
    public static class StringUtil {

        //================================================================================
        // メソッド
        //================================================================================

        /// <summary>
        /// 符号付き整数文字列に変換
        /// </summary>
        /// <param name="value">整数値</param>
        /// <returns>符号付き整数文字列</returns>
        public static string ToSigned( int value ) {
            //return value.ToString( "+#,##0;-#,##0" );
            return value.ToString( "+#;-#" );
        }

        /// <summary>
        /// 符号付き整数文字列に変換
        /// </summary>
        /// <param name="value">少数値</param>
        /// <returns>符号付き整数文字列</returns>
        public static string ToSigned( float value ) {
            return value.ToString( "+#.#;-#.#" );
        }

        /// <summary>
        /// スネークケースをアッパーキャメル(パスカル)ケースに変換
        /// </summary>
        public static string SnakeToUpperCamel( string str, string separater = "" ) {
            if( string.IsNullOrEmpty( str ) ) {
                return str;
            }
            return str.Split( new[] { '_' }, StringSplitOptions.RemoveEmptyEntries ).Select( s => char.ToUpperInvariant( s[0] ) + s.Substring( 1, s.Length - 1 ) )
                //.Aggregate( string.Empty, ( s1, s2 ) => s1 + s2 );
                .Aggregate( ( s1, s2 ) => s1 + separater + s2 );
        }

        /// <summary>
        /// スネークケースをローワーキャメル(キャメル)ケースに変換
        /// </summary>
        public static string SnakeToLowerCamel( string str ) {
            if( string.IsNullOrEmpty( str ) ) {
                return str;
            }
            return SnakeToUpperCamel( str ).Insert( 0, char.ToLowerInvariant( str[0] ).ToString() ).Remove( 1, 1 );
        }

        /// <summary>
        /// デリミタでトリミング
        /// </summary>
        /// <param name="delimiter">デリミタ</param>
        /// <returns>トリミングした文字列</returns>
        public static string TrimDelimiter( string str, string delimiter ) {
            var index = str.IndexOf( delimiter );
            if( index < 0 ) {
                return str;
            }
            return str.Substring( index + delimiter.Length );
        }
    }
} // app_system
