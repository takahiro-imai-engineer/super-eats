/*
 * * * * This bare-bones script was auto-generated * * * *
 * The code commented with "/ * * /" demonstrates how data is retrieved and passed to the adapter, plus other common commands. You can remove/replace it once you've got the idea
 * Complete it according to your specific use-case
 * Consult the Example scripts if you get stuck, as they provide solutions to most common scenarios
 * 
 * Main terms to understand:
 *		Model = class that contains the data associated with an item (title, content, icon etc.)
 *		Views Holder = class that contains references to your views (Text, Image, MonoBehavior, etc.)
 * 
 * Default expected UI hiererchy:
 *	  ...
 *		-Canvas
 *		  ...
 *			-MyScrollViewAdapter
 *				-Viewport
 *					-Content
 *				-Scrollbar (Optional)
 *				-ItemPrefab (Optional)
 * 
 * Note: If using Visual Studio and opening generated scripts for the first time, sometimes Intellisense (autocompletion)
 * won't work. This is a well-known bug and the solution is here: https://developercommunity.visualstudio.com/content/problem/130597/unity-intellisense-not-working-after-creating-new-1.html (or google "unity intellisense not working new script")
 * 
 * 
 * Please read the manual under "/Docs", as it contains everything you need to know in order to get started, including FAQ
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;


// There are 2 important callbacks you need to implement, apart from Start(): CreateViewsHolder() and UpdateViewsHolder()
// See explanations below
// Start（）とは別に、実装する必要のある2つの重要なコールバックがあります：CreateViewsHolder（）とUpdateViewsHolder（）です。
// 以下の説明を参照してください
public class ShopListAdapter : OSA<BaseParamsWithPrefab, ShopItemViewsHolder>
{
    [SerializeField] GameObject leftButtonRoot;
    [SerializeField] GameObject rightButtonRoot;

    // Helper that stores data and notifies the adapter when items count changes
    // Can be iterated and can also have its elements accessed by the [] operator
    // データを保存し、アイテム数が変更されたときにアダプタに通知するヘルパー
    // 繰り返すことができ、[]演算子でその要素にアクセスすることもできます
    public SimpleDataHelper<ShopListItemModel> Data { get; private set; }

    private int currentItemIndex = 0;
    private bool isInitialized = false;
    private TitleConstant.ShopItemType currentShopItemType;
    private UnityAction<int> onClickContentCallback = null;
    private UnityAction<int> onPurchaseCallback = null;


    #region OSA implementation
    protected override void Start()
    {
        isInitialized = false;
        Data = new SimpleDataHelper<ShopListItemModel>(this);

        // Calling this initializes internal data and prepares the adapter to handle item count changes
        // これをコールすると、内部データを初期化し、アイテム数の変更を処理するためのアダプタを準備します。
        base.Start();
    }

    private void Update()
    {
        if (!isInitialized)
        {
            return;
        }
        leftButtonRoot.SetActive(_Params.Scrollbar.value >= 0.05f);
        rightButtonRoot.SetActive(_Params.Scrollbar.value <= 0.95f);
    }

    public void SetClickCallback(UnityAction<int> onClickContentCallback, UnityAction<int> onPurchaseCallback)
    {
        this.onClickContentCallback = onClickContentCallback;
        this.onPurchaseCallback = onPurchaseCallback;
    }

    public void ChangeContent(TitleConstant.ShopItemType shopItemType, bool isForceScroll = false)
    {
        bool isScroll = isForceScroll || currentShopItemType != shopItemType;
        currentShopItemType = shopItemType;
        var saveData = UserDataProvider.Instance.GetSaveData();
        int selectContentIndex = 0;
        if (currentShopItemType == TitleConstant.ShopItemType.Avatar)
        {
            selectContentIndex = saveData.SelectAvatarId - 1;
        }
        else if (currentShopItemType == TitleConstant.ShopItemType.Bag)
        {
            selectContentIndex = saveData.SelectBagId - 1;
        }
        else if (currentShopItemType == TitleConstant.ShopItemType.Bicycle)
        {
            selectContentIndex = saveData.SelectBicycleId - 1;
        }

        // Retrieve the models from your data source and set the items count
        // データソースからモデルを取得し、アイテム数を設定する
        var shopItemDataList = AssetManager.Instance.GetShopItemDataList(shopItemType);
        int count = shopItemDataList.Count;
        var newItems = new ShopListItemModel[count];

        // Retrieve your data here
        for (int i = 0; i < count; ++i)
        {
            var model = new ShopListItemModel();
            model.shopItemData = shopItemDataList[i];
            newItems[i] = model;
        }

        if (isInitialized)
        {
            Data.ResetItems(newItems);
            // NOTE: 選択中に移動
            if (isScroll)
            {
                base.ScrollTo(selectContentIndex);
            }
        }
        else
        {
            DG.Tweening.DOVirtual.DelayedCall(0.5f, () =>
            {
                isInitialized = true;
                Data.InsertItemsAtEnd(newItems);
                base.ScrollTo(selectContentIndex);
            });
        }
    }

    // This is called initially, as many times as needed to fill the viewport, 
    // and anytime the viewport's size grows, thus allowing more items to be displayed
    // Here you create the "ViewsHolder" instance whose views will be re-used
    // *For the method's full description check the base implementation
    // これは最初に、ビューポートを満たすために必要な回数だけ呼ばれます。
    // ビューポートのサイズが大きくなり、表示できる項目が増えた場合にも対応可能
    // ここで、ビューを再利用するための "ViewsHolder" インスタンスを作成します。
    // *このメソッドの完全な説明は、基本実装をチェックしてください。
    protected override ShopItemViewsHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new ShopItemViewsHolder();

        // Using this shortcut spares you from:
        // - instantiating the prefab yourself
        // - enabling the instance game object
        // - setting its index 
        // - calling its CollectViews()
        // このショートカットを使用することで、以下の作業を省くことができます。
        // - プレハブのインスタンスを作成する
        // ゲーム・オブジェクトのインスタンスを有効にする
        // インデックスを設定する 
        // - CollectViews() を呼び出す
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

        return instance;
    }

    // This is called anytime a previously invisible item become visible, or after it's created, 
    // or when anything that requires a refresh happens
    // Here you bind the data from the model to the item's views
    // *For the method's full description check the base implementation
    // これは、以前は見えなかったアイテムが見えるようになったとき、またはそれが作られた後にいつでも呼ばれます。
    // あるいは、更新が必要なことが起きたとき
    // ここでは、モデルからアイテムのビューにデータをバインドします。
    // *このメソッドの完全な説明については、基本実装をチェックしてください。
    protected override void UpdateViewsHolder(ShopItemViewsHolder newOrRecycled)
    {
        // In this callback, "newOrRecycled.ItemIndex" is guaranteed to always reflect the
        // index of item that should be represented by this views holder. You'll use this index
        // to retrieve the model from your data set
        // このコールバックでは、"newOrRecycled.ItemIndex" が常に反映されることが保証されています。
        // このビューホルダーによって表現されるべきアイテムの 
        // インデックス。このインデックスを使用します。
        // データセットからモデルを取得するために
        ShopListItemModel model = Data[newOrRecycled.ItemIndex];

        newOrRecycled.Init(model, onClickContentCallback, onPurchaseCallback);
    }

    // This is the best place to clear an item's views in order to prepare it from being recycled, but this is not always needed, 
    // especially if the views' values are being overwritten anyway. Instead, this can be used to, for example, cancel an image 
    // download request, if it's still in progress when the item goes out of the viewport.
    // <newItemIndex> will be non-negative if this item will be recycled as opposed to just being disabled
    // *For the method's full description check the base implementation
    // これはアイテムのビューをクリアして、リサイクルされないように準備するのに最適な場所ですが、これは常に必要なわけではありません。
    // 特に、ビューの値がいずれは上書きされる場合。代わりに、これは例えば、画像 
    // アイテムがビューポートの外に出たときに、ダウンロード要求がまだ進行中であれば、 
    // このアイテムが無効化されるのではなく、リサイクルされる場合、 <newItemIndex> は非負になります。
    // *このメソッドの完全な説明については、基本実装を確認してください。
    protected override void OnBeforeRecycleOrDisableViewsHolder(ShopItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
    {
        base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
    }

    // You only need to care about this if changing the item count by other means than ResetItems, 
    // case in which the existing items will not be re-created, but only their indices will change.
    // Even if you do this, you may still not need it if your item's views don't depend on the physical position 
    // in the content, but they depend exclusively to the data inside the model (this is the most common scenario).
    // In this particular case, we want the item's index to be displayed and also to not be stored inside the model,
    // so we update its title when its index changes. At this point, the Data list is already updated and 
    // shiftedViewsHolder.ItemIndex was correctly shifted so you can use it to retrieve the associated model
    // Also check the base implementation for complementary info
    // ResetItems 以外の方法でアイテム数を変更する場合のみ、これを気にする必要があります。
    // 既存のアイテムは再作成されず、インデックスのみが変更される場合。
    // このような場合でも、アイテムのビューが物理的な位置に依存しないのであれば、必要ないかもしれません。
    // コンテンツ内のデータにのみ依存し、モデル内のデータにのみ依存する（これは最も一般的なシナリオである）。
    // この場合、アイテムのインデックスを表示し、かつ、モデル内部に保存しないようにしたい。
    // インデックスが変更されると、タイトルが更新されます。この時点で、Data listはすでに更新されており 
    // shiftedViewsHolder.ItemIndex は正しくシフトされたので、関連するモデルを取得するためにそれを使用できます。
    // ベースとなる実装の補足情報も確認すること
    protected override void OnItemIndexChangedDueInsertOrRemove(ShopItemViewsHolder shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
    {
        base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

        // shiftedViewsHolder.titleText.text = Data[shiftedViewsHolder.ItemIndex].title + " #" + shiftedViewsHolder.ItemIndex;
    }
    #endregion

    void ScrollTo(int index)
    {
        int numDone = 0;
        SmoothScrollTo(
            itemIndex: index,
            duration: 0.25f,
            normalizedOffsetFromViewportStart: 0.5f,
            normalizedPositionOfItemPivotToUse: 0.5f,
            onProgress: progress =>
             {
                 return true;
             },
            onDone: () =>
             {
                 Debug.Log("スクロール完了");
             },
            overrideCurrentScrollingAnimation: true
        );
    }

    public void OnClickLeftArrow()
    {
        currentItemIndex = _VisibleItems.First().ItemIndex;
        if (currentItemIndex == 0)
        {
            currentItemIndex += 1;
        }
        Debug.Log(currentItemIndex);
        ScrollTo(currentItemIndex - 1);
    }

    public void OnClickRightArrow()
    {
        currentItemIndex = _VisibleItems.Last().ItemIndex;
        if (currentItemIndex == GetItemsCount() - 1)
        {
            currentItemIndex -= 1;
        }
        Debug.Log(currentItemIndex);
        ScrollTo(currentItemIndex + 1);
    }
}

// Class containing the data associated with an item
public class ShopListItemModel
{
    public ShopItemData shopItemData;
}


// This class keeps references to an item's views.
// Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
public class ShopItemViewsHolder : BaseItemViewsHolder
{
    public ShopItemView shopListItem;


    // Retrieving the views from the item's root GameObject
    public override void CollectViews()
    {
        base.CollectViews();

        // root.GetComponentAtPath("Root", out root);
        shopListItem = root.GetComponent<ShopItemView>();
    }

    public void Init(ShopListItemModel model, UnityAction<int> onClickContentCallback, UnityAction<int> onPurchaseCallback)
    {
        shopListItem.Init(model.shopItemData, onClickContentCallback, onPurchaseCallback);
    }
}

