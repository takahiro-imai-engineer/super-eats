using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using app_system;

/// <summary>
/// サウンドマネージャ
/// </summary>
public class SoundManager : MonoBehaviourSingleton<SoundManager>, IGameRegistry
{

    /// <summary>
    /// オーディオチャンネル
    /// </summary>
    internal class Audiochannel
    {

        /// <summary>ミュート</summary>
        public bool IsMute = false;

        /// <summary>ボリューム</summary>
        public float MasterVolume = 1.0f;

        /// <summary>オーディオソース（チャンネル）リスト</summary>
        public List<AudioSource> AudioSources = new List<AudioSource>();
    }

    /// <summary>再生チャンネルのリスト</summary>
    private Dictionary<System.Type, Audiochannel> channelsList = new Dictionary<System.Type, Audiochannel>();

    /// <summary>クロスフェード処理のコルーチン</summary>
    private Coroutine crossFadeRoutine = null;

    //================================================================================
    // ゲームセッティング
    //================================================================================

    /// <summary>サウンドコンテナのリスト</summary>
    [SerializeField]
    private List<SoundContainer> soundContainers;

    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting(GameSetting gameSetting)
    {

        // 常駐サウンドアセットのリストをサウンドコンテナのリストへ設定
        soundContainers = new List<SoundContainer>(gameSetting.ResidentialSoundAssetList);

        // 再生チャンネルの割り当て
        foreach (var container in soundContainers)
        {

            // 常駐にする
            container.IsResidential = true;

            // AudioSource（チャンネル） を作成
            var audioChannel = new Audiochannel();
            for (int i = 0; i < container.MaxChannelCount; i++)
            {
                audioChannel.AudioSources.Add(gameObject.AddComponent<AudioSource>());
            }

            // 作成した AudioSource（チャンネル） をサウンドコンテナのタイプ別に再生チャンネルのリストへ登録
            System.Type type = container.GetType();     // サウンドコンテナのタイプ
            if (channelsList.ContainsKey(type))
            {
                // Debug.LogError(string.Format("{0} is already exist!", type));
                continue;
            }
            channelsList.Add(type, audioChannel);
        }
    }

    /// <summary>
    /// シャットダウン
    /// 
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    public void Shutdown()
    {

        // オーディオクリップをクリア
        foreach (var audioChannel in channelsList.Values)
        {
            audioChannel.AudioSources.ForEach(audioSource => audioSource.clip = null);
        }

        // 常駐以外のサウンドコンテナを削除
        soundContainers.RemoveAll(container =>
        {
            if (container == null)
            {
                return true;
            }

            // 常駐
            if (container.IsResidential)
            {
                return false;
            }

            // 削除
            Remove(container.ContainerName);
            return true;
        });
    }

    //================================================================================
    // メソッド
    //================================================================================

    /// <summary>
    /// サウンドが存在するかどうか
    /// </summary>
    /// <typeparam name="T">サウンドコンテナのタイプ</typeparam>
    /// <param name="name">サウンド名</param>
    /// <returns>存在するかどうか</returns>
    public bool IsExist<T>(string name, bool isCareClip = false) where T : SoundContainer
    {
        return findAudioResource<T>(name, isCareClip) != null;
    }

