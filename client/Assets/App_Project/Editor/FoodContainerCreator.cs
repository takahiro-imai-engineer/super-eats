using app_system;
using UnityEditor;

/// <summary>
/// ダイアログアセット作成
/// </summary>
public class FoodContainerCreator {

	/// <summary>
	/// ダイアログコンテナ作成
	/// </summary>
	[MenuItem ("[Game]/Data Asset/Create Food Container")]
	public static void CreateFoodContainer () {

		// データアセット（ScriptableObject）作成
		DataAssetCreator.Create<FoodContainer> ();
	}
}