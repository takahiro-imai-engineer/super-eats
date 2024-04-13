using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using app_system;
using Cysharp.Threading.Tasks;
#if UNITY_ANDROID
using Google.Play.AssetDelivery;
#endif

public class AssetManager : MonoBehaviourSingleton<AssetManager>, IGameRegistry
{
    /// <summary>食べ物コンテナのリスト</summary>
    [SerializeField] private FoodContainer foodContainer;

    /// <summary>ショップコンテナのリスト</summary>
    [SerializeField] private ShopContainer shopContainer;

    //================================================================================
    // ローカル
    //================================================================================
    private Dictionary<string, StageData> loadStageDatas = new Dictionary<string, StageData>();

    /// <summary>PlayAssetDeliveryでダウンロードしたアセットバンドルのリスト</summary>
    private Dictionary<string, AssetBundle> loadAssetBundles = new Dictionary<string, AssetBundle>();

    //================================================================================
    // プロパティ
    //================================================================================
    public List<BagData> BagDataList => shopContainer.BagDataList;
    public List<AvatarData> AvatarDataList => shopContainer.AvatarDataList;
    public List<BicycleData> BicycleDataList => shopContainer.BicycleDataList;
    public bool IsLoadedAssetBundle { get; private set; } = false;

    //================================================================================
    // ゲームセッティング
    //================================================================================
    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting(GameSetting gameSetting)
    {

    }

    public async UniTask SetUpAssetBundle()
    {
        if (IsLoadedAssetBundle)
        {
            Debug.Log($"AssetBundle 読み込み済み");
            return;
        }
#if UNITY_ANDROID
        var baseAssetBundlePath = $"{Application.streamingAssetsPath}/Android";
#elif UNITY_IOS
        var baseAssetBundlePath = $"{Application.streamingAssetsPath}/iOS";
#else
        var baseAssetBundlePath = "";
#endif

        string[] assetBundles = new string[8]
        {
            "title_base",
            "charactericon",
            "foodicon",
            "shopicon",
            "skybox",
            "sound",
            "tips",
            "tutorial_image"
        };
        int downloadAssetNum = 0;
        foreach (var item in assetBundles)
        {
            if (loadAssetBundles.ContainsKey(item))
            {
                // 既にダウンロード済みの場合、スキップ
                downloadAssetNum++;
                continue;
            }
            var assetBundle = await AssetBundle.LoadFromFileAsync(Path.Combine(baseAssetBundlePath, item));
            loadAssetBundles.Add(item, assetBundle);
            downloadAssetNum++;
        }
        IsLoadedAssetBundle = true;
    }

#if UNITY_ANDROID
    public async UniTask SetUpPlayAssetDelivery(CancellationToken ct)
    {
        if (IsLoadedAssetBundle)
        {
            Debug.Log($"AssetBundle 読み込み済み");
            return;
        }
        Debug.Log($"PlayAssetDeliveryダウンロード開始");
        string[] assetBundles = new string[8]
        {
            "title_base",
            "charactericon",
            "foodicon",
            "shopicon",
            "skybox",
            "sound",
            "tips",
            "tutorial_image"
        };
        int downloadAssetNum = 0;
        var gameLoadingScreen = OverlayGroup.Instance.Get<GameLoadingScreen>();
        foreach (var assetBundleName in assetBundles)
        {
            Debug.Log($"{assetBundleName}のPlayAssetDelivery.IsDownloaded: {PlayAssetDelivery.IsDownloaded(assetBundleName)}");
            if (loadAssetBundles.ContainsKey(assetBundleName))
            {
                // 既にダウンロード済みの場合、スキップ
                continue;
            }
            var bundleRequest = PlayAssetDelivery.RetrieveAssetBundleAsync(assetBundleName);

            while (!bundleRequest.IsDone)
            {
                //Wi-Fiの接続をチェック
                await checkWifiAsync(bundleRequest, ct);

                //進捗率
                Debug.Log($"{assetBundleName}: {bundleRequest.DownloadProgress * 100f}%");
                Debug.LogWarning($"PlayAssetDelivery Status={bundleRequest.Status}");
                Debug.LogWarning($"PlayAssetDelivery Error={bundleRequest.Error}");

                await UniTask.Yield(ct);
            }

            if (bundleRequest.Error != AssetDeliveryErrorCode.NoError)
            {
                Debug.LogError($"{assetBundleName}: bundleRequest.Error={bundleRequest.Error}");
                //何かしらのエラーはここで拾う
                await UniTask.Yield(ct);
            }
            Debug.Log($"PlayAssetDeliveryダウンロード完了: {assetBundleName}. 存在判定={bundleRequest.AssetBundle != null}");
            loadAssetBundles.Add(assetBundleName, bundleRequest.AssetBundle);
            downloadAssetNum++;
            if (gameLoadingScreen != null)
            {
                gameLoadingScreen.SetProgress(downloadAssetNum / (float)assetBundles.Length);
            }
        }
        await UniTask.WaitUntil(() => downloadAssetNum == assetBundles.Length);
        Debug.Log($"PlayAssetDeliveryダウンロード終了");
        IsLoadedAssetBundle = true;
    }

