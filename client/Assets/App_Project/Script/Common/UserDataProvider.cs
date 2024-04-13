using app_system;
using template;

/// <summary>
/// ユーザーデータプロバイダ
/// </summary>
public class UserDataProvider : MonoBehaviourSingleton<UserDataProvider>, IGameRegistry
{
    /// <summary>セーブデータ</summary>
    SaveData _saveData;

    //================================================================================
    // ゲームセッティング
    //================================================================================

    public void SetupGameSetting(GameSetting gameSetting)
    {
        _saveData = new SaveData();
        _saveData.Load();
        // データの存在しない場合
        if (!_saveData.IsExist || !_saveData.IsVersionUp)
        {
            _saveData.Init();
            _saveData.IsVersionUp = true;
            WriteSaveData();
        }
    }

    public void Shutdown()
    {
    }

    //================================================================================
    // ユーザデータ
    //================================================================================

    /// <summary>
    /// セーブデータを取得する
    /// </summary>
    public SaveData GetSaveData()
    {
        return _saveData;
    }

    /// <summary>
    /// セーブデータを保存する
    /// </summary>
    public void WriteSaveData()
    {
        _saveData.Save();
    }

    /// <summary>
    /// セーブデータを削除する
    /// </summary>
    public void DeleteSaveData()
    {
        _saveData.Delete();
        _saveData = new SaveData();
        _saveData.Init();
    }

    public int GetNextShotContentId(TitleConstant.ShopItemType shopItemType)
    {
        int currentContentId = shopItemType switch
        {
            TitleConstant.ShopItemType.Bag => _saveData.SelectBagId,
            TitleConstant.ShopItemType.Avatar => _saveData.SelectAvatarId,
            TitleConstant.ShopItemType.Bicycle => _saveData.SelectBicycleId,
            _ => -1
        };
        return AssetManager.Instance.GetShopItemData(shopItemType, currentContentId + 1) != null ? currentContentId + 1 : currentContentId;
    }

    public bool IsTutorialStage()
    {
        return _saveData.TotalStageLevel == 1;
    }
}
