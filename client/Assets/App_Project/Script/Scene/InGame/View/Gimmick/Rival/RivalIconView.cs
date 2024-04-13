using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RivalIconView : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image characterIcon;
    [SerializeField] private Image nationalityFlagIcon;
    [SerializeField] private TextMeshProUGUI characterNameText;

    private Transform cameraTransform;

    public void Init(RivalData rivalData)
    {
        canvas.worldCamera = Camera.main;
        cameraTransform = Camera.main.transform;
        // キャラアイコン・国旗・名前を反映
        characterIcon.sprite = AssetManager.Instance.LoadCharacterIconSprite(rivalData.IconName);
        nationalityFlagIcon.sprite = rivalData.NationalFlag;
        characterNameText.text = rivalData.Name;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            return;
        }
        // transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
        transform.rotation = cameraTransform.rotation;
    }
}
