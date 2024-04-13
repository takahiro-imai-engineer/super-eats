using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using Cysharp.Threading.Tasks;
#if UNITY_ANDROID
using Google.Play.AssetDelivery;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayAssetDeliveryTest : MonoBehaviour
{
    /// <summary>
    /// 進捗率のテキスト
    /// </summary>
    [SerializeField] private Text _progressText;
    [SerializeField] private Image _image;

    /// <summary>
    /// 読み込んだアセットバンドル用変数
    /// </summary>
    private AssetBundle _assetBundle;
    private int currentIndex = 0;
    private Dictionary<string, AssetBundle> playAssetDeliveryAssetBundles = new Dictionary<string, AssetBundle>();

    private void Awake()
    {
#if UNITY_ANDROID
        AllDownload(this.GetCancellationTokenOnDestroy()).Forget();
#endif
    }

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        // LoadAssetBundleAsync("pc_footballers_cardimage_0000_0100", this.GetCancellationTokenOnDestroy()).Forget();
#endif
    }

    private void OnDestroy()
    {

    }

#if UNITY_ANDROID
    /// <summary>
    /// アセットバンドルをロードする
    /// </summary>
    /// <param name="assetBundleName">アセットバンドルの名前</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns></returns>
    private async UniTask LoadAssetBundleAsync(string assetBundleName, CancellationToken ct)
    {
        Debug.Log($"{assetBundleName}のPlayAssetDelivery.IsDownloaded: {PlayAssetDelivery.IsDownloaded(assetBundleName)}");
        _progressText.text = "Start";

        if (playAssetDeliveryAssetBundles.ContainsKey(assetBundleName))
        {
            _assetBundle = playAssetDeliveryAssetBundles[assetBundleName];
        }
        else
        {
            if (!PlayAssetDelivery.IsDownloaded(assetBundleName))
            {
                Debug.LogError($"{assetBundleName}は、未ダウンロードです。");
                _progressText.text = $"{assetBundleName}は、未ダウンロードです。";
                return;
            }
            var bundleRequest = PlayAssetDelivery.RetrieveAssetBundleAsync(assetBundleName);

            // //Wi-Fiの接続をチェック
            // await checkWifiAsync(bundleRequest, ct);

            while (!bundleRequest.IsDone)
            {
                //Wi-Fiの接続をチェック
                await checkWifiAsync(bundleRequest, ct);

                //進捗率
                _progressText.text = bundleRequest.DownloadProgress * 100f + "%";

                await UniTask.Yield(ct);
            }

            if (bundleRequest.Error != AssetDeliveryErrorCode.NoError)
            {
                //何かしらのエラーはここで拾う
                await UniTask.Yield(ct);
            }

            //DLに成功したらアセットバンドルの中身を参照する
            _assetBundle = bundleRequest.AssetBundle;
            playAssetDeliveryAssetBundles.Add(assetBundleName, bundleRequest.AssetBundle);
        }

        var bundleAssets = _assetBundle.GetAllAssetNames();
        var assetQueue = new Queue<string>(bundleAssets);
        string assetName = assetQueue.Dequeue();
        AssetBundleRequest request =
            _assetBundle.LoadAssetAsync<Sprite>(assetName);
        Sprite newSprite = (Sprite)request.asset;
        Debug.Log("Finished load of " + assetName);
        Debug.Log($"{assetBundleName}のPlayAssetDelivery.IsDownloaded: {PlayAssetDelivery.IsDownloaded(assetBundleName)}");

        _image.sprite = newSprite;
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


    private async UniTask AllDownload(CancellationToken ct)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Debug.Log("AllDownload処理開始");
        List<string> assetBundleList = new List<string>()
        {
            "charactericon",
            "foodicon",
            "shopicon",
            "skybox",
            "sound",
            "tips",
            "tutorial_image"
        };
        int downloadAssetNum = 0;
        foreach (var assetBundleName in assetBundleList)
        {
            Debug.Log($"{assetBundleName}のPlayAssetDelivery.IsDownloaded: {PlayAssetDelivery.IsDownloaded(assetBundleName)}");
            var bundleRequest = PlayAssetDelivery.RetrieveAssetBundleAsync(assetBundleName);

            while (!bundleRequest.IsDone)
            {
                //Wi-Fiの接続をチェック
                await checkWifiAsync(bundleRequest, ct);

                //進捗率
                Debug.Log($"{assetBundleName}: {bundleRequest.DownloadProgress * 100f}%");

                await UniTask.Yield(ct);
            }

            if (bundleRequest.Error != AssetDeliveryErrorCode.NoError)
            {
                Debug.LogError($"{assetBundleName}: bundleRequest.Error={bundleRequest.Error}");
                //何かしらのエラーはここで拾う
                await UniTask.Yield(ct);
            }
            Debug.Log($"PlayAssetDeliveryダウンロード完了: {assetBundleName}. 存在判定={bundleRequest.AssetBundle != null}");
            playAssetDeliveryAssetBundles.Add(assetBundleName, bundleRequest.AssetBundle);
            downloadAssetNum++;
        }
        await UniTask.WaitUntil(() => downloadAssetNum == assetBundleList.Count);
        sw.Stop();
        Debug.Log("AllDownload処理時間: " + sw.ElapsedMilliseconds + "ms");
    }

    public void OnClickDownload()
    {
        AllDownload(this.GetCancellationTokenOnDestroy()).Forget();
    }

#endif

    ///ボタン押下時の処理
    public void OnClick()
    {
#if UNITY_ANDROID
        List<string> assetBundleList = new List<string>()
        {
            "charactericon",
            "foodicon",
            "shopicon",
            "skybox",
            "sound",
            "tips",
            "tutorial_image"
        };
        string assetName = "";
        foreach (var item in assetBundleList.Select((Entry, Index) => new { Entry, Index }))
        {
            if (currentIndex == item.Index)
            {
                assetName = item.Entry;
                break;
            }
        }
        Debug.Log(assetName + "を読み込み");
        LoadAssetBundleAsync(assetName, this.GetCancellationTokenOnDestroy()).Forget();
        currentIndex++;
        if (currentIndex >= assetBundleList.Count)
        {
            currentIndex = 0;
        }
#endif
    }
}
