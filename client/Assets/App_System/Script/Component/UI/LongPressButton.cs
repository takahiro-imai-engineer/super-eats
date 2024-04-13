public class LongPressButton : UnityEngine.MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler, UnityEngine.EventSystems.IPointerUpHandler, UnityEngine.EventSystems.IPointerExitHandler
{
    /// <summary>長押し時間</summary>
    [UnityEngine.SerializeField, UnityEngine.Range(0, 5)]
    private float LongPressDuration= 1.0f;
    /// <summary></summary>
    [UnityEngine.SerializeField, ReadOnly]
    private bool IsLongTap = false;

    /// <summary>タップイベント</summary>
    [UnityEngine.SerializeField]
    private UnityEngine.Events.UnityEvent PressEvent;
    private void OnPress() { if (PressEvent.IsNotNull()) { PressEvent.Invoke(); } }
    /// <summary>ロングタップイベント</summary>
    [UnityEngine.SerializeField]
    private UnityEngine.Events.UnityEvent BegineLongPressEvent;
    private void OnBegineLongPress() { if (BegineLongPressEvent.IsNotNull()) { BegineLongPressEvent.Invoke(); } }
    /// <summary>ロングタップイベント</summary>
    [UnityEngine.SerializeField]
    private UnityEngine.Events.UnityEvent EndLongPressEvent;
    private void OnEndLongPress() { if (EndLongPressEvent.IsNotNull()) { EndLongPressEvent.Invoke(); } }

    /// <summary> 時間監視ルーチン </summary>
    private UnityEngine.Coroutine MonitorTimeRoutine;


    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Reset();
        MonitorTimeRoutine = StartCoroutine(MonitorTime(()=> {
            BegineLongPressEvent.Invoke();
        }));
    }

    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (IsLongTap) {
            OnEndLongPress();
        }
        else {
            OnPress();
        }
        Reset();
    }

    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // 何も実行せず、状態を初期化
        Reset();
    }

    private System.Collections.IEnumerator MonitorTime(System.Action begineLongPressEvent)
    {
        var time = 0f;
        while (true)
        {
            yield return null;
            time += UnityEngine.Time.deltaTime;
            if (time >= LongPressDuration)
            {
                IsLongTap = true;
                begineLongPressEvent.Invoke();
                yield break;
            }
        }
    }

    private void Reset()
    {
        if (MonitorTimeRoutine.IsNotNull())
        {
            StopCoroutine(MonitorTimeRoutine);
        }
        MonitorTimeRoutine = null;
        IsLongTap = false;
    }
}
