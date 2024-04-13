using JsonFx.Json;

public class JsonUtil {
	// オブジェクトのJsonシリアライズを行い返却する
	public static string GetSerialize<T>( T objectValue ) {
		//        string jsonString = JsonUtility.ToJson(objectValue);
		string jsonString = JsonWriter.Serialize( objectValue );
		return jsonString;
	}

	// json文字列のデシリアライズを行いオブジェクトを返却する
	public static T GetDeserialize<T>( string jsonString ) {

		//        var jsonObject = JsonUtility.FromJson<T>(jsonString);
		var jsonObject = JsonReader.Deserialize<T>( jsonString );
		return jsonObject;
	}
}
