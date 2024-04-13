using UnityEditor;
using app_system;

/// <summary>
/// ゲームセッティングアセット作成
/// </summary>
public class GameSettingAssetCreator {

    /// <summary>
    /// ゲームセッティング作成
    /// </summary>
    [MenuItem( "[Game]/Data Asset/Create Game Setting" )]
	public static void CreateGameSetting() {

		// データアセット（ScriptableObject）作成
		DataAssetCreator.Create<GameSetting>();
	}
}
