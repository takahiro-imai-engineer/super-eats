using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AvatarView : MonoBehaviour
{
    [SerializeField] Transform avatarRoot;
    [SerializeField] List<GameObject> _AvatarList;


    public void Init()
    {

    }

    public void ChangeAvatar(int AvatarId, bool isChangeAnimation)
    {
        for (int i = 0; i < _AvatarList.Count; i++)
        {
            _AvatarList[i].SetActive(i + 1 == AvatarId);
        }
        if (isChangeAnimation)
        {
            // NOTE: ボヨンと大きくなる
            avatarRoot.DOPunchScale(
                new Vector3(-0.5f, -0.5f, -0.5f),
                0.5f
            ).SetLink(gameObject);
            // NOTE: 1回転する
            avatarRoot.DOLocalRotate(
                new Vector3(0, 360f + avatarRoot.localEulerAngles.y, 0f),
                0.3f,
                RotateMode.FastBeyond360
            ).SetLink(gameObject);
        }
    }
}
