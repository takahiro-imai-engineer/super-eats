#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.UI
{
	/// <summary>
	/// uGUI の GraphicRaycaster の当たり判定だけを使う
	/// 画像のない Image 等で、タッチイベントのブロックをするのは　Overdraw になるので、これをアタッチして描画をしないようにする
	/// raycast の領域は Rect Transform のサイズになる
	/// </summary>
	public class GraphicCast : Graphic {

		/// <summary>
		/// Raises the populate mesh event.
		/// </summary>
		/// <param name="vh">Vh.</param>
		protected override void OnPopulateMesh( VertexHelper vh ) {
			base.OnPopulateMesh( vh );
			vh.Clear();
		}

#if UNITY_EDITOR
		/// <summary>
		/// Graphic cast editor.
		/// </summary>
		[CustomEditor( typeof( GraphicCast ) )]
		class GraphicCastEditor : Editor {

			/// <summary>
			/// Raises the inspector GU event.
			/// </summary>
			public override void OnInspectorGUI() {

			}
		}
#endif
	}
}
