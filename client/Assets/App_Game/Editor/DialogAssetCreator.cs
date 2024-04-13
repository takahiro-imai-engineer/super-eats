using UnityEditor;
using app_system;

/// <summary>
/// ダイアログアセット作成
/// </summary>
public class DialogAssetCreator {

	/// <summary>
	/// ダイアログコンテナ作成
	/// </summary>
    [MenuItem( "[Game]/Data Asset/Create Dialog Container" )]
    public static void CreateDialogContainer() {

		// データアセット（ScriptableObject）作成
		DataAssetCreator.Create<DialogContainer>();
	}
}
