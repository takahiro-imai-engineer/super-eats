using DG.Tweening;
using template;
using UnityEngine;
using UnityEngine.Events;

public class WarpNextStageView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private PlayerView playerView;
    [SerializeField] private RectTransform endingText;

    //================================================================================
    // メソッド
    //================================================================================
    public void Init()
    {
        playerView.Init();
        endingText.gameObject.SetActive(false);
        var saveData = UserDataProvider.Instance.GetSaveData();
        // バッグ
        var bagData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Bag, saveData.SelectBagId) as BagData;
        playerView.ChangeBag(bagData);

        // アバター
        var avatarData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Avatar, saveData.SelectAvatarId) as AvatarData;
        playerView.ChangeAvatar(avatarData);

        // 自転車
        var bicycleData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Bicycle, saveData.SelectBicycleId) as BicycleData;
        playerView.ChangeBicycle(bicycleData);
        playerView.StartMoveMotion();
    }

    public void MovePlayer(UnityAction onCompleteCallback)
    {
        Sequence sequence = DOTween.Sequence()
            .AppendInterval(4f)
            .AppendCallback(() =>
            {
                SoundManager.Instance.Play<SEContainer>(SEName.SE_DASH);
                playerView.transform.DOMoveZ(
                        100f,
                        20f
                    )
                    .SetSpeedBased()
                    .SetEase(Ease.InQuad)
                    .SetLink(gameObject);
            })
            .AppendInterval(3f)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                onCompleteCallback.Invoke();
            });
    }

    public void ShowEnding(UnityAction onCompleteCallback)
    {

        endingText.anchoredPosition = new Vector2(0, -1800f);
        endingText.gameObject.SetActive(true);
        endingText.DOAnchorPos(
                Vector2.up * 1800f,
                35f
            )
            .SetDelay(2f)
            .SetLink(gameObject);
        DOVirtual.DelayedCall(27f, () =>
        {
            onCompleteCallback.Invoke();
        });
    }
}
