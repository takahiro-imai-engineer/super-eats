using System.Globalization;
using System.Text.RegularExpressions;

namespace app_system
{
    /// <summary>
    /// Unicode ユーティリティ
    /// </summary>
    public static class UnicodeUtil {

        //================================================================================
        // メソッド
        //================================================================================

        /// <summary>
        /// サロゲート文字（絵文字とか）を取り除く
        /// 
        /// ●Unicode 文字プロパティ
        ///   http://phpspot.net/php/man/php/regexp.reference.unicode.html
        /// ●Unicode 文字種
        ///   http://www.asahi-net.or.jp/~ax2s-kmtn/ref/unicode/emoji.html
        /// </summary>
        /// <param name="text">対象の文字列</param>
        /// <param name="isAdditionalMatchDelegate">追加の条件チェックのデリゲート</param>
        /// <returns>サロゲート文字を削除した文字列</returns>
        public static string TrimSurrogate( string text, System.Predicate<string> isAdditionalMatchDelegate = null ) {
            if( string.IsNullOrEmpty( text ) ) {
                return text;
            }

            // サロゲート文字を削除した文字列
            string trimmedText = "";

            // TextElementEnumerator 取得
            // サロゲートペアや結合文字列でも１文字として扱える
            TextElementEnumerator textEE = StringInfo.GetTextElementEnumerator( text );

            // 読み取る位置を先頭へ
            textEE.Reset();

            // １文字ずつ処理する
            while( textEE.MoveNext() ) {

                // １文字取得
                var textElm = textEE.GetTextElement();

                // サロゲート文字かどうか
                if( isSurrogateCharacter( textElm, textEE ) ) {
                    continue;
                }

                // 追加の条件チェックのデリゲート
                if( isAdditionalMatchDelegate != null ) {
                    if( isAdditionalMatchDelegate( textElm ) ) {
                        continue;
                    }
                }

                // NFC変換。濁点、半濁点の2文字扱い対応
                textElm = textElm.Normalize();

                // サロゲート文字を削除した文字列を作っていく
                trimmedText += textElm;
            }

            // サロゲート文字を削除した文字列
            return trimmedText;
        }

        /// <summary>
        /// サロゲート文字（絵文字とか）が含まれるかどうか
        /// </summary>
        /// <param name="text">対象の文字列</param>
        /// <param name="isAdditionalMatchDelegate">追加の条件チェックのデリゲート</param>
        /// <returns>true ならサロゲート文字が含まれる</returns>
        public static bool IsContainSurrogate( string text, System.Predicate<string> isAdditionalMatchDelegate = null ) {
            if( string.IsNullOrEmpty( text ) ) {
                return false;
            }

            // TextElementEnumerator 取得
            // サロゲートペアや結合文字列でも１文字として扱える
            TextElementEnumerator textEE = StringInfo.GetTextElementEnumerator( text );

            // 読み取る位置を先頭へ
            textEE.Reset();

            // １文字ずつ処理する
            while( textEE.MoveNext() ) {

                // １文字取得
                var textElm = textEE.GetTextElement();

                // サロゲート文字かどうか
                if( isSurrogateCharacter( textElm, textEE ) ) {
                    return true;
                }

                // 追加の条件チェックのデリゲート
                if( isAdditionalMatchDelegate != null ) {
                    if( isAdditionalMatchDelegate( textElm ) ) {
                        return true;
                    }
                }
            }

            // サロゲート文字は含まれない
            return false;
        }

        //================================================================================
        // ローカル
        //================================================================================

        /// <summary>
        /// サロゲート文字かどうか
        /// </summary>
        /// <param name="letter">１文字</param>
        /// <param name="textEE">文字列内の現在のテキスト要素</param>
        /// <returns>true ならサロゲート文字</returns>
        private static bool isSurrogateCharacter( string letter, TextElementEnumerator textEE ) {

            // サロゲート文字のチェック
            {
                // １文字が２以上の Char であれば、サロゲートペアか結合文字列と判断する
                if( letter.Length > 1 ) {
                    return true;
                }

                // StringInfo クラスを使って、サロゲートペアか結合文字が含まれているか調べる
                if( textEE.Current.ToString().Length != new StringInfo( letter ).LengthInTextElements ) {
                    return true;
                }

                // サロゲート文字が含まれているか調べる。"Cs" はサロゲート文字の 文字プロパティ
                if( Regex.IsMatch( letter, @"\p{Cs}" ) ) {
                    return true;
                }

                // 結合文字が含まれているか調べる。"M" は結合文字の Unicode 文字プロパティ
                if( Regex.IsMatch( letter, @"\p{M}" ) ) {
                    return true;
                }
            }

            // 特殊文字のチェック
            {
                //// ↓だと厳しすぎる
                //// 特殊文字が含まれているか調べる。"S" は記号文字の Unicode 文字プロパティ
                //if( Regex.IsMatch( letter, @"\p{S}" ) ) {
                //    return true;
                //}

                // 特殊文字が含まれているか調べる。"2600～26FF" は「装飾や文字として用いられる絵文字」、"2700～27BF" は「装飾用記号フォントに含まれる記号」
                if( Regex.IsMatch( letter, @"[\u2600-\u27BF]" ) ) {
                    return true;
                }
            }

            // サロゲート文字ではない
            return false;
        }
    }
} // app_system
