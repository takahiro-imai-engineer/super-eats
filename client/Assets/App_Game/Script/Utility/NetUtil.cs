//#define ENABLE_LOG_SESSION_KEY

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

public class NetUtil {

	private static string url;

	private static string sessionKey;

	private static string requestorId;

	// サーバ通信用ドメイン
	private static string domain = null;


	private static string serverSecretKey;
	public static string ServerSecretKey { get { return serverSecretKey; } }

    public static System.DateTime unixTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

    // ドメインデータを取得する
    public static string GetDomain() {
		return domain;
	}

	public static void SetDomain( string value ) {
		domain = value;

		// ApiRequestUrlを設定
		string protocol = "https://";
		
		if( domain == "192.168.99.100" | domain == "127.0.0.1") {
			protocol = "http://";
		}
		NetUtil.SetApiRequestUrl( protocol + domain + "/api/v1" );
	}

	public static void SetServerSecretKey( string value ) {
		serverSecretKey = value;
	}

	// APIrequest用URLの設定
	public static void SetApiRequestUrl( string url ) {
		NetUtil.url = url;
	}

	// Apiリクエスト用Urlの生成
	public static string GetApiRequestUrl( string url ) {
		return NetUtil.url + url;
	}

	// ユニークKeyを取得する
	public static string GetUniqueId() {
		Guid guidValue = Guid.NewGuid();
		return guidValue.ToString();
	}

	// リクエストをユニークな値
	public static string GetNonce() {

		int length = 16;

		string timeStampString = TimestampUtil.Timestamp().ToString();
		string joinResult = "";
		// 文字列の長さが少ない場合
		if( length > timeStampString.Length ) {
			for( int i = 0; i < ( length - timeStampString.Length ); i++ ) {
				joinResult += "0";
			}
		}
		string resultString = joinResult + TimestampUtil.Timestamp().ToString();
		return resultString;
		/*		
				int length = 16;
				string passwordChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
				StringBuilder sb = new StringBuilder(length);
				for (int i = 0; i < length; i++)
				{
					//文字の位置をランダムに選択
					int pos = UnityEngine.Random.Range(0, passwordChars.Length);

					//選択された位置の文字を取得
					char c = passwordChars[pos];
					//パスワードに追加
					sb.Append(c);
				}

				return sb.ToString();
		*/
	}

	public static string GetAuthToken() {
		return sessionKey;
	}

	// ユーザIDの取得を行う
	public static string GetAuthRequestorId() {
		return requestorId;
	}

	// りくえすたーIDを設定する
	public static void SetAuthRequestorId( string lrequestorId ) {
		requestorId = lrequestorId;
	}

	// RFC3986に準拠したURLエンコードを行う
	public static string EscapeUrl( string url ) {

		url = System.Uri.EscapeDataString( url );

		// RFC2396とRFC3986の差分の吸収を行う
		foreach( Match m in ( new Regex( "[!*'()]" ) ).Matches( url ) ) {
			url = url.Replace( m.Value, "%" + BitConverter.GetBytes( m.Value[0] )[0].ToString( "X" ) );
		}

		return url;
	}

	// ユーザのセッションデータを設定する
	public static void SetSessionKey( string lsessionKey ) {
		sessionKey = lsessionKey;
	}

	// セッションKeyの返却を行う
	public static string GetSessionKey() {
		return sessionKey;
	}

	public static string ConvertMd5( string text ) {

		//文字列をbyte型配列に変換する
		byte[] data = System.Text.Encoding.UTF8.GetBytes( text );

		//MD5CryptoServiceProviderオブジェクトを作成
		System.Security.Cryptography.MD5CryptoServiceProvider md5 =
			new System.Security.Cryptography.MD5CryptoServiceProvider();
		//System.Security.Cryptography.MD5 md5 =
		//    System.Security.Cryptography.MD5.Create();

		//ハッシュ値を計算する
		byte[] bs = md5.ComputeHash( data );

		string result = BitConverter.ToString( bs ).ToLower().Replace( "-", "" );

		return result;
	}

	// 暗号化を行う
	public static string EncryptString( string aesKey, string aesIV, string encryptStringValue ) {

		string code = "";
		// AES暗号化サービスプロバイダ
		RijndaelManaged aes = new RijndaelManaged();
		aes.BlockSize = 128;
		aes.KeySize = 128;
		aes.IV = Encoding.UTF8.GetBytes( aesIV );
		aes.Key = Encoding.UTF8.GetBytes( aesKey );
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.Zeros;

		// 文字列をバイト型配列に変換
		byte[] src = Encoding.UTF8.GetBytes( encryptStringValue );

		// 暗号化する
		using( ICryptoTransform encrypt = aes.CreateEncryptor() ) {
			byte[] dest = encrypt.TransformFinalBlock( src, 0, src.Length );

			// バイト型配列からBase64形式の文字列に変換
			code = Convert.ToBase64String( dest );
		}

		return code;
	}

