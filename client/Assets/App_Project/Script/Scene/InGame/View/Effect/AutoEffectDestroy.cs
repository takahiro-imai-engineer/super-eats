using UnityEngine;

/// <summary>
/// エフェクトの終了時に親も自動で削除するクラス
/// </summary>
public class AutoEffectDestroy : MonoBehaviour {
    //================================================================================
    // インスペクタ
    //================================================================================
    /// <summary>削除する親ルート</summary>
    [SerializeField] private GameObject parentRoot;
    /// <summary>パーティクル</summary>
    [SerializeField] private ParticleSystem particle;
    //================================================================================
    // メソッド
    //================================================================================
    private void Start () {
        Destroy (gameObject, particle.duration);
    }
    private void OnDestroy () {
        Destroy (parentRoot);
    }
}