using UnityEngine;
using DG.Tweening;

public class ZombieView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private Collider collider;
    [SerializeField] private Animator animator;
    [Header("ラグドール停止までの時間")]
    [SerializeField] private float stopRagdollTime = 2f;
    [Header("吹き飛ばす力")]
    [SerializeField] private float pushPower = 500f;
    [Header("移動する場合、接触時に移動を停止するために要アタッチ")]
    [SerializeField] private MovingGimick movingGimmick;

    //================================================================================
    // ローカル
    //================================================================================
    private Rigidbody[] ragdollRigidbodies;

    //================================================================================
    // メソッド
    //================================================================================
    private void Awake()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        SetRagdoll(false);
        collider.enabled = true;
    }

    public void BlowAway(Vector3 pushVector)
    {
        if (movingGimmick != null)
        {
            // 移動停止
            movingGimmick.Stop();
        }
        collider.enabled = false;
        SetRagdoll(true);
        Debug.Log($"吹っ飛ばし方向：{pushVector}");
        pushVector.y = 0.1f;
        foreach (var ragdoll in ragdollRigidbodies)
        {
            ragdoll.AddForce(pushVector * pushPower, ForceMode.Impulse);
        }
        DOVirtual.DelayedCall(
            stopRagdollTime,
            () =>
            {
                foreach (var ragdoll in ragdollRigidbodies)
                {
                    ragdoll.isKinematic = true;
                }
            }
        );
    }

    private void SetRagdoll(bool isEnable)
    {
        foreach (var ragdoll in ragdollRigidbodies)
        {
            ragdoll.isKinematic = !isEnable;
            animator.enabled = !isEnable;
        }
    }
}
