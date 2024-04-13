using System.Reflection;
using UnityEngine;

/// <summary>
/// ファイルストレージオブジェクト
/// </summary>
public class FileStorageObject<T> {

	/// <summary>シークレットキー</summary>
	public const string SECRET_KEY = "1D9lknS4";

	/// <summary>ファイル名</summary>
	public string FileName;

	/// <summary>問題が発生した場合にリセットするかどうか</summary>
	protected virtual bool IsReset () { return false; }

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public FileStorageObject (string folderPath = null) {

		// クラス名を SHA256 でハッシュ化
		FileName = CryptoUtil.SHA256Hash (typeof (T).Name, SECRET_KEY);

		// フォルダ指定がある場合
		if (string.IsNullOrEmpty (folderPath) == false) {
			FileName = System.IO.Path.Combine (folderPath, FileName);
		}
	}

	//================================================================================
	// メソッド
	//================================================================================

	/// <summary>
	/// ストレージへ保存
	/// </summary>
	public bool Save () {
		bool isSuccess = false;

		try {

			// Easy Save で保存
			// NOTE: 暗号化はしてない
			var json = JsonUtility.ToJson (this);
			ES3.Save<string> (FileName, json);

			// 成功
			isSuccess = true;
		} catch (System.Exception e) {
			Debug.LogError (e.Message);
		}

		return isSuccess;
	}

	/// <summary>
	/// ストレージから読込
	/// </summary>
	public bool Load () {
		bool isSuccess = false;

		// ファイル名
		var fileName = FileName;

		// ファイルが存在しない
		if (!ES3.KeyExists (fileName)) {
			return false;
		}

		try {
			// Easy Save で読込
			var json = ES3.Load<string> (fileName);
			T obj = JsonUtility.FromJson<T> (json);

			// 各メンバに設定
			FieldInfo[] objFields = typeof (T).GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
			foreach (var objField in objFields) {
				objField.SetValue (this, objField.GetValue (obj));
			}

			// 成功
			isSuccess = true;
		} catch (System.Exception e) {
			if (IsReset ()) {
				Delete ();
			} else {
				Debug.LogError (e.Message);
			}
		}

		return isSuccess;
	}

	/// <summary>
	/// ストレージから削除
	/// </summary>
	public void Delete () {

		// Easy Save で削除
		ES3.DeleteFile (FileName);
	}

	/// <summary>
	/// ストレージに存在するかどうか
	/// </summary>
	public bool IsExist {
		get {
			// Easy Save で存在チェック
			return ES3.KeyExists (FileName);
		}
	}
}