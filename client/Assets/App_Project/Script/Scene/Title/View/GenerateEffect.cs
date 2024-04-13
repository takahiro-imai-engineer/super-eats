using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateEffect : MonoBehaviour
{
    [Header("エフェクト名(Resources/Effectフォルダ)")]
    [SerializeField] private string effectName;

    private void Start()
    {
        var effect = Resources.Load<GameObject>("Effect/" + effectName);
        Instantiate(effect, transform.position, Quaternion.identity, transform);
    }
}
