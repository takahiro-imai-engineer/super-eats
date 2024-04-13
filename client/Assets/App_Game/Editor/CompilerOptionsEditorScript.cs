using UnityEditor;

[InitializeOnLoad]
public class CompilerOptionsEditorScript
{
    private static bool waitingForStop;

    static CompilerOptionsEditorScript()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        if ( !waitingForStop && EditorApplication.isCompiling && EditorApplication.isPlaying )
        {
            EditorApplication.LockReloadAssemblies();
            EditorApplication.playmodeStateChanged += PlaymodeChanged;
            waitingForStop = true;
        }
    }

    private static void PlaymodeChanged()
    {
        if ( EditorApplication.isPlaying ) return;

        EditorApplication.UnlockReloadAssemblies();
        EditorApplication.playmodeStateChanged -= PlaymodeChanged;
        waitingForStop = false;
    }
}