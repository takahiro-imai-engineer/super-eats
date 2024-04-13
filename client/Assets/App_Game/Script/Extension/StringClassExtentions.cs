/// <summary>
/// String クラス 拡張.
/// </summary>
public static class StringClassExtentions
{
    /// <summary>
    /// 指定された文字列が null または 文字列("") かどうかを返します.
    /// </summary>
    public static bool IsNullOrEmpty(this string self) { return string.IsNullOrEmpty(self); }
    /// <summary>
    /// 指定された文字列が null または 文字列("") 以外であるかどうかを返します.
    /// </summary>
    public static bool IsNotNullOrEmpty(this string self) { return !string.IsNullOrEmpty(self); }
    /// <summary>
    /// 指定された形式に基づいてオブジェクトの値を文字列に変換し、別の文字列に挿入します。
    /// </summary>
    public static string FormatString(this string self, params object[] args) { return string.Format(self, args); }
    /// <summary>
    /// 文字サイズのフォーマット
    /// </summary>
    public static string FormatSize(this string message, int messageSize)
    {
        return string.Format(FORMAT_SIZE, messageSize, message);
    }
    public const string FORMAT_SIZE = "<size={0}>{1}</size>";
    /// <summary>
    /// 文字色のフォーマット
    /// </summary>
    public static string FormatColor(this string message, string colorCode)
    {
        return string.Format(FORMAT_COLOR, colorCode, message);
    }
    public const string FORMAT_COLOR = "<color={0}>{1}</color>";
}
