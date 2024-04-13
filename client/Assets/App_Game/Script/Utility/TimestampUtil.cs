using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class TimestampUtil {

	public static System.DateTime unixTime = new System.DateTime( 1970, 1, 1, 0, 0, 0, 0 );

	// TimeStampを取得する
	public static long Timestamp() {
		System.DateTime targetTime = System.DateTime.Now.ToUniversalTime();
		System.TimeSpan elapsedTime = targetTime - unixTime;
		return ( long )elapsedTime.TotalMilliseconds;
	}

	/// <summary>
	/// UnixTimeに変換する。
	/// </summary>
	/// <param name="dateTime"></param>
	/// <returns></returns>
	public static long FromDateTime( string dateTimeString ) {
		DateTime dateTime = DateTime.Parse( dateTimeString );
		double nowTicks = ( dateTime.ToUniversalTime() - unixTime ).TotalSeconds;
		return ( long )nowTicks;
	}

	/// <summary>
	/// DateTimeに変換する
	/// </summary>
	/// <param name="unixTimeLong"></param>
	/// <returns></returns>
	public static DateTime FromUnixTime( long unixTimeLong ) {
		return unixTime.AddSeconds( unixTimeLong ).ToLocalTime();
	}



	public static void Dump( object obj ) {
		string text = Dump( obj, "", "" );
		Debug.Log( text );
	}

	private static string Dump( object obj, string space, string text ) {
		object target = obj;
		Type targetType = target.GetType();
		BindingFlags allMemberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		FieldInfo[] targetFields = targetType.GetFields( allMemberBindingFlags );


		// クラス名
		text += space + targetType.Name + " {\n";
		foreach( FieldInfo field in targetFields ) {
			// オブジェクトの場合
			if( field.FieldType != typeof( int )
				&& field.FieldType != typeof( short )
				&& field.FieldType != typeof( string )
				&& field.FieldType != typeof( long )
				&& field.FieldType != typeof( bool )
				&& field.FieldType != typeof( double )
				&& field.FieldType != typeof( float )
				) {
				Debug.Log( field.Name );
				Debug.Log( field.FieldType );
				Debug.Log( field.GetValue( target ) );

				// Nullの場合
				if( null == field.GetValue( target ) ) {
					text += space + "    " + field.Name + " => null \n";
				}
				else if( field.FieldType == typeof( IList ) ) {
					// リストの場合(本来はforeach)
					/*					text += space + "    " + field.Name +" => \n" ;
										foreach(var result in field.GetValue(target)) {
											text = Dump(result, (space + "    "), text);
										}*/
					break;
				}
				else {
					text += space + "    " + field.Name + " => \n";
					text = Dump( field.GetValue( target ), ( space + "    " ), text );
				}
			}
			else {
				text += space + "    " + field.Name + " => " + field.GetValue( target ) + "\n";
			}
		}
		text += space + "}\n";

		return text;
	}
}
