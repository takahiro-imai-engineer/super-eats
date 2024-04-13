using System;
using System.Reflection;

namespace app_system
{
	/// <summary>
	/// enum ラベル属性の基底クラス
	/// </summary>
	[AttributeUsage( AttributeTargets.Field, AllowMultiple = false, Inherited = true )]
	public class LabelAttributeBase : Attribute {

		/// <summary>ラベル</summary>
		public string Label { get; private set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="label">ラベル</param>
		protected LabelAttributeBase( string label ) {
			this.Label = label;
		}

		/// <summary>
		/// enum 値からの取得
		/// </summary>
		/// <typeparam name="T">タイプ</typeparam>
		/// <param name="value">enum 値</param>
		/// <returns></returns>
		public static string Get<T>( Enum value ) where T : LabelAttributeBase {
			string name = value.ToString();
			FieldInfo info = value.GetType().GetField( name );
			Type type = typeof( T );
			LabelAttributeBase attr = ( LabelAttributeBase )System.Attribute.GetCustomAttribute( info, type );
			return attr.Label;
		}
	}

	/// <summary>
	/// enum ラベル属性
	/// </summary>
	public class LabelAttribute : LabelAttributeBase {

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="label">ラベル</param>
		public LabelAttribute( string label ) : base( label ) {}
	}

	/// <summary>
	/// enum ラベル属性の拡張
	/// </summary>
	public static class LabelAttributeExtensions {

		/// <summary>
		/// enum ラベル取得
		/// </summary>
		/// <param name="value">enum 値</param>
		/// <returns>ラベル文字列</returns>
		public static string GetLabel( this Enum value ) {
			return LabelAttributeBase.Get<LabelAttribute>( value );
		}
	}
} // app_system
