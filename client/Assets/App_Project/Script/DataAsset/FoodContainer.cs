using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 食べ物コンテナ
/// </summary>
public class FoodContainer : ScriptableObject {
	//================================================================================
	// メンバ変数
	//================================================================================
	/// <summary>データリスト</summary>
	public List<FoodInfo> InfoList = new List<FoodInfo> ();

}

/// <summary>
/// 食べ物情報
/// </summary>
[System.Serializable]
public class FoodInfo {
	public FoodData Data;

	public FoodView Prefab;
}