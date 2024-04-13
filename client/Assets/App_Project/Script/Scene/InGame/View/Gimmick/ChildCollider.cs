using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 子のコライダーの接触イベントを親に伝えるクラス
/// </summary>
public class ChildCollider : MonoBehaviour {
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private bool isTrigger = true;
    [SerializeField] private bool isCollision = false;
    [SerializeField] private string tagName = app_system.Tags.Player;
    //================================================================================
    // ローカル
    //================================================================================
    private UnityAction callback;
    //================================================================================
    // メソッド
    //================================================================================
    public void SetEvent (UnityAction _callback) {
        callback = _callback;
    }

    private void OnTriggerEnter (Collider other) {
        if (!isTrigger) {
            return;
        } else if (other.gameObject.CompareTag (tagName)) {
            callback.Invoke ();
        }
    }

    private void OnCollisionEnter (Collision other) {
        if (!isCollision) {
            return;
        } else if (other.gameObject.CompareTag (tagName)) {
            callback.Invoke ();
        }
    }
}