    /// <summary>
    /// Wi-Fiの接続をチェックする
    /// </summary>
    /// <param name="playAssetBundleRequest">リクエストしたアセットバンドル</param>
    /// <param name="ct">キャンセルトークン</param>
    private async UniTask checkWifiAsync(PlayAssetBundleRequest playAssetBundleRequest, CancellationToken ct)
    {
        //アセットバンドルが150MB以上の場合、Wi-Fiの接続が前提となる　接続が無い場合はユーザーにWi-Fi無しでDLしても良いか確認する。
        //150MB超えているかどうかは自動で判定
        if (playAssetBundleRequest.Status == AssetDeliveryStatus.WaitingForWifi)
        {
            //Wi-Fi無しでDLしても良いか確認
            var userConfirmationOperation = PlayAssetDelivery.ShowCellularDataConfirmation();

            //ユーザーの入力を待つ
            await userConfirmationOperation.ToUniTask(cancellationToken: ct).AttachExternalCancellation(ct);

            if ((userConfirmationOperation.Error != AssetDeliveryErrorCode.NoError) ||
                (userConfirmationOperation.GetResult() != ConfirmationDialogResult.Accepted))
            {
                // ユーザーが拒否した時の処理
            }

            // Wi-Fiに接続された、もしくは"未接続でも可"が承認されるのを待つ
            await UniTask.WaitWhile(() => playAssetBundleRequest.Status != AssetDeliveryStatus.WaitingForWifi, cancellationToken: ct);
        }
    }
#endif

    //================================================================================
    // CharacterData
    //================================================================================
    public Sprite LoadCharacterIconSprite(string characterIconName)
    {
        if (!loadAssetBundles.ContainsKey("charactericon"))
        {
            Debug.LogError($"Failed to load AssetBundle! characterIconName={characterIconName}");
            return null;
        }

        // テクスチャをメモリにロード
        var sprite = loadAssetBundles["charactericon"].LoadAsset<Sprite>(characterIconName);

#if UNITY_EDITOR
        if (sprite == null)
        {
            Debug.LogError($"NotFound AssetBundle foodIconName={characterIconName}");
        }
#endif

        return sprite;
    }

    //================================================================================
    // FoodData
    //================================================================================
    public FoodInfo GetFoodInfo(int foodId)
    {
        var foodInfo = foodContainer.InfoList.Find(d => d.Data.Id == foodId);
        if (foodInfo == null)
        {
            Debug.LogError("食べ物情報の取得に失敗");
            return null;
        }
        return foodInfo;
    }

    public Sprite LoadFoodIconSprite(string foodIconName)
    {
        if (!loadAssetBundles.ContainsKey("foodicon"))
        {
            Debug.LogError($"Failed to load AssetBundle! foodIconName={foodIconName}");
            return null;
        }

        // テクスチャをメモリにロード
        var sprite = loadAssetBundles["foodicon"].LoadAsset<Sprite>(foodIconName);

#if UNITY_EDITOR
        if (sprite == null)
        {
            Debug.LogError($"NotFound AssetBundle foodIconName={foodIconName}");
        }
#endif
        return sprite;
    }

    //================================================================================
    // ShopData
    //================================================================================
    public ShopItemData GetShopItemData(TitleConstant.ShopItemType shopItemType, int contentId)
    {
        ShopItemData shopItemData = null;
        if (shopItemType == TitleConstant.ShopItemType.Bag)
        {
            shopItemData = BagDataList.FirstOrDefault(d => d.id == contentId) as ShopItemData;
            return shopItemData;
        }
        else if (shopItemType == TitleConstant.ShopItemType.Avatar)
        {
            shopItemData = AvatarDataList.FirstOrDefault(d => d.id == contentId) as ShopItemData;
            return shopItemData;
        }
        else if (shopItemType == TitleConstant.ShopItemType.Bicycle)
        {
            shopItemData = BicycleDataList.FirstOrDefault(d => d.id == contentId) as ShopItemData;
            return shopItemData;
        }
        else
        {
            return shopItemData;
        }
    }

    public List<ShopItemData> GetShopItemDataList(TitleConstant.ShopItemType shopItemType)
    {
        var shopItemDataList = new List<ShopItemData>();
        if (shopItemType == TitleConstant.ShopItemType.Bag)
        {
            shopItemDataList.AddRange(BagDataList);
            return shopItemDataList;
        }
        else if (shopItemType == TitleConstant.ShopItemType.Avatar)
        {
            shopItemDataList.AddRange(AvatarDataList);
            return shopItemDataList;
        }
        else if (shopItemType == TitleConstant.ShopItemType.Bicycle)
        {
            shopItemDataList.AddRange(BicycleDataList);
            return shopItemDataList;
        }
        else
        {
            return shopItemDataList;
        }
    }

