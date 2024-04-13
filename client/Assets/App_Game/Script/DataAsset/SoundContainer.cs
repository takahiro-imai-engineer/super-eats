using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// サウンドコンテナ
/// </summary>
[System.Serializable]
public class SoundContainer : ScriptableObject
{

    /// <summary>コンテナ名。※アセットバンドル等で追加するものは必須</summary>
    public string ContainerName;

    /// <summary>最大チャンネル数。同時再生数。AudioSource の数。※アセットバンドル等で追加するものは指定しなくてよい</summary>
    public int MaxChannelCount;

    /// <summary>オーディオリソース</summary>
    public List<AudioResource> Resources;

    /// <summary>常駐コンテナかどうか</summary>
    [System.NonSerialized]
    [HideInInspector]
    public bool IsResidential;

    /// <summary>
    /// オーディオリソース追加
    /// </summary>
    /// <param name="audioResource">オーディオリソース</param>
    public void AddResource(AudioResource audioResource)
    {
        if (Resources == null)
        {
            Resources = new List<AudioResource>();
        }

        // 名前が重複していない場合に追加
        if (Resources.Any(resource => resource.Name == audioResource.Name) == false)
        {
            Resources.Add(audioResource);
        }
    }
}

/// <summary>
/// オーディオリソース
/// </summary>
[System.Serializable]
public class AudioResource
{

    /// <summary>名前</summary>
    public string Name;

    /// <summary>クリップ</summary>
    // public AudioClip Clip;

    /// <summary>ループの有無</summary>
    public bool IsLoop;
}