    /// <summary>
    /// ギミックSEを取得
    /// </summary>
    /// <param name="gimickSE"></param>
    /// <returns></returns>
    public AudioResource GetGimickSE(InGameConstant.GimickSE gimickSE)
    {
        string name = gimickSE
        switch
        {
            InGameConstant.GimickSE.CarHorn => SEName.SE_CAR_HORN,
            InGameConstant.GimickSE.Silen => SEName.SE_SIREN,
            InGameConstant.GimickSE.PoliceCar => SEName.SE_POLICE_CAR,
            InGameConstant.GimickSE.TrafficJam => SEName.SE_TRAFFIC_JAM,
            InGameConstant.GimickSE.ConcreteFall => SEName.SE_CONCRETE_FALL,
            InGameConstant.GimickSE.CarIn => SEName.SE_CAR_COMING,
            InGameConstant.GimickSE.RivalIn => SEName.SE_RIVAL_IN,
            InGameConstant.GimickSE.BoxFall => SEName.SE_BOX_FALL,
            InGameConstant.GimickSE.ContainerMove => SEName.SE_CONTAINER_MOVE,
            InGameConstant.GimickSE.ContainerFall => SEName.SE_CONTAINER_FALL,
            InGameConstant.GimickSE.TruckMove => SEName.SE_TRUCK_MOVE,
            InGameConstant.GimickSE.TruckHorn => SEName.SE_TRUCK_HORN,
            InGameConstant.GimickSE.HoverCarFall => SEName.SE_HOVER_CAR_FALL,
            InGameConstant.GimickSE.EnemyDrone => SEName.SE_ENEMY_DRONE,
            InGameConstant.GimickSE.RivalDrone => SEName.SE_RIVAL_DRONE,
            InGameConstant.GimickSE.BigShipAlarm => SEName.SE_BIG_SHIP_ALARM,
            InGameConstant.GimickSE.BigShipEngine => SEName.SE_BIG_SHIP_ENGINE,
            InGameConstant.GimickSE.DoorClose => SEName.SE_DOOR_CLOSE,
            InGameConstant.GimickSE.CyberCarIn => SEName.SE_CYBER_CAR_IN,
            InGameConstant.GimickSE.CrossHover => SEName.SE_CROSS_HOVER,
            InGameConstant.GimickSE.HoverCarStart => SEName.SE_HOVER_CAR_START,
            InGameConstant.GimickSE.Robot => SEName.SE_ROBOT,
            InGameConstant.GimickSE.TimeToWarp => SEName.SE_TIME_TO_WARP,
            _ => ""
        };

        AudioResource playResource = findAudioResource<template.SEContainer>(name);

        if (playResource == null)
        {
            Debug.LogWarning("Sound resource not found. " + name);
            return null;
        }

        return playResource;
    }

    //--------------------------------------------------------------------------------
    // 再生／停止
    //--------------------------------------------------------------------------------

    /// <summary>
    /// 再生
    /// </summary>
    /// <typeparam name="T">サウンドコンテナのタイプ</typeparam>
    /// <param name="name">サウンド名</param>
    /// <param name="isStopPlaying">再生中のサウンドを停止するかどうか</param>
    /// <param name="fadeDuration">フェード時間</param>
    /// <param name="onCompleteListener">フェード終了コールバック</param>
    /// <param name="skipTime">途中再生時の時間（秒）</param>
    /// <returns>再生チャンネル</returns>
    public AudioSource Play<T>(string name, bool isStopPlaying = false, float fadeDuration = 0, UnityAction onCompleteListener = null, float skipTime = 0) where T : SoundContainer
    {

        // フェードイン再生の場合
        if (fadeDuration > 0)
        {
            return CrossFade<T>(name, null, fadeDuration, onCompleteListener);
        }

        // 対象のオーディオチャンネルを取得
        Audiochannel audioChannel = getAudiochannel<T>();
        if (audioChannel == null)
        {
            Debug.LogWarning("Sound container type not found. " + typeof(T));
            return null;   // サウンドコンテナのタイプがない
        }

        // 再生するオーディオリソースを取得
        AudioResource playResource = findAudioResource<T>(name);
        if (playResource == null)
        {
            Debug.LogWarning("Sound resource not found. " + name);
            return null;
        }

        // 再生中のサウンドを停止する
        if (isStopPlaying)
        {
            Stop<T>(fadeDuration: fadeDuration);
        }

        // 再生するチャンネルを取得
        var playChannel = audioChannel.AudioSources.FirstOrDefault(channel => channel.isPlaying == false);
        if (playChannel == null)
        {
            Debug.LogWarning("Sound channel no empty. " + name);
            return null;   // チャンネルに空がない
        }

        // 再生
        //if( playChannel.clip != null ) {
        //    playChannel.clip.UnloadAudioData();
        //}
        playChannel.clip = AssetManager.Instance.GetAudioClip(playResource.Name);
        playChannel.loop = playResource.IsLoop;
        playChannel.time = skipTime;
        playChannel.Play();

        return playChannel;
    }

