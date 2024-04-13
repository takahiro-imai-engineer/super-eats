using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class GameUtility {
	//--------------------------------------------------------------------------------
	// Listから要素をランダムで1つ取得する
	//--------------------------------------------------------------------------------
	public static T GetRandom<T> (IReadOnlyList<T> list) {
		return list[UnityEngine.Random.Range (0, list.Count)];
	}
}
