using UnityEngine;
using UnityEditor;

namespace app_system
{
	/// <summary>
	/// データアセット（ScriptableObject）作成
	/// </summary>
	public class DataAssetCreator {

        /// <summary>
        /// 作成
        /// </summary>
        /// <typeparam name="T">タイプ</typeparam>
        /// <typeparam name="isUniquePath">ユニークなファイル名にするかどうか</typeparam>
        /// <returns>出力パス</returns>
        public static string Create<T>( bool isUniquePath = true ) where T : ScriptableObject {

			// Assets 直下もしくは、フォルダを１つだけ選択するように
			var objs = Selection.objects;
			if( objs.Length > 1 ) {
				EditorUtility.DisplayDialog( "DataAssetCreator", "Error:\n\n" + "Select Only One Folder!", "OK" );
				return null;
			}

			// 出力パス
			var path = objs.Length == 0 ? null : AssetDatabase.GetAssetPath( objs[0] );
			if( string.IsNullOrEmpty( path ) ) {
				path = "Assets/";
			}
			if( System.IO.Directory.Exists( path ) == false ) {
				EditorUtility.DisplayDialog( "DataAssetCreator", "Error:\n\n" + "Select Only One Folder!", "OK" );
				return null;
			}
			path = System.IO.Path.Combine( path, typeof( T ).Name );
            string filePath = string.Format( "{0}.asset", path );
            if( isUniquePath ) {
                filePath = AssetDatabase.GenerateUniqueAssetPath( filePath );
            }

            // ScriptableObject 作成
            T dataAsset = ScriptableObject.CreateInstance<T>();

			// アセット作成
			AssetDatabase.CreateAsset( dataAsset, filePath );
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = dataAsset;

            return filePath;
        }
	}
} // app_system
