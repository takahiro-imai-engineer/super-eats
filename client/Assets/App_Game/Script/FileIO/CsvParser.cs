using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;

/// <summary>
/// CSV パーサ
/// </summary>
public static class CsvParser {

    /// <summary>
    /// パース
    /// </summary>
    /// <typeparam name="T">タイプ</typeparam>
    /// <param name="csv">CSV テキスト</param>
    /// <returns>タイプオブジェクトのリスト。エラーの場合は null</returns>
    public static List<T> Parse<T>( string csv ) where T : new() {

        // 行ごとに、カラムに対応する文字列にする
        List<string[]> records = new List<string[]>();
        using( var reader = new StringReader( csv ) ) {
            string line;
            while( ( line = reader.ReadLine() ) != null ) {
                records.Add( line.Split( ',' ) );
            }
        }
        if( records.Count == 0 ) {
            return null;
        }

        // フィールド情報
        FieldInfo[] fields = typeof( T ).GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly );

        // 各フィールドがカラムのどの位置にあるかを得る
        int[] columnIndices = new int[fields.Length];
        int columLine = 0;
        for( int fieldIdx = 0; fieldIdx < fields.Length; fieldIdx++ ) {
            int columnIdx = -1;
            for( int recordIdx = columLine; recordIdx < records.Count; recordIdx++ ) {

                // カラム名が合致したら位置確定
                var fieldName = fields[fieldIdx].Name;
                columnIdx = Array.IndexOf( records[recordIdx], fieldName );
                if( columnIdx >= 0 ) {

                    // カラム行
                    columLine = recordIdx;
                    break;
                }
            }
            if( columnIdx < 0 ) {
                UnityEngine.Debug.LogError( string.Format( "Invalid Column Name: {0}.{1}", typeof( T ).Name, fields[fieldIdx].Name ) );
                return null;
            }

            // フィールド位置に対するカラム位置
            columnIndices[fieldIdx] = columnIdx;
        }

        // タイプオブジェクトのリスト
        List<T> typeObjects = new List<T>();

        // カラム行の次からレコード数分まわす
        for( int recordIdx = columLine + 1; recordIdx < records.Count; recordIdx++ ) {

            // タイプオブジェクト作成
            T typeObj = new T();

            // フィールド位置に対応するカラムの内容を変換して格納
            for( int fieldIdx = 0; fieldIdx < fields.Length; fieldIdx++ ) {

                // 変換する値
                var record = records[recordIdx];
                var convertValue = record[columnIndices[fieldIdx]];
                if( string.IsNullOrEmpty( convertValue ) ) {
                    continue;   // 空の場合は何もしない
                }

                // フィールドタイプで変換
                Type fieldType = fields[fieldIdx].FieldType;
                try {
                    object fieldValue;
                    if( fieldType.IsEnum ) {    // enum の場合
                        fieldValue = Enum.Parse( fieldType, convertValue );
                    }
                    else {
                        fieldValue = System.Convert.ChangeType( convertValue, fieldType );
                    }

                    // フィールド値を格納
                    fields[fieldIdx].SetValue( typeObj, fieldValue );
                }
                catch( Exception e ) {
                    UnityEngine.Debug.LogError( string.Format( "Invalid Column Value: {0}.{1}\n[{2}]", typeof( T ).Name, fields[fieldIdx].Name, e.Message ) );
                }
            }

            // リストへ
            typeObjects.Add( typeObj );
        }
        if( typeObjects.Count == 0 ) {
            return null;
        }

        return typeObjects;
    }
}
