public class ApplicationUtil
{
    /// <summary> iPhone かどうか </summary>
    public static bool IsiPhone { get { return UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer; } }
    /// <summary> Android かどうか </summary>
    public static bool IsAndroid { get { return UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android; } }
    /// <summary> WebGL かどうか </summary>
    public static bool IsWebGL { get { return UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WebGLPlayer; } }

    /// <summary>
    /// WebGLかどうか (UnityEditorのPlatformがWebGLの場合もtrue)
    /// </summary>
    public static bool IsUnityWebGL {
        get {
#if UNITY_WEBGL
            return true;
#else
            return false;
#endif
        }
    }
    /// <summary>
    /// UnityEditorかどうか
    /// </summary>
    public static bool IsUnityEditor {
        get {
#if UNITY_EDITOR
            return true;
#else
                return false;
#endif
        }
    }
    /// <summary>
    /// ブラウザでURLを開きます
    /// </summary>
    /// <param name="url">URL.</param>
    public static void OpenURL(string url)
    {
#if UNITY_WEBGL
        UnityEngine.Application.ExternalEval(string.Format("window.open('{0}');", url));
#else
        UnityEngine.Application.OpenURL(url);
#endif
    }

}
