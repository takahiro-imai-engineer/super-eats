using UnityEngine;
using DG.Tweening;
using template;

public class SoundGimick : MonoBehaviour
{
    [Header("生成時に自動でSEを再生するか")]
    [SerializeField] private bool isAutoPlay;
    [Header("3Dサウンドにするか(自機との距離によって音量が変動)")]
    [SerializeField] private bool is3DSound;
    [Header("ループ再生するか")]
    [SerializeField] private bool isLoop;
    [Header("再生するSE")]
    [SerializeField] private InGameConstant.GimickSE gimickSE;
    [Header("SE再生までの遅延時間")]
    [SerializeField] private float playDelayTime = 0f;

    private bool isInitialize = false;
    private AudioSource audioSource;

    //================================================================================
    // ローカル
    //================================================================================
    private void Awake()
    {
        Init();
    }

    //================================================================================
    // ローカル
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        if (isInitialize)
        {
            return;
        }
        isInitialize = true;
        var movingGimick = this.GetComponent<MovingGimick>();
        if (movingGimick != null)
        {
            audioSource = movingGimick.MoveObjectRoot.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        var audioResource = SoundManager.Instance.GetGimickSE(gimickSE);
        audioSource.clip = AssetManager.Instance.GetAudioClip(audioResource.Name);
        audioSource.spatialBlend = is3DSound ? 1f : 0f;
        audioSource.dopplerLevel = 0;
        audioSource.playOnAwake = false;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 100;
        audioSource.loop = isLoop;
        if (isAutoPlay && !UserDataProvider.Instance.GetSaveData().IsMuteSe)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// サウンドを再生
    /// </summary>
    public void PlaySound()
    {
        if (UserDataProvider.Instance.GetSaveData().IsMuteSe)
        {
            return;
        }
        if (playDelayTime <= 0f)
        {

            audioSource.Play();
        }
        else
        {
            DOVirtual.DelayedCall(playDelayTime, () =>
            {
                audioSource.Play();
            });
        }
    }
}
