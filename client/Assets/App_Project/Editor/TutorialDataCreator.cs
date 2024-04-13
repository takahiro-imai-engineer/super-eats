using app_system;
using UnityEditor;

/// <summary>
/// チュートリアルデータ作成
/// </summary>
public class TutorialDataCreator
{

    /// <summary>
    /// ダイアログコンテナ作成
    /// </summary>
    [MenuItem("[Game]/Data Asset/Create TutorialData")]
    public static void CreateTutorialData()
    {

        // データアセット（ScriptableObject）作成
        DataAssetCreator.Create<TutorialData>();
    }
}