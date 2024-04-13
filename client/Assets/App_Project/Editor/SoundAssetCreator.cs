using UnityEditor;
using app_system;

namespace template
{
    /// <summary>
    /// サウンドアセット作成
    /// </summary>
    public class SoundAssetCreator
    {
#if true
        /// <summary>
        /// BGM コンテナ作成
        /// </summary>
        [MenuItem("[Game]/Sound/Create BGM Container")]
        public static void CreateBGMContainer()
        {

            // データアセット（ScriptableObject）作成
            DataAssetCreator.Create<BGMContainer>();
        }

        /// <summary>
        /// ジングルコンテナ作成
        /// </summary>
        [MenuItem("[Game]/Sound/Create Jingle Container")]
        public static void CreateJingleContainer()
        {

            // データアセット（ScriptableObject）作成
            DataAssetCreator.Create<JingleContainer>();
        }

        /// <summary>
        /// SE コンテナ作成
        /// </summary>
        [MenuItem("[Game]/Sound/Create SE Container")]
        public static void CreateSEContainer()
        {

            // データアセット（ScriptableObject）作成
            DataAssetCreator.Create<SEContainer>();
        }

        /// <summary>
        /// ボイスコンテナ作成
        /// </summary>
        [MenuItem("[Game]/Sound/Create Voice Container")]
        public static void CreateVoiceContainer()
        {

            // データアセット（ScriptableObject）作成
            DataAssetCreator.Create<VoiceContainer>();
        }
#endif
    }
} // template
