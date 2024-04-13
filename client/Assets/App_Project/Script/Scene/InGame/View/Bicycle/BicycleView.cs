using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using app_system;
using DG.Tweening;

public class BicycleView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] Transform bicycleRoot;

    //================================================================================
    // プロパティ
    //================================================================================
    public Animator Animator { private set; get; }

    //================================================================================
    // メソッド
    //================================================================================
    public void ChangeBicycle(BicycleData bicycleData, bool isChangeAnimation)
    {
        foreach (Transform item in bicycleRoot)
        {
            Destroy(item.gameObject);
        }
        var bicycleObject = Instantiate(bicycleData.bicycleObject, bicycleRoot);
        Animator = bicycleObject.GetComponent<Animator>();

        if (isChangeAnimation)
        {
            // NOTE: ボヨンと大きくなる
            bicycleRoot.DOPunchScale(
                new Vector3(-0.5f, -0.5f, -0.5f),
                0.5f
            ).SetLink(gameObject);
            // NOTE: 1回転する
            bicycleRoot.DOLocalRotate(
                new Vector3(0, 360f + bicycleRoot.localEulerAngles.y, 0f),
                0.3f,
                RotateMode.FastBeyond360
            ).SetLink(gameObject);
        }
    }

    public void StopAnimation()
    {
        if (Animator != null)
        {
            Animator.enabled = false;
        }
    }
}
