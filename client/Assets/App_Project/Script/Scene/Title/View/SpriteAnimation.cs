using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float fps = 24.0f;
    [SerializeField] private List<Sprite> frames;
    [SerializeField] private bool isLoop = false;

    private int frameIndex;
    private bool isStartAnimation = false;

    public void Init()
    {
        gameObject.SetActive(false);
        frameIndex = 0;
        isStartAnimation = false;
    }

    public void Show()
    {
        isStartAnimation = true;
        gameObject.SetActive(true);
        var sequence = DOTween.Sequence().SetLink(gameObject);
        for (int i = 0; i < frames.Count; i++)
        {
            sequence.AppendInterval(1f / fps)
                .AppendCallback(() =>
                {
                    frameIndex++;
                    if (frameIndex >= frames.Count)
                    {
                        frameIndex = 0;
                    }
                    image.sprite = frames[frameIndex];
                });
        }
        if (isLoop)
        {
            sequence.SetLoops(-1);
        }
        else
        {
            sequence.OnComplete(() =>
            {
                Hide();
            });
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
