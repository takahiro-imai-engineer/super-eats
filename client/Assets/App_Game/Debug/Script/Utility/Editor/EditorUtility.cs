

namespace App.Editor
{
    public class EditorUtility
    {
        /// <summary>
        /// ディレクトリを開く
        /// </summary>
        /// <param name="directoryPath">Assetsからのパス（例：Assets/App_Project）.</param>
        public static void OpenDirectory(string directoryPath)
        {
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.OSXEditor) { System.Diagnostics.Process.Start(directoryPath); }
            else if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsEditor) { UnityEditor.EditorUtility.RevealInFinder(directoryPath); }
            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadMainAssetAtPath(directoryPath);
        }


        public static void SelectFile(string filePath)
        {
            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadMainAssetAtPath(filePath);
        }


    }
}
