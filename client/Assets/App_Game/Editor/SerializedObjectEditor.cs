using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 選択中の Object と関連 Object から SerializedObject を作って Property を全て表示する（編集も可）
/// </summary>
public class SerializedObjectEditor : EditorWindow {
    const int defaultVisibleArrayElements = 20;
    int maxVisibleArrayElements = defaultVisibleArrayElements;
    bool someArraysVisible;
    List<SerializedObject> targets = new List<SerializedObject>();
    Object lastSelected;
    bool warning;
    Vector2 scroll;
    bool[] foldout = new bool[0];

    [MenuItem( "[Game]/External/Serialized Object Editor" )]
    static void OpenWindow() {
        EditorWindow.GetWindow<SerializedObjectEditor>( "SerializedObject" ).OnSelectionChange();
    }

    void OnGUI() {
        if( lastSelected != Selection.activeObject ) {
            CheckSelections();
        }

        // シーンを読み込もうとしている場合は警告
        if( warning ) {
            EditorGUILayout.HelpBox( "Loading scene file may cause problems.", MessageType.Warning );
            if( GUILayout.Button( "Load Scene (Unsafe)" ) ) {
                warning = false;
                AddSelectionToTargets();
            }
        }

        // 配列が表示されていないときに表示量をリセット
        if( !someArraysVisible ) {
            maxVisibleArrayElements = defaultVisibleArrayElements;
        }
        else {
            someArraysVisible = false;
        }

        scroll = EditorGUILayout.BeginScrollView( scroll );
        for( int i = 0; i < targets.Count; i++ ) {
            var target = targets[i];

            // タイトル
            foldout[i] = EditorGUILayout.InspectorTitlebar( foldout[i], target.targetObjects );

            // 再帰的に全Propertyを表示・編集
            var iter = target.GetIterator();
            if( foldout[i] && iter.Next( true ) ) {
                EditorGUI.indentLevel++;
                do {
                    DrawSerializedProperty( iter );
                }
                while( iter.Next( false ) );
                EditorGUI.indentLevel--;
            }

            // 編集結果を保存
            target.ApplyModifiedProperties();
        }
        EditorGUILayout.EndScrollView();
    }

    void CheckSelections() {
        DisposeOldTargets();
        warning = false;
        foreach( var obj in Selection.objects ) {
            if( AssetDatabase.GetAssetPath( obj ).EndsWith( ".unity" ) ) {
                warning = true;
                break;
            }
        }
        if( !warning ) {
            AddSelectionToTargets();
        }
    }

    public void OnSelectionChange() {
        CheckSelections();
        Repaint();
    }

    void DisposeOldTargets() {
        foreach( var target in targets ) {
            target.Dispose();
        }
        targets.Clear();
    }

    /// <summary>
    /// Selection.objectsをtargetsに加える。
    /// </summary>
    void AddSelectionToTargets() {
        lastSelected = Selection.activeObject;
        foreach( Object obj in Selection.objects ) {
            if( AssetDatabase.Contains( obj ) ) {
                var path = AssetDatabase.GetAssetPath( obj );
                // AssetImporterも取得
                targets.Add( new SerializedObject( AssetImporter.GetAtPath( path ) ) );
                if( System.IO.Directory.Exists( path ) ) {
                    // フォルダの場合はフォルダだけ
                    targets.Add( new SerializedObject( obj ) );
                }
                else {
                    // Assetの場合は同ファイルに含まれるものを全て取得
                    var assets = AssetDatabase.LoadAllAssetsAtPath( path );
                    targets.AddRange( assets.Select( x => new SerializedObject( x ) ) );
                }
            }
            else if( obj is GameObject ) {
                // GameObjectの場合はComponentも取得
                var go = obj as GameObject;
                targets.Add( new SerializedObject( obj ) );
                targets.AddRange( go.GetComponents<Component>().Select( x => new SerializedObject( x ) ) );
            }
            else {
                targets.Add( new SerializedObject( obj ) );
            }
        }
        foldout = Enumerable.Repeat( true, targets.Count ).ToArray();
    }