    /// <summary>
    /// ギミックSE再生
    /// </summary>
    /// <param name="gimickSE"></param>
    /// <param name="isStopPlaying"></param>
    /// <param name="fadeDuration"></param>
    /// <param name="onCompleteListener"></param>
    /// <param name="skipTime"></param>
    /// <returns></returns>
    public AudioSource PlayGimickSE(InGameConstant.GimickSE gimickSE, bool isStopPlaying = false, float fadeDuration = 0, UnityAction onCompleteListener = null, float skipTime = 0)
    {

        // 対象のオーディオチャンネルを取得
        Audiochannel audioChannel = getAudiochannel<template.SEContainer>();
        if (audioChannel == null)
        {
            Debug.LogWarning("Sound container type not found. " + typeof(template.SEContainer));
            return null;   // サウンドコンテナのタイプがない
        }

        // 再生するオーディオリソースを取得
        AudioResource playResource = GetGimickSE(gimickSE);

        if (playResource == null)
        {
            Debug.LogWarning("Sound resource not found. " + name);
            return null;
        }

        // 再生中のサウンドを停止する
        if (isStopPlaying)
        {
            Stop<template.SEContainer>(fadeDuration: fadeDuration);
        }

        // 再生するチャンネルを取得
        var playChannel = audioChannel.AudioSources.FirstOrDefault(channel => channel.isPlaying == false);
        if (playChannel == null)
        {
            Debug.LogWarning("Sound channel no empty. " + name);
            return null;   // チャンネルに空がない
        }

        // 再生
        //if( playChannel.clip != null ) {
        //    playChannel.clip.UnloadAudioData();
        //}
        playChannel.clip = AssetManager.Instance.GetAudioClip(playResource.Name);
        playChannel.loop = playResource.IsLoop;
        playChannel.time = skipTime;
        playChannel.Play();

        return playChannel;
    }

    /// <summary>
    /// 停止
    /// </summary>
    /// <typeparam name="T">サウンドコンテナのタイプ</typeparam>
    /// <param name="name">サウンド名。null なら全て</param>
    /// <param name="fadeDuration">フェード時間</param>
    /// <param name="onCompleteListener">フェード終了コールバック</param>
    public void Stop<T>(string name = null, float fadeDuration = 0, UnityAction onCompleteListener = null) where T : SoundContainer
    {

        // フェードアウト停止の場合
        if (fadeDuration > 0)
        {
            CrossFade<T>(null, name, fadeDuration, onCompleteListener);
            return;
        }

        // 再生中のチャンネルを取得
        var playChannels = findPlayingChannel<T>(name);
        if (playChannels == null)
        {
            return;
        }

        // 対象のオーディオチャンネルを取得
        Audiochannel audioChannel = getAudiochannel<T>();
        if (audioChannel == null)
        {
            return;     // サウンドコンテナのタイプがない
        }

        // 停止
        foreach (var playChannel in playChannels)
        {
            playChannel.Stop();
            //if( playChannel.clip != null ) {
            //    playChannel.clip.UnloadAudioData();
            //}
            playChannel.clip = null;
            playChannel.volume = audioChannel.MasterVolume;
        }
    }

