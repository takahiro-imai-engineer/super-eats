using app_system;
using UnityEditor;

/// <summary>
/// ステージデータ作成
/// </summary>
public class StageDataCreator {

    /// <summary>
    /// ダイアログコンテナ作成
    /// </summary>
    [MenuItem ("[Game]/Data Asset/Create StageData")]
    public static void CreateStageData () {

        // データアセット（ScriptableObject）作成
        DataAssetCreator.Create<StageData> ();
    }
}