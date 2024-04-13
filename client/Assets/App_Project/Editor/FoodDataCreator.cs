using app_system;
using UnityEditor;

/// <summary>
/// 食べ物データ作成
/// </summary>
public class FoodDataCreator {

    /// <summary>
    /// ダイアログコンテナ作成
    /// </summary>
    [MenuItem ("[Game]/Data Asset/Create FoodData")]
    public static void CreateFoodData () {

        // データアセット（ScriptableObject）作成
        DataAssetCreator.Create<FoodData> ();
    }
}