    /// <summary>
    /// クロスフェード再生
    /// </summary>
    /// <param name="enterName">フェードイン BGM 名</param>
    /// <param name="exitName">フェードアウト BGM 名。null なら全て</param>
    /// <param name="duration">フェード時間</param>
    /// <param name="onCompleteListener">フェード終了コールバック</param>
    /// <param name="skipTime">途中再生時の時間（秒）</param>
    /// <returns>再生チャンネル</returns>
    public AudioSource CrossFade<T>(string enterName, string exitName, float duration, UnityAction onCompleteListener = null, float skipTime = 0) where T : SoundContainer
    {

        // クロスフェード処理中
        if (crossFadeRoutine != null)
        {
            //return null;

            // 停止
            Stop<T>();

            // クロスフェードコルーチン停止
            StopCoroutine(crossFadeRoutine);
            crossFadeRoutine = null;
        }

        // フェードイン対象チャンネル（１つ）
        AudioSource enterChannel = null;
        if (enterName != null)
        {
            enterChannel = Play<T>(enterName, skipTime: skipTime);
            if (enterChannel == null)
            {
                if (onCompleteListener != null)
                {
                    onCompleteListener.Invoke();
                }
                return null;
            }
        }

        // フェードアウト対象チャンネル（複数）
        var exitChannels = findPlayingChannel<T>(exitName);
        if (exitChannels == null || exitChannels.Count == 0)
        {
            if (onCompleteListener != null)
            {
                onCompleteListener.Invoke();
            }
            return null;
        }
        exitChannels = exitChannels.Where(channel => channel != enterChannel).ToList();    // フェードイン対象チャンネルを除く

        // 対象チャンネルが１つもない
        if (enterChannel == null && (exitChannels == null || exitChannels.Count == 0))
        {
            return null;
        }

        // 対象のオーディオチャンネルを取得
        Audiochannel audioChannel = getAudiochannel<T>();
        if (audioChannel == null)
        {
            return null;    // サウンドコンテナのタイプがない
        }

        // クロスフェード開始
        crossFadeRoutine = StartCoroutine(crossFade(enterChannel, exitChannels, duration, audioChannel.MasterVolume, onCompleteListener));

        return enterChannel;
    }

    /// <summary>
    /// 再生中かどうか
    /// </summary>
    /// <typeparam name="T">サウンドコンテナのタイプ</typeparam>
    /// <param name="name">サウンド名</param>
    /// <returns>再生中なら true</returns>
    public bool IsPlaying<T>(string name) where T : SoundContainer
    {

        // 再生中のチャンネルを検索
        var channels = findPlayingChannel<T>(name);
        if (channels == null)
        {
            return false;
        }

        // 再生中のチャンネルがあるかどうか
        return channels.Any();
    }

    /// <summary>
    /// 再生中のサウンド名を取得
    /// </summary>
    /// <returns>再生中のサウンド名</returns>
    public List<string> GetPlaying<T>() where T : SoundContainer
    {

        // 再生中のオーディオリソースを検索
        var resources = findPlayingResource<T>(null);
        if (resources == null)
        {
            return null;
        }

        // 再生中のサウンド名
        return resources.Select(resource => resource.Name).ToList();
    }

    /// <summary>
    /// クロスフェード中かどうか
    /// </summary>
    public bool IsCrossFading
    {
        get { return crossFadeRoutine != null; }
    }

    //--------------------------------------------------------------------------------
    // ミュート／ボリューム
    //--------------------------------------------------------------------------------

    /// <summary>
    /// ミュート設定
    /// </summary>
    /// <typeparam name="T">サウンドコンテナのタイプ</typeparam>
    /// <param name="isMute">ミュートするかどうか</param>
    /// <param name="isApply">実際の適用もするかどうか</param>
    public void SetMute<T>(bool isMute, bool isApply = true) where T : SoundContainer
    {

        // 対象のオーディオチャンネルを取得
        Audiochannel audioChannel = getAudiochannel<T>();
        if (audioChannel == null)
        {
            return;     // サウンドコンテナのタイプがない
        }

        // ミュートを設定
        audioChannel.IsMute = isMute;

        // 適用
        if (isApply)
        {
            audioChannel.AudioSources.ForEach(channel => channel.mute = isMute);
        }
    }

    /// <summary>
    /// ボリューム設定
    /// </summary>
    /// <typeparam name="T">サウンドコンテナのタイプ</typeparam>
    /// <param name="volume">ボリューム（0.0～1.0）</param>
    /// <param name="isApply">実際の適用もするかどうか</param>
    public void SetVolume<T>(float volume, bool isApply = true) where T : SoundContainer
    {

        // リミッタ
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);

        // 対象のオーディオチャンネルを取得
        Audiochannel audioChannel = getAudiochannel<T>();
        if (audioChannel == null)
        {
            return;     // サウンドコンテナのタイプがない
        }

        // ボリュームを設定
        audioChannel.MasterVolume = volume;

        // 適用
        if (isApply)
        {
            audioChannel.AudioSources.ForEach(channel => channel.volume = volume);
        }
    }

    //--------------------------------------------------------------------------------
    // サウンドコンテナ追加／削除
    //--------------------------------------------------------------------------------

    /// <summary>
    /// サウンドコンテナが存在するかどうか
    /// </summary>
    /// <param name="soundContainerName">サウンドコンテナ名</param>
    /// <returns>存在するかどうか</returns>
    public bool IsExist(string soundContainerName)
    {

        // サウンドコンテナ名は大文字小文字の区別なし
        return soundContainers.Any(container => container.ContainerName.ToLower() == soundContainerName.ToLower());
    }

    /// <summary>
    /// サウンドコンテナ追加
    /// 
    /// 非常駐のアセットバンドルのサウンド等、追加・削除を行う場合に使用
    /// </summary>
    /// <param name="soundContainer">サウンドコンテナ</param>
    /// <returns>true なら成功</returns>
    public bool Add(SoundContainer soundContainer)
    {

        // 既存なら追加しない
        if (IsExist(soundContainer.ContainerName))
        {
            return false;
        }

        // サウンドコンテナのリストへ追加
        soundContainers.Add(soundContainer);
        Debug.Log(soundContainer);

        return true;
    }

    /// <summary>
    /// サウンドコンテナ削除
    /// 
    /// SoundManager.Add() で追加したサウンドコンテナを削除する
    /// </summary>
    /// <param name="containerName">コンテナ名（SoundContainer.ContainerName）</param>
    /// <param name="isRemoveAudioResource">trueなら個別にAudioResourceを解放する</param>
    public void Remove(string containerName, bool isRemoveAudioResource = false)
    {

        // サウンドコンテナのリストから削除
        var soundContainer = soundContainers.FirstOrDefault(container => container.ContainerName.ToLower() == containerName.ToLower());   // サウンドコンテナ名は大文字小文字の区別なし
        if (soundContainer != null)
        {
            soundContainers.Remove(soundContainer);

            // ローカルで生成したコンテナはオーディオクリップを個別に解放
            if (isRemoveAudioResource)
            {
                if (soundContainer.Resources != null)
                {
                    foreach (var resource in soundContainer.Resources)
                    {
                        // Resources.UnloadAsset(resource.Clip);
                        // resource.Clip = null;
                    }
                }
            }
            else
            {
                // コンテナ自体も解放
                Resources.UnloadAsset(soundContainer);
            }
        }
    }

    //================================================================================
    // ローカル
    //================================================================================

    /// <summary>
    /// 対象のオーディオリソースを検索
    /// </summary>
    /// <typeparam name="T">サウンドコンテナのタイプ</typeparam>
    /// <param name="name">サウンド名</param>
    /// <returns>対象のオーディオリソース</returns>
    private AudioResource findAudioResource<T>(string name, bool isCareClip = false) where T : SoundContainer
    {

        // サウンドコンテナのタイプ
        System.Type type = typeof(T);

        // 対象のオーディオリソース
        AudioResource playResource = null;

        // 対象のサウンドコンテナを取得
        var targetContainers = soundContainers.Where(container => container.GetType() == type).ToList();
        if (targetContainers == null)
        {
            return null;
        }

        // 対象のオーディオリソースを取得
        foreach (var container in targetContainers)
        {
            playResource = container.Resources.FirstOrDefault(resource =>
            {
                if (isCareClip)
                {
                    return false;
                }
                return resource.Name == name;
            });
            if (playResource != null)
            {
                break;
            }
        }

        return playResource;
    }

    /// <summary>
    /// 再生中のチャンネルを検索
    /// </summary>
    /// <typeparam name="T">サウンドコンテナのタイプ</typeparam>
    /// <param name="name">サウンド名。null なら全て</param>
    /// <returns>再生中のチャンネル</returns>
    private List<AudioSource> findPlayingChannel<T>(string name) where T : SoundContainer
    {

        // 再生中のオーディオリソース
        AudioResource playResource = null;

        // サウンド名が指定されてなければ全ての再生中のオーディオリソースが対象
        if (name != null)
        {

            // 対象のオーディオリソースを検索
            playResource = findAudioResource<T>(name);
            if (playResource == null)
            {
                return null;
            }
        }

        // 対象のオーディオチャンネルを取得
        Audiochannel audioChannel = getAudiochannel<T>();
        if (audioChannel == null)
        {
            return null;      // サウンドコンテナのタイプがない
        }

        // 再生中のチャンネルを取得
        var playChannels = audioChannel.AudioSources.Where(channel => channel.isPlaying && (playResource == null || channel.clip.name == playResource.Name)).ToList();

        return playChannels;
    }

    /// <summary>
    /// 再生中のオーディオリソースを検索
    /// </summary>
    /// <typeparam name="T">サウンドコンテナのタイプ</typeparam>
    /// <param name="name">サウンド名。null なら全て</param>
    /// <returns>再生中のオーディオリソース</returns>
    private List<AudioResource> findPlayingResource<T>(string name) where T : SoundContainer
    {

        // サウンドコンテナのタイプ
        System.Type type = typeof(T);

        // 対象のサウンドコンテナを取得
        var targetContainers = soundContainers.Where(container => container.GetType() == type).ToList();
        if (targetContainers == null)
        {
            return null;
        }
        // 再生中のチャンネルを検索
        var playChannels = findPlayingChannel<T>(null);

        // 再生中のオーディオリソース
        var playResources = new List<AudioResource>();

        // 再生中のオーディオリソースを取得
        foreach (var container in targetContainers)
        {
            var resources = container.Resources.Where(resource =>
            {

                // サウンド名が指定されてなければ全ての再生中のオーディオリソースが対象
                if (name != null && resource.Name != name)
                {
                    return false;
                }

                return playChannels.Any(channel => channel.clip == AssetManager.Instance.GetAudioClip(resource.Name));
            });

            // 再生中のオーディオリソース
            playResources.AddRange(resources);
        }

        return playResources;
    }

    /// <summary>
    /// サウンドコンテナのタイプからオーディオチャンネルを取得
    /// </summary>
    /// <returns>オーディオチャンネル</returns>
    private Audiochannel getAudiochannel<T>() where T : SoundContainer
    {

        // サウンドコンテナのタイプ
        System.Type type = typeof(T);

        // 対象のサウンドコンテナの再生チャンネルを取得
        Audiochannel audiochannel;
        if (channelsList.TryGetValue(type, out audiochannel) == false)
        {
            return null;      // サウンドコンテナのタイプがない
        }

        return audiochannel;
    }

    /// <summary>
    /// クロスフェード
    /// </summary>
    /// <param name="enterChannel">フェードイン対象チャンネル</param>
    /// <param name="exitChannels">フェードアウト対象チャンネル</param>
    /// <param name="duration">フェード時間</param>
    /// <param name="onCompleteListener">フェード終了コールバック</param>
    private IEnumerator crossFade(AudioSource enterChannel, List<AudioSource> exitChannels, float duration, float masterVolume, UnityAction onCompleteListener)
    {

        float pastTime = 0;
        while (true)
        {

            // ボリューム算出
            pastTime += Time.deltaTime;
            float volume = Mathf.Lerp(0.0f, masterVolume, pastTime / duration);
            if (pastTime >= duration)
            {
                volume = masterVolume;
            }

            // ボリューム設定
            if (enterChannel != null)
            {
                enterChannel.volume = volume;
            }
            if (exitChannels != null)
            {
                foreach (var channel in exitChannels)
                {
                    channel.volume = masterVolume - volume;
                }
            }

            // 終了チェック
            if (pastTime >= duration)
            {
                break;
            }
            yield return null;
        }

        // フェードアウトしたチャンネルを停止してボリュームを戻す
        if (exitChannels != null)
        {
            foreach (var channel in exitChannels)
            {
                channel.Stop();
                channel.clip = null;
                channel.volume = masterVolume;
            }
        }

        // フェード終了コールバック
        if (onCompleteListener != null)
        {
            onCompleteListener.Invoke();
        }

        // クロスフェード処理終了
        crossFadeRoutine = null;
    }
}
