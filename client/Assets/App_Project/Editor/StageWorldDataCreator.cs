using app_system;
using UnityEditor;

/// <summary>
/// ステージグループデータ作成
/// </summary>
public class StageWorldDataCreator
{

    /// <summary>
    /// ダイアログコンテナ作成
    /// </summary>
    [MenuItem("[Game]/Data Asset/Create StageWorldData")]
    public static void CreateStageWorldData()
    {

        // データアセット（ScriptableObject）作成
        DataAssetCreator.Create<StageWorldData>();
    }
}