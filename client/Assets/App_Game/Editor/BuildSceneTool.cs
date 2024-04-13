using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using app_system;

/// <summary>
/// ビルドシーンツール
/// </summary>
public class BuildSceneTool : EditorWindow {

    /// <summary>
    /// オープン
    /// </summary>
    [MenuItem( "[Game]/Build/Build Scene Tool" )]
    static void Open() {

        // オープン
        EditorWindow.GetWindow<BuildSceneTool>( false, typeof( BuildSceneTool ).Name );
    }

    //================================================================================

    /// <summary>シーン要素</summary>
    internal class SceneElement {

        /// <summary>選択したかどうか</summary>
        public bool IsSelected = false;

        /// <summary>シーンアセットパス</summary>
        public string ScenePath;
    }

    /// <summary>シーン要素リスト</summary>
    List<SceneElement> sceneElements = null;

    /// <summary>コンテンツリスト・スクロール位置</summary>
    Vector2 scrollPos = Vector2.zero;

    /// <summary>レイアウト</summary>
    const float CAPTION_SELECT_BOX_SIZE = 10.0f;
    const float CAPTION_SCENE_NAME_SIZE = 1000;
    const float TOP_BUTTOM_PADDING = 5.0f;

    //================================================================================

    /// <summary>
    /// 描画
    /// </summary>
    void OnGUI() {

        // セットアップ
        setup();

        // 上パディング
        GUILayout.Space( TOP_BUTTOM_PADDING );

        // コンテンツ描画
        drawContent();

        // 操作描画
        drawOperation();

        // 下パディング
        GUILayout.Space( TOP_BUTTOM_PADDING );
    }

    /// <summary>
    /// セットアップ
    /// </summary>
    void setup() {
        if( sceneElements != null ) {
            return;
        }

        // シーン要素リスト
        sceneElements = new List<SceneElement>();

        // 選択フォルダ内のシーンアセットを取得
        var scenes = Selection.GetFiltered<SceneAsset>( SelectionMode.DeepAssets );
        scenes = scenes.OrderBy( scene => AssetDatabase.GetAssetPath( scene ) ).ToArray();

        // 全シーンアセット分のシーン要素を作る
        foreach( var scene in scenes ) {

            // シーン要素
            var sceneElement = new SceneElement();

            // 選択中
            sceneElement.IsSelected = true;

            // シーンアセットパス
            sceneElement.ScenePath = AssetDatabase.GetAssetPath( scene );

            // シーン要素リストへ追加
            sceneElements.Add( sceneElement );
        }

        // コンテンツリスト・スクロール位置
        scrollPos = Vector2.zero;
    }

    /// <summary>
    /// コンテンツ描画
    /// </summary>
    void drawContent() {

        // コンテンツリスト・スクロール表示
        scrollPos = EditorGUILayout.BeginScrollView( scrollPos, GUI.skin.box );
        {
            // シーン要素分まわす
            foreach( var sceneElement in sceneElements ) {

                EditorGUILayout.BeginHorizontal( EditorStyles.helpBox );
                {
                    // 選択チェックボックス
                    sceneElement.IsSelected = GUILayout.Toggle( sceneElement.IsSelected, "", GUI.skin.toggle, GUILayout.Width( CAPTION_SELECT_BOX_SIZE ) );
                    GUILayout.FlexibleSpace();

                    // シーン名
                    GUILayout.Label( sceneElement.ScenePath, GUI.skin.label, GUILayout.Width( CAPTION_SCENE_NAME_SIZE ) );
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 操作描画
    /// </summary>
    void drawOperation() {
        if( sceneElements.Count == 0 ) {
            return;
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            // Build Settings に適用
            if( GUILayout.Button( "Apply to Build Settings", GUI.skin.button ) ) {

                // Build Settings の「Scene In Build」を書き換える
                List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
                sceneElements.ForEach( element => {
                    editorBuildSettingsScenes.Add( new EditorBuildSettingsScene( element.ScenePath, element.IsSelected ) );
                } );
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
