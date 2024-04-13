using app_system;
using UnityEditor;

/// <summary>
/// 注文者データ作成
/// </summary>
public class OrdererDataCreator {

    /// <summary>
    /// ダイアログコンテナ作成
    /// </summary>
    [MenuItem ("[Game]/Data Asset/Create OrdererData")]
    public static void CreateFoodData () {

        // データアセット（ScriptableObject）作成
        DataAssetCreator.Create<OrdererData> ();
    }
}