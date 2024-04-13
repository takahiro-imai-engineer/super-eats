using UnityEngine;

public class TitlePlayerController : MonoBehaviour
{
    /// <summary>ルート</summary>
    [SerializeField] private GameObject root;
    /// <summary>プレイヤーのルート</summary>
    [SerializeField] private GameObject playerRoot;
    /// <summary>プレイヤービュー</summary>
    [SerializeField] private PlayerView playerView;
    [SerializeField] private Transform bagRoot;
    [SerializeField] private Transform bagPosition;
    [SerializeField] private Transform bicycleRoot;
    [SerializeField] private Transform bicyclePosition;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        root.SetActive(true);
        playerView.PlayerEffectView.Init();
        if (GameVariant.Instance.Get<InGameVariant>().IsEndWarpStage)
        {
            playerView.StartEnterStageMotion();
            GameVariant.Instance.Get<InGameVariant>().IsEndWarpStage = false;
        }
        bicycleRoot.position = bicyclePosition.position;
        bicycleRoot.rotation = bicyclePosition.rotation;
    }

    /// <summary>
    /// アバター変更
    /// </summary>
    /// <param name="avatarId"></param>
    public void ChangeAvatar(int avatarId, bool isChangeAnimation = false)
    {
        var avatarData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Avatar, avatarId) as AvatarData;
        playerView.ChangeAvatar(avatarData, isChangeAnimation);
    }

    /// <summary>
    /// バッグ変更
    /// </summary>
    /// <param name="bagId"></param>
    public void ChangeBag(int bagId, bool isChangeAnimation = false)
    {
        var bagData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Bag, bagId) as BagData;
        playerView.ChangeBag(bagData, isChangeAnimation, isTitleScene: true);
    }

    /// <summary>
    /// 自転車変更
    /// </summary>
    /// <param name="bicycleId"></param>
    public void ChangeBicycle(int bicycleId, bool isChangeAnimation = false)
    {
        var bicycleData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Bicycle, bicycleId) as BicycleData;
        playerView.ChangeBicycle(bicycleData, isChangeAnimation);
        playerView.BicycleView.StopAnimation();
    }

    public void ShowBicycleEffect()
    {
        playerView.PlayerEffectView.ShowBicycleEffect();
    }
}
