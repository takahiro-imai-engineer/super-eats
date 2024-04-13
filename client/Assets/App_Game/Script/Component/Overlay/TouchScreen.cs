using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchScreen : OverlayObject
{
    [SerializeField]
    private EventTrigger eventTrigger;

    /// <summary>
    /// 表示
    /// </summary>
    public static TouchScreen Show(UnityAction clickEvent)
    {
        // インスタンス生成
        var instance = OverlayGroup.Instance.Show<TouchScreen>();
        if(instance == null)
        {
            return null;
        }
        // 初期化
        instance.Initialize(clickEvent);
        return instance;
    }

    /// <summary>
    /// 消去
    /// </summary>
    public static void Dismiss()
    {
        // インスタンス取得
        var instance = OverlayGroup.Instance.Get<TouchScreen>();
        if (instance == null)
        {
            Debug.LogWarning("TouchScreen instans not found.");
            return;
        }
        // インスタンス消去
        OverlayGroup.Instance.Dismiss<TouchScreen>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns>The initialize.</returns>
    /// <param name="clickEvent">Click event.</param>
    public void Initialize(UnityAction clickEvent)
    {
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { clickEvent.Invoke(); });
        if(eventTrigger == null)
        {
            Debug.LogWarning("EventTrigger not found.");
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        eventTrigger.triggers.Add(entry);
    }

}