    public Sprite LoadShopIconSprite(string shopIconName)
    {
        if (!loadAssetBundles.ContainsKey("shopicon"))
        {
            Debug.LogError($"Failed to load AssetBundle! shopIconName={shopIconName}");
            return null;
        }

        // テクスチャをメモリにロード
        var sprite = loadAssetBundles["shopicon"].LoadAsset<Sprite>(shopIconName);

#if UNITY_EDITOR
        if (sprite == null)
        {
            Debug.LogError($"NotFound AssetBundle shopIconName={shopIconName}");
        }
#endif
        return sprite;
    }


    //================================================================================
    // StageData
    //================================================================================
    public StageData LoadStageData(string stageName)
    {
        if (!loadStageDatas.ContainsKey(stageName))
        {
            var stageData = Resources.Load<StageData>($"StageData/{stageName}");
            if (stageData == null)
            {
                Debug.LogError(stageName.ToString());
            }
            loadStageDatas.Add(stageName, stageData);
        }

        return loadStageDatas[stageName];
    }

    public void ReleaseStageAsset()
    {
        loadStageDatas.Clear();
        GameRegistry.MemoryClean(isGC: true);
    }

    //================================================================================
    // Tutorial
    //================================================================================
    public Sprite LoadTutorialImageSprite(string tutorialImageName)
    {
        if (!loadAssetBundles.ContainsKey("tutorial_image"))
        {
            Debug.Log($"Failed to load AssetBundle! tutorialImageName={tutorialImageName}");
            return null;
        }

        // テクスチャをメモリにロード
        var sprite = loadAssetBundles["tutorial_image"].LoadAsset<Sprite>(tutorialImageName);
#if UNITY_EDITOR
        if (sprite == null)
        {
            Debug.LogError($"NotFound AssetBundle tutorialImageName={tutorialImageName}");
        }
#endif
        return sprite;
    }

    //================================================================================
    // Tips
    //================================================================================
    public Sprite LoadTipsSprite(string tipsName)
    {
        if (!loadAssetBundles.ContainsKey("tips"))
        {
            Debug.Log($"Failed to load AssetBundle! tipsName={tipsName}");
            return null;
        }

        // テクスチャをメモリにロード
        var sprite = loadAssetBundles["tips"].LoadAsset<Sprite>(tipsName);
#if UNITY_EDITOR
        if (sprite == null)
        {
            Debug.LogError($"NotFound AssetBundle tipsName={tipsName}");
        }
#endif
        return sprite;
    }

    //================================================================================
    // 拠点
    //================================================================================
    public TitleBaseView LoadBase(string baseName)
    {
        if (!loadAssetBundles.ContainsKey("title_base"))
        {
            Debug.Log($"Failed to load AssetBundle! baseName={baseName}");
            return null;
        }

        // オブジェクトをメモリにロード
        var titleBase = loadAssetBundles["title_base"].LoadAsset<GameObject>(baseName);

        if (titleBase == null)
        {
#if UNITY_EDITOR
            Debug.LogError($"NotFound AssetBundle baseName={baseName}");
#endif
            return null;
        }


        return titleBase.GetComponent<TitleBaseView>();
    }

    //================================================================================
    // SkyBox
    //================================================================================
    public Material LoadSkyboxMaterial(string skyboxMaterialName)
    {
        if (!loadAssetBundles.ContainsKey("skybox"))
        {
            Debug.Log($"Failed to load AssetBundle! skyboxMaterialName={skyboxMaterialName}");
            return null;
        }

        var material = loadAssetBundles["skybox"].LoadAsset<Material>(skyboxMaterialName);
        Shader s = Shader.Find("Skybox/Cubemap");
        if (material && s)
        {
            material.shader = s;
        }
#if UNITY_EDITOR
        if (material == null)
        {
            Debug.LogError($"NotFound AssetBundle skyboxMaterialName={skyboxMaterialName}");
        }
#endif
        return material;
    }

    //================================================================================
    // AudioClip
    //================================================================================
    public AudioClip GetAudioClip(string soundName)
    {
        if (!loadAssetBundles.ContainsKey("sound"))
        {
            Debug.Log($"Failed to load AssetBundle! soundName={soundName}");
            return null;
        }

        // AudioClipをメモリにロード
        var audioClip = loadAssetBundles["sound"].LoadAsset<AudioClip>(soundName);
#if UNITY_EDITOR
        if (audioClip == null)
        {
            Debug.LogError($"NotFound AssetBundle soundName={soundName}");
        }
#endif
        return audioClip;
    }

    /// <summary>
    /// シャットダウン
    /// 
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    public void Shutdown()
    {

    }
}