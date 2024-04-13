using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// リザルトカメラクラス
/// </summary>
public class ResultCamera : MonoBehaviour {

    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private CinemachineVirtualCamera resultCamera;
    [SerializeField] private float rotateSpeed = 3.0f;
    [SerializeField] private Vector3 offset;
    //================================================================================
    // ローカル
    //================================================================================
    private Vector3 playerPosition;
    private bool isResult = false;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init (PlayerController playerController) {
        isResult = false;
        resultCamera.Priority = 10;
        resultCamera.LookAt = playerController.transform;
    }

    /// <summary>
    /// 表示
    /// </summary>
    public void Show (Vector3 _playerPosition) {
        playerPosition = _playerPosition;
        this.transform.position = playerPosition + offset;
        resultCamera.Priority = 300;
        isResult = true;
    }

    void Update () {
        if (!isResult) {
            return;
        }

        //回転させる角度
        float angle = Time.deltaTime * rotateSpeed;

        //カメラを回転させる
        transform.RotateAround (playerPosition, Vector3.up, angle);
    }
}