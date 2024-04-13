using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using app_system;
using DG.Tweening;

public class BagView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] Transform bagRoot;

    //================================================================================
    // 定数
    //================================================================================
    const float BagOffsetPositionZ = 0.25f;

    //================================================================================
    // メソッド
    //================================================================================

    public void ChangeBag(BagData bagData, bool isChangeAnimation, bool isTitleScene = false)
    {
        foreach (Transform item in bagRoot)
        {
            Destroy(item.gameObject);
        }
        var bagObject = Instantiate(bagData.bagObject, bagRoot);
        if (isTitleScene)
        {
            bagObject.transform.localPosition = new Vector3(0, 0, BagOffsetPositionZ);
        }
        if (isChangeAnimation)
        {
            // NOTE: ボヨンと大きくなる
            bagRoot.DOPunchScale(
                new Vector3(-0.5f, -0.5f, -0.5f),
                0.5f
            ).SetLink(gameObject);
            // NOTE: 1回転する
            bagRoot.DOLocalRotate(
                new Vector3(0, 360f + bagRoot.localEulerAngles.y, 0f),
                0.3f,
                RotateMode.FastBeyond360
            ).SetLink(gameObject);
        }
    }
}
