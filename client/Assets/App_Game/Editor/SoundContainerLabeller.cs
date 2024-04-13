using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

/// <summary>
/// サウンドコンテナのオーディオクリップに名前を付ける
/// </summary>
public class SoundContainerLabeller
{

    /// <summary>
    /// サウンドコンテナのオーディオクリップに名前を付ける
    /// サウンドIDを定義した Assets/SoundId.txt が生成される
    /// 
    /// サウンドコンテナを選択、またはサウンドコンテナが入ったフォルダを選択した状態で実行する
    /// </summary>
    [MenuItem("[Game]/Sound/Set Sound Names")]
    static void SetSoundNames()
    {

        // サウンド名
        List<string> soundNames = new List<string>();

        // 指定ファイル／フォルダ（内ファイル）のオブジェクト
        var selectObjs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        // サウンドコンテナ分
        foreach (var selectObj in selectObjs)
        {
            if ((selectObj is SoundContainer) == false)
            {
                continue;
            }

            // パス
            var assetPath = AssetDatabase.GetAssetPath(selectObj);
            if (File.Exists(assetPath))
            {

                // ロードして名前を付ける
                var containerObj = AssetDatabase.LoadAssetAtPath<SoundContainer>(assetPath);
                foreach (var resource in containerObj.Resources)
                {
                    resource.Name = string.Empty;
                    // if( resource.Clip ) {
                    // 	resource.Name = resource.Clip.name;

                    // 	// サウンド名
                    // 	soundNames.Add( resource.Name );
                    // }
                }
                EditorUtility.SetDirty(containerObj);
            }
        }

        // 保存
        AssetDatabase.SaveAssets();

        // サウンドID出力
        exportSoundId(soundNames);
    }

    /// <summary>
    /// サウンドID出力
    /// </summary>
    static void exportSoundId(List<string> soundNames)
    {
        if (soundNames.Count == 0)
        {
            return;
        }

        // 定義を書く
        var sb = new StringBuilder();
        foreach (var soundName in soundNames)
        {
            sb.Append(string.Format("\tpublic const string {0} = \"{1}\";", soundName, soundName));
            sb.Append(System.Environment.NewLine);
        }

        // ローカルテキスト定義ファイルを作成
        string filePath = Path.GetFileName(Application.dataPath) + "/SoundId.txt";
        using (var sw = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
        {
            sw.Write(sb.ToString());
        }

        AssetDatabase.Refresh();
    }
}