	// 暗号化を行う
	public static byte[] EncryptBytes( string aesKey, string aesIV, byte[] encryptBytes ) {

		// AES暗号化サービスプロバイダ
		RijndaelManaged aes = new RijndaelManaged();
		aes.BlockSize = 128;
		aes.KeySize = 128;
		aes.IV = Encoding.UTF8.GetBytes( aesIV );
		aes.Key = Encoding.UTF8.GetBytes( aesKey );
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.Zeros;

		// 暗号化する
		using( ICryptoTransform encrypt = aes.CreateEncryptor() ) {
			return encrypt.TransformFinalBlock( encryptBytes, 0, encryptBytes.Length );
		}
	}

	//　復号化
	public static string DecryptString( string aesKey, string aesIV, string decryptStringValue ) {

		// AES暗号化サービスプロバイダ
		RijndaelManaged aes = new RijndaelManaged();
		aes.BlockSize = 128;
		aes.KeySize = 128;
		aes.IV = Encoding.UTF8.GetBytes( aesIV );
		aes.Key = Encoding.UTF8.GetBytes( aesKey );
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.Zeros;

		// 文字列をバイト型配列に変換
		//		byte[] src = Encoding.UTF8.GetBytes (encryptStringValue);

		byte[] strBytes = System.Convert.FromBase64String( decryptStringValue );
		ICryptoTransform decrypt = aes.CreateDecryptor();
		byte[] decBytes = decrypt.TransformFinalBlock( strBytes, 0, strBytes.Length );

		decrypt.Dispose();

		return System.Text.Encoding.UTF8.GetString( decBytes );
		/*		
				// 暗号化する
				using (ICryptoTransform decrypt = aes.CreateDecryptor()) {
					byte[] dest = encrypt.TransformFinalBlock (src, 0, src.Length);

					// バイト型配列からBase64形式の文字列に変換
					code = Convert.ToBase64String (dest);
				}
		*/
		//		return code;
	}

	//　復号化
	public static byte[] DecryptStringToBytes( string aesKey, string aesIV, string decryptStringValue ) {

		// AES暗号化サービスプロバイダ
		RijndaelManaged aes = new RijndaelManaged();
		aes.BlockSize = 128;
		aes.KeySize = 128;
		aes.IV = Encoding.UTF8.GetBytes( aesIV );
		aes.Key = Encoding.UTF8.GetBytes( aesKey );
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.Zeros;

		byte[] strBytes = System.Convert.FromBase64String( decryptStringValue );
		ICryptoTransform decrypt = aes.CreateDecryptor();
		byte[] decBytes = decrypt.TransformFinalBlock( strBytes, 0, strBytes.Length );

		decrypt.Dispose();

		return decBytes;
	}

	//　復号化
	public static byte[] DecryptBytes( string aesKey, string aesIV, byte[] decryptBytes ) {

		// AES暗号化サービスプロバイダ
		RijndaelManaged aes = new RijndaelManaged();
		aes.BlockSize = 128;
		aes.KeySize = 128;
		aes.IV = Encoding.UTF8.GetBytes( aesIV );
		aes.Key = Encoding.UTF8.GetBytes( aesKey );
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.Zeros;

		ICryptoTransform decrypt = aes.CreateDecryptor();
        byte[] decBytes = null;
        try {
            decBytes = decrypt.TransformFinalBlock( decryptBytes, 0, decryptBytes.Length );
        }
        catch( Exception e ) {
            UnityEngine.Debug.LogError( "DecryptBytes Failed : " + e.Message );
        }

		decrypt.Dispose();

		return decBytes;
	}

    /// <summary>
    /// 複合化
    /// </summary>
    public static string DecryptBytesToString( string aesKey, string aesIV, byte[] decryptBytes ) {
        var decBytes = DecryptBytes( aesKey, aesIV, decryptBytes );
        return System.Text.Encoding.UTF8.GetString( decBytes );
    }

    /// <summary>
    /// UnixTimeに変換する。
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long FromDateTime(string dateTimeString) {
        DateTime dateTime = DateTime.Parse(dateTimeString);
        double nowTicks = (dateTime.ToUniversalTime() - unixTime).TotalSeconds;
        return (long)nowTicks;
    }

    /// <summary>
    /// DateTimeに変換する
    /// </summary>
    /// <param name="unixTimeLong"></param>
    /// <returns></returns>
    public static DateTime FromUnixTime(long unixTimeLong) {
        return unixTime.AddSeconds(unixTimeLong).ToLocalTime();
    }
}
