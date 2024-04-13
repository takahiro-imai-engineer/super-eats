using System;
using System.Reflection;

namespace app_system
{
	/// <summary>
	/// enum 値属性の基底クラス
	/// </summary>
	[AttributeUsage( AttributeTargets.Field, AllowMultiple = false, Inherited = true )]
	public class ValueAttributeBase : Attribute {

		/// <summary>値</summary>
		public int IntValue { get; private set; }
        public float FloatValue { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">値</param>
        protected ValueAttributeBase( int value ) {
			this.IntValue = value;
		}
        protected ValueAttributeBase( float value ) {
            this.FloatValue = value;
        }

        /// <summary>
        /// enum 値からの取得
        /// </summary>
        /// <typeparam name="T">タイプ</typeparam>
        /// <param name="enumValue">enum 値</param>
        /// <returns>値</returns>
        public static int GetIntValue<T>( Enum enumValue ) where T : ValueAttributeBase {
			string name = enumValue.ToString();
			FieldInfo info = enumValue.GetType().GetField( name );
			Type type = typeof( T );
			ValueAttributeBase attr = ( ValueAttributeBase )System.Attribute.GetCustomAttribute( info, type );
			return attr.IntValue;
		}
        public static float GetFloatValue<T>( Enum enumValue ) where T : ValueAttributeBase {
            string name = enumValue.ToString();
            FieldInfo info = enumValue.GetType().GetField( name );
            Type type = typeof( T );
            ValueAttributeBase attr = ( ValueAttributeBase )System.Attribute.GetCustomAttribute( info, type );
            return attr.FloatValue;
        }
    }

    /// <summary>
    /// enum 値属性
    /// </summary>
    public class ValueAttribute : ValueAttributeBase {

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="value">値</param>
		public ValueAttribute( int value ) : base( value ) {}
        public ValueAttribute( float value ) : base( value ) {}
    }

    /// <summary>
    /// enum 値属性の拡張
    /// </summary>
    public static class ValueAttributeExtensions {

		/// <summary>
		/// enum 値取得
		/// </summary>
		/// <param name="enumValue">enum 値</param>
		/// <returns>値</returns>
		public static int GetIntValue( this Enum enumValue ) {
			return ValueAttributeBase.GetIntValue<ValueAttribute>( enumValue );
		}
        public static float GetFloatValue( this Enum enumValue ) {
            return ValueAttributeBase.GetFloatValue<ValueAttribute>( enumValue );
        }
    }
} // app_system
