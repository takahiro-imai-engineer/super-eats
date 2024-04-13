public class ResidentOverlayObject<T> : OverlayObject where T : OverlayObject
{
    /// <summary>インスタンス</summary>
    private static T instance;

    /// <summary>インスタンス</summary>
    public static T Instance
    {
        get
        {
            if (!Exists())
            {
				instance = OverlayGroup.Instance.Get<T>();
                if (!Exists())
                {
					instance = OverlayGroup.Instance.Show<T>();
                }
            }
            return instance;
        }
    }

    public static bool Exists()
    {
        return instance.IsNotNull();
    }

	/// <summary>
    /// 消去
    /// </summary>
    public static void Dismiss()
    {
        // インスタンス消去
        if (Exists())
        {
            OverlayGroup.Instance.Dismiss<T>();
            instance = null;
        }
    }

}
