using app_system;
using UnityEditor;

/// <summary>
/// ショップアイテムデータ作成
/// </summary>
public class ShopItemDataCreator
{

    [MenuItem("[Game]/Data Asset/ShopData/Create AvatarData")]
    public static void CreateAvatarData()
    {

        // データアセット（ScriptableObject）作成
        DataAssetCreator.Create<AvatarData>();
    }

    [MenuItem("[Game]/Data Asset/ShopData/Create BagData")]
    public static void CreateBagData()
    {

        // データアセット（ScriptableObject）作成
        DataAssetCreator.Create<BagData>();
    }

    [MenuItem("[Game]/Data Asset/ShopData/Create BicycleData")]
    public static void CreateBicycleData()
    {

        // データアセット（ScriptableObject）作成
        DataAssetCreator.Create<BicycleData>();
    }
}