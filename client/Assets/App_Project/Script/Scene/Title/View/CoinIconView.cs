using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class CoinIconView : MonoBehaviour
{
    //================================================================================
    // ローカル変数
    //================================================================================
    /// <summary>コインの動き</summary>
    private Tween tween;

    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 移動する
    /// </summary>
    public void Move(Vector3 targetPosition, float moveDuration, UnityAction onEndMoveCallback)
    {
        // コインを移動する
        tween = transform
            .DOMove(targetPosition, moveDuration)
            .SetLink(gameObject)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                onEndMoveCallback?.Invoke();
                // 移動が終了したら削除する
                Destroy(gameObject);
            });
    }

    private void OnDisable()
    {
        // Tweenを破棄する
        if (DOTween.instance != null)
        {
            tween.Kill();
        }
    }
}
