public static class ButtonExtention
{
    public static bool Enabled(this UnityEngine.UI.Button button)
    {
        if (button == null)
        {
            return false;
        }
        if (button.gameObject == null)
        {
            return false;
        }
        if (!button.interactable)
        {
            return false;
        }
        if (!button.isActiveAndEnabled)
        {
            return false;
        }
        return true;
    }
}
