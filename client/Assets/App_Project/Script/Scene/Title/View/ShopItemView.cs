using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using template;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ShopItemView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject root;
    [SerializeField] private Image background;
    [SerializeField] private Sprite canPurchaseSprite;
    [SerializeField] private Sprite notPurchaseSprite;

    [SerializeField] private Image itemIcon;

    [SerializeField] private GameObject effectRoot;
    [SerializeField] private ItemEffect bagEffect;
    [SerializeField] private ItemEffect avatarEffect;
    [SerializeField] private ItemEffect bicycleEffect;

    [SerializeField] private TextMeshProUGUI needCoinValueText;
    [SerializeField] private RectTransform cursorRoot;

    [System.Serializable]
    class ItemEffect
    {
        public GameObject root;
        public TextMeshProUGUI valueText;
    }
    //================================================================================
    // ローカル
    //================================================================================
    private int contentId = 0;
    private bool isLockButton = false;
    private UnityAction<int> onClickContentCallback = null;
    private UnityAction<int> onPurchaseCallback = null;

    //================================================================================
    // メソッド
    //================================================================================
    public void Init(ShopItemData shopItemData, UnityAction<int> onClickContentCallback, UnityAction<int> onPurchaseCallback)
    {
        SetData(shopItemData);
        isLockButton = false;
        cursorRoot.gameObject.SetActive(false);

        this.onClickContentCallback = onClickContentCallback;
        this.onPurchaseCallback = onPurchaseCallback;
    }

    public void SetData(ShopItemData shopItemData)
    {
        contentId = shopItemData.id;
        itemIcon.sprite = AssetManager.Instance.LoadShopIconSprite(shopItemData.iconName);
        effectRoot.SetActive(true);
        bagEffect.root.SetActive(false);
        avatarEffect.root.SetActive(false);
        bicycleEffect.root.SetActive(false);

        var saveData = UserDataProvider.Instance.GetSaveData();
        bool isBuyable = saveData.MoneyNum >= shopItemData.needCoin;
        bool isPurchased = false;
        if (shopItemData.GetType() == typeof(BagData))
        {
            var bagData = (BagData)shopItemData;
            bagEffect.root.SetActive(true);
            bagEffect.valueText.text = $"Lv.{bagData.magnetLevel}";
            isPurchased = saveData.PurchaseBagIds.Contains(contentId);
        }
        else if (shopItemData.GetType() == typeof(AvatarData))
        {
            var avatarData = (AvatarData)shopItemData;
            avatarEffect.root.SetActive(true);
            avatarEffect.valueText.text = $"+{avatarData.bonusCoin}";
            isPurchased = saveData.PurchaseAvatarIds.Contains(contentId);
        }
        else if (shopItemData.GetType() == typeof(BicycleData))
        {
            var bicycleData = (BicycleData)shopItemData;
            bicycleEffect.root.SetActive(true);
            bicycleEffect.valueText.text = $"{bicycleData.playerLife}";
            isPurchased = saveData.PurchaseBicycleIds.Contains(contentId);
        }

        background.sprite = isBuyable && !isPurchased ? canPurchaseSprite : notPurchaseSprite;
        needCoinValueText.text = shopItemData.needCoin.ToString();
    }

    public void ShowTutorialCursor()
    {
        this.isLockButton = false;
        cursorRoot.gameObject.SetActive(true);
    }

    public void HideTutorialCursor(bool isLockButton = true)
    {
        this.isLockButton = isLockButton;
        cursorRoot.gameObject.SetActive(false);
    }

    public void OnClickItem()
    {
        if (isLockButton) return;
        onClickContentCallback?.Invoke(contentId);
    }

    public void OnClickPurchaseByCoin()
    {
        if (isLockButton) return;
        onPurchaseCallback?.Invoke(contentId);
    }

}
