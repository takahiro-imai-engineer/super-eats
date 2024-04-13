using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneBase : MonoBehaviour {
	/// <summary>インスタンス</summary>
	private static SceneBase instance;

	/// <summary>インスタンス</summary>
	public static SceneBase Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<SceneBase> ();
			}
			return instance;
		}
	}
	//================================================================================
	// インスペクタ
	//================================================================================
	/// <summary>ゲームセッティング</summary>
	[SerializeField]
	protected GameSetting gameSetting;

	//================================================================================
	// メソッド
	//================================================================================
	/// <summary>
	/// コンストラクタ
	/// </summary>
	protected void Awake () {

		// 既存なら削除
		if (Instance != this) {
			Destroy (this);
		}
		// ゲームセッティングのセットアップ
		StartCoroutine (setupGameSetting ());
	}

	/// <summary>
	/// ゲームセッティングのセットアップ
	/// </summary>
	private IEnumerator setupGameSetting () {

		// ゲームレジストリが有効になるまで待つ
		while (GameRegistry.Instance == null) {
			yield return null;
		}

		// ゲームセッティングのセットアップ
		GameRegistry.Instance.SetupGameSetting (gameSetting); // ゲームセッティング取得
	}

	/// <summary>
	/// デストラクタ
	/// </summary>
	protected void OnDestroy () {
		instance = null;
	}
}