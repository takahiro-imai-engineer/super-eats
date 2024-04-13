using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// ギミックをパスに沿って移動させるクラス
/// </summary>
public class MovingGimick : GimmickView
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [Header("移動するオブジェクト")]
    /// <summary>ルート</summary>
    [SerializeField] private GameObject moveObjectRoot;
    [Header("移動する経路")]
    /// <summary>パスルート</summary>
    [SerializeField] private GameObject pathRoot;
    [Header("移動し始める判定")]
    [SerializeField] private ChildCollider childCollider;
    [Header("パスの補完した時の動き方。曲線(CatmullRom)/直線(Linear)")]
    [SerializeField] private PathType pathType;
    [Header("時間経過による動き方")]
    [SerializeField] private Ease ease = Ease.Linear;
    [Header("移動時間")]
    [SerializeField] private float durationTime = 1f;
    [Header("最初から表示しておくか")]
    [SerializeField] private bool isBeforeShow = true;
    [Header("移動完了時に非表示にするか")]
    [SerializeField] private bool isCompleteHide = false;
    [Header("ループするか")]
    [SerializeField] private bool isLoop = false;

    //================================================================================
    // ローカル
    //================================================================================
    private bool isActive = false;
    private Sequence sequence;
    private Vector3[] movePathList;
    //================================================================================
    // プロパティ
    //================================================================================
    public bool IsLoop => isLoop;
    public GameObject MoveObjectRoot => moveObjectRoot;
    //================================================================================
    // メソッド
    //================================================================================
    private void Awake()
    {
        if (moveObjectRoot == null)
        {
            Debug.LogError("移動対象は設定されていません。");
            return;
        }
        moveObjectRoot.SetActive(isBeforeShow);
        var pathList = pathRoot.GetComponentsInChildren<Transform>().Where(c => pathRoot != c.gameObject).ToList();
        movePathList = new Vector3[pathList.Count];
        for (int i = 0; i < pathList.Count; i++)
        {
            movePathList[i] = pathList[i].position;
        }
        var soundGimick = this.GetComponent<SoundGimick>();
        sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                moveObjectRoot.SetActive(true);
                if (soundGimick != null)
                {
                    soundGimick.Init();
                    soundGimick.PlaySound();
                }
            })
            .Append(
                moveObjectRoot.transform
                .DOPath(movePathList, durationTime, pathType)
                .SetLookAt(0.1f, Vector3.forward)
                .SetEase(ease)
                .SetLoops(isLoop ? -1 : 1)
            )
            .OnComplete(() =>
            {
                moveObjectRoot.SetActive(!isCompleteHide);
                if (!isLoop)
                {
                    // Destroy (this.gameObject, 1f);
                }
            })
            .Pause()
            .SetAutoKill(false)
            .SetLink(this.gameObject);
        childCollider.SetEvent(HitEvent);
    }

    private void OnDestroy()
    {
        sequence.Kill();
    }

    /// <summary>
    /// ギミック開始
    /// </summary>
    private void ActiveGimick()
    {
        isActive = true;
        sequence.Play();
    }

    /// <summary>
    /// ギミック停止
    /// </summary>
    public void Stop()
    {
        sequence.Pause();
    }

    private void HitEvent()
    {
        if (isActive)
        {
            return;
        }
        else
        {
            ActiveGimick();
            Debug.Log("移動ギミック起動");
        }
    }
}