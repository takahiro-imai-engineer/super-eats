using app_system;
using UnityEditor;

/// <summary>
/// ダイアログアセット作成
/// </summary>
public class ShopContainerCreator
{

    /// <summary>
    /// ダイアログコンテナ作成
    /// </summary>
    [MenuItem("[Game]/Data Asset/Create ShopContainer")]
    public static void CreateShopContainer()
    {

        // データアセット（ScriptableObject）作成
        DataAssetCreator.Create<ShopContainer>();
    }
}