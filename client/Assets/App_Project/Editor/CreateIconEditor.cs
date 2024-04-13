using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (CreateIcon))] //拡張するクラスを指定
public class CreateIconEditor : Editor {

    /// <summary>
    /// InspectorのGUIを更新
    /// </summary>
    public override void OnInspectorGUI () {
        //元のInspector部分を表示
        base.OnInspectorGUI ();

        //targetを変換して対象を取得
        CreateIcon createIconScript = target as CreateIcon;

        //PrivateMethodを実行する用のボタン
        if (GUILayout.Button ("Save")) {
            //SendMessageを使って実行
            createIconScript.SendMessage ("Save", null, SendMessageOptions.DontRequireReceiver);
        }

    }

}