using UnityEditor;
using app_system;

/// <summary>
/// オーバーレイアセット作成
/// </summary>
public class OverlayAssetCreator {

    /// <summary>
    /// オーバーレイコンテナ作成
    /// </summary>
    [MenuItem( "[Game]/Data Asset/Create Overlay Container" )]
    public static void CreateOverlayContainer() {

		// データアセット（ScriptableObject）作成
		DataAssetCreator.Create<OverlayContainer>();
	}
}
