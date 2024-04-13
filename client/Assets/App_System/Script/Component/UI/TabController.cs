using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace app_system
{
	/// <summary>
	/// タブコントローラ
	/// </summary>
	public class TabController : MonoBehaviour {

		//==========================================================================
		// インスペクタ
		//==========================================================================

		/// <summary>タブコンテンツ</summary>
		[SerializeField]
		private List<TabContent> tabContents;

		//==========================================================================

		/// <summary>タブコンテンツ</summary>
		[System.Serializable]
		private struct TabContent {

			/// <summary>タブボタン</summary>
			public Button tabButton;

			/// <summary>対象のコンテンツ</summary>
			public RectTransform targetContent;

			/// <summary>選択中のタブ（null の場合は無効）</summary>
			public RectTransform selectedTab;

			/// <summary>
			/// インタラクションの設定
			/// </summary>
			/// <param name="isEnabled">true なら有効</param>
			public void SetTabInteractable( bool isEnabled ) {
				tabButton.interactable = isEnabled;

				// 「選択中のタブ」が有効ならそちらを表示
				if( selectedTab != null ) {
					selectedTab.gameObject.SetActive( !isEnabled );
					tabButton.gameObject.SetActive( isEnabled );
				}
			}
		}

		//==========================================================================
		// Mono
		//==========================================================================

		/// <summary>
		/// 開始
		/// </summary>
		private void Start() {
			if( tabContents == null ) {
				return;
			}

			// セットアップ
			tabContents.ForEach( content => {

				// 対象コンテンツのアクティブ状態でタブボタンのインタラクションを設定（対象コンテンツはあらかじめ有効／無効を設定しておく）
				content.SetTabInteractable( !content.targetContent.gameObject.activeSelf );

				// タブボタン押下時のイベント
				content.tabButton.onClick.AddListener( () => {

					// 自身の対象コンテンツをアクティブにする
					tabContents.ForEach( dto => dto.targetContent.gameObject.SetActive( false ) );
					content.targetContent.gameObject.SetActive( true );

					// 自身以外のタブボタンのインタラクションを有効にする
					tabContents.ForEach( dto => dto.SetTabInteractable( true ) );
					content.SetTabInteractable( false );
				} );
			} );
		}
	}
} // app_system
