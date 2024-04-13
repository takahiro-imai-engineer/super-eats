using System.Text;
using System.Security.Cryptography;

/// <summary>
/// 暗号化ユーティリティ
/// </summary>
public class CryptoUtil {

	/// <summary>
	/// SHA256 ハッシュ化
	/// </summary>
	/// <param name="value">ハッシュ化する値</param>
	/// <param name="secretKey">シークレットキー</param>
	/// <returns></returns>
	public static string SHA256Hash( string value, string secretKey ) {

		// 値とキーをバイトデータに変換
		UTF8Encoding encoding = new UTF8Encoding();
		byte[] bytes = encoding.GetBytes( value );
		byte[] keyBytes = encoding.GetBytes( secretKey );

		// SHA256 でハッシュ化
		HMACSHA256 sha256 = new HMACSHA256( keyBytes );
		byte[] hashBytes = sha256.ComputeHash( bytes );	// 32 バイト

		// 16進文字列に変換
		string hashedValue = "";	// 64 文字
		foreach( byte b in hashBytes ) {
			hashedValue += string.Format( "{0,0:x2}", b );
		}

		return hashedValue;
	}
}
