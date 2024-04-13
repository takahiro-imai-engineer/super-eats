using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TitleShopView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] List<ShopTab> shopTabList;
    [System.Serializable]
    class ShopTab
    {
        public Button activeTab;
        public Button inactiveTab;
        public GameObject badgeRoot;
    }
    [SerializeField] ShopListAdapter shopListAdapter;

    //================================================================================
    // ローカル
    //================================================================================
    TitleConstant.ShopItemType currentShopItemType;
    UnityAction<TitleConstant.ShopItemType> onChangeContentCallback = null;
    UnityAction closeCallback = null;

    //================================================================================
    // メソッド
    //================================================================================
    public void Init(UnityAction<TitleConstant.ShopItemType> onChangeContentCallback, UnityAction closeCallback, UnityAction<TitleConstant.ShopItemType, int> onClickContentCallback, UnityAction<TitleConstant.ShopItemType, int> onPurchaseCallback)
    {
        currentShopItemType = TitleConstant.ShopItemType.Bag;
        this.onChangeContentCallback = onChangeContentCallback;
        this.closeCallback = closeCallback;
        shopListAdapter.SetClickCallback(
            (int contentId) => onClickContentCallback?.Invoke(currentShopItemType, contentId),
            (int contentId) => onPurchaseCallback?.Invoke(currentShopItemType, contentId)
        );
        shopListAdapter.ChangeContent(currentShopItemType);
    }

    public void Show()
    {
        currentShopItemType = TitleConstant.ShopItemType.Bag;
        SelectTab(currentShopItemType, isForceScroll: true);
    }

    public void SelectTab(TitleConstant.ShopItemType selectShopItemType, bool isForceScroll = false)
    {
        currentShopItemType = selectShopItemType;
        for (int i = 0; i < shopTabList.Count; i++)
        {
            if (i == (int)currentShopItemType)
            {
                shopTabList[i].activeTab.gameObject.SetActive(true);
                shopTabList[i].inactiveTab.gameObject.SetActive(false);
            }
            else
            {
                shopTabList[i].activeTab.gameObject.SetActive(false);
                shopTabList[i].inactiveTab.gameObject.SetActive(true);
            }
        }

        var saveData = UserDataProvider.Instance.GetSaveData();
        if (currentShopItemType == TitleConstant.ShopItemType.Avatar)
        {
            TitleScene.Variant.SelectContentId = saveData.SelectAvatarId;
        }
        else if (currentShopItemType == TitleConstant.ShopItemType.Bag)
        {
            TitleScene.Variant.SelectContentId = saveData.SelectBagId;
        }
        else if (currentShopItemType == TitleConstant.ShopItemType.Bicycle)
        {
            TitleScene.Variant.SelectContentId = saveData.SelectBicycleId;
        }

        shopListAdapter.ChangeContent(currentShopItemType, isForceScroll);
        onChangeContentCallback.Invoke(currentShopItemType);
    }

    public void UpdateShop()
    {
        shopListAdapter.ChangeContent(currentShopItemType);
    }

    public void OnClickTab(int tabId)
    {
        SelectTab((TitleConstant.ShopItemType)tabId);
    }

    public void UpdateTabBadge(TitleConstant.ShopItemType selectShopItemType, bool isShowBadge)
    {
        shopTabList[(int)selectShopItemType].badgeRoot.SetActive(isShowBadge);
    }

    public void OnClickBackButton()
    {
        closeCallback?.Invoke();
    }
}