    /// <summary>
    /// SerializedPropertyの中身を再帰的に表示。
    /// </summary>
    /// <param name="prop">SerializedProperty.</param>
    void DrawSerializedProperty( SerializedProperty prop ) {
        switch( prop.propertyType ) {
            case SerializedPropertyType.Generic:
                // 配列とObjectは折りたたみ式
                prop.isExpanded = EditorGUILayout.Foldout( prop.isExpanded, prop.name );
                if( !prop.isExpanded )
                    break;

                // インデント
                EditorGUI.indentLevel++;
                if( !prop.isArray ) {
                    // Serializable属性が付いたObject
                    var child = prop.Copy();
                    var end = prop.GetEndProperty( true );
                    if( child.Next( true ) ) {
                        while( !SerializedProperty.EqualContents( child, end ) ) {
                            DrawSerializedProperty( child );
                            if( !child.Next( false ) )
                                break;
                        }
                    }
                }
                else {
                    // 配列も上と同じ扱いでも問題ないが、操作APIが用意されているのでそれを使う
                    prop.arraySize = EditorGUILayout.IntField( "Length", prop.arraySize );
                    var showCount = Mathf.Min( prop.arraySize, maxVisibleArrayElements );
                    for( int i = 0; i < showCount; i++ ) {
                        DrawSerializedProperty( prop.GetArrayElementAtIndex( i ) );
                    }
                    // 重くなるので全ては表示しない
                    if( prop.arraySize > showCount ) {
                        GUILayout.BeginHorizontal();
                        // 無理矢理インデント
                        for( int i = 0; i < EditorGUI.indentLevel; i++ ) {
                            GUILayout.Space( EditorGUIUtility.singleLineHeight );
                        }
                        if( GUILayout.Button( "Show more ..." ) ) {
                            maxVisibleArrayElements += defaultVisibleArrayElements;
                        }
                        GUILayout.EndHorizontal();
                        someArraysVisible = true;
                    }
                }
                // インデント戻す
                EditorGUI.indentLevel--;
                break;
            case SerializedPropertyType.Integer:
                prop.intValue = EditorGUILayout.IntField( prop.name, prop.intValue );
                break;
            case SerializedPropertyType.Boolean:
                prop.boolValue = EditorGUILayout.Toggle( prop.name, prop.boolValue );
                break;
            case SerializedPropertyType.Float:
                prop.floatValue = EditorGUILayout.FloatField( prop.name, prop.floatValue );
                break;
            case SerializedPropertyType.String:
                prop.stringValue = EditorGUILayout.TextField( prop.name, prop.stringValue );
                break;
            case SerializedPropertyType.Color:
                prop.colorValue = EditorGUILayout.ColorField( prop.name, prop.colorValue );
                break;
            case SerializedPropertyType.ObjectReference:
                prop.objectReferenceValue = EditorGUILayout.ObjectField(
                    prop.name, prop.objectReferenceValue, typeof( Object ), true );
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField( "Type", prop.type );
                EditorGUI.indentLevel--;
                break;
            case SerializedPropertyType.LayerMask:
                prop.intValue = EditorGUILayout.IntField( prop.name, prop.intValue );
                break;
            case SerializedPropertyType.Enum:
                // Maskの場合と通常の場合両方表示
                EditorGUILayout.PropertyField( prop );
                prop.enumValueIndex = EditorGUILayout.IntField( prop.name, prop.enumValueIndex );
                EditorGUI.indentLevel++;
                prop.enumValueIndex = EditorGUILayout.Popup( "< Enum >", prop.enumValueIndex, prop.enumNames );
                prop.enumValueIndex = EditorGUILayout.MaskField( "< Mask >", prop.enumValueIndex, prop.enumNames );
                EditorGUI.indentLevel--;
                break;
            case SerializedPropertyType.Vector2:
                prop.vector2Value = EditorGUILayout.Vector2Field( prop.name, prop.vector2Value );
                break;
            case SerializedPropertyType.Vector3:
                prop.vector3Value = EditorGUILayout.Vector3Field( prop.name, prop.vector3Value );
                break;
            case SerializedPropertyType.Rect:
                prop.rectValue = EditorGUILayout.RectField( prop.name, prop.rectValue );
                break;
            case SerializedPropertyType.ArraySize:
                prop.intValue = EditorGUILayout.IntField( prop.name, prop.intValue );
                break;
            case SerializedPropertyType.Character:
                EditorGUILayout.PropertyField( prop );
                break;
            case SerializedPropertyType.AnimationCurve:
                prop.animationCurveValue = EditorGUILayout.CurveField( prop.name, prop.animationCurveValue );
                break;
            case SerializedPropertyType.Bounds:
                prop.boundsValue = EditorGUILayout.BoundsField( prop.name, prop.boundsValue );
                break;
            case SerializedPropertyType.Gradient:
                EditorGUILayout.PropertyField( prop );
                break;
            case SerializedPropertyType.Quaternion:
                prop.quaternionValue = Quaternion.Euler(
                    EditorGUILayout.Vector3Field( prop.name, prop.quaternionValue.eulerAngles ) );
                break;
        }
    }
}
