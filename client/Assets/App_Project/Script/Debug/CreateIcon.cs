using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode] //SendMessageでエラーが出ないように
public class CreateIcon : MonoBehaviour
{

    /// <summary>
    /// アイコンタイプ
    /// </summary>
    public enum IconType
    {
        Character,
        Food,
        Movie
    }
    //================================================================================
    // インスペクタ
    //================================================================================
    //表示したログ、SerializeFieldを付ける事でInspectorに表示されるように
    [SerializeField] private Camera camera = null;
    [Header("アイコンの種類(「Assets/App_Project/Image/Icon/XXX/iconName.png」の保存先に使用)")]
    [SerializeField] private IconType iconType = IconType.Character;
    [SerializeField] private Vector2 iconSize = new Vector2(256, 256);
    [SerializeField] private string iconName = "";
    [SerializeField] private bool isUpdateCapture = false;
    private int captureIndex = 0;
    private string saveIconName = "";

    //================================================================================
    // メソッド
    //================================================================================
    private void Awake()
    {
        saveIconName = iconName;
    }

    private void Update()
    {
        if (!isUpdateCapture)
        {
            return;
        }
        captureIndex++;
        saveIconName = $"{iconName}_{captureIndex}";
        Save();
    }

    /// <summary>
    /// </summary>
    private void Save()
    {

        var icon = camera.targetTexture;
        if (icon == null)
        {
            Debug.LogError("カメラのtargetTextureがnullです。");
            return;
        }
        if (iconName == "")
        {
            Debug.LogError("アイコン名が入力されていません。");
            return;
        }

#if UNITY_EDITOR
        StartCoroutine(Capture());
        // AssetDatabase.CreateAsset (icon, $"Assets/App_Project/Image/Icon/{iconType.ToString()}/{iconName}.png");
        // AssetDatabase.Refresh ();
#endif
    }

    public IEnumerator Capture()
    {
        //_captureCameraに写るものを800*600でPNG画像として保存
        var coroutine = StartCoroutine(CaptureFromCamera((int)iconSize.x, (int)iconSize.y, camera));
        yield return coroutine;
    }

    /// <summary>
    /// CaptureMain
    /// </summary>
    /// <param name="width">横解像度</param>
    /// <param name="height">縦解像度</param>
    private IEnumerator CaptureFromCamera(int width, int height, Camera camera)
    {
        var d_width = camera.targetTexture.width;
        var d_height = camera.targetTexture.height;

        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

        if (camera.targetTexture != null)
            camera.targetTexture.Release();

        camera.targetTexture = new RenderTexture(width, height, 24);

        yield return new WaitForEndOfFrame();

        RenderTexture.active = camera.targetTexture;
        tex.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        string savePath = $"Assets/App_Project/Image/Icon/{iconType.ToString()}/{saveIconName}.png";
        File.WriteAllBytes(savePath, bytes);

        Destroy(tex);

        if (camera.targetTexture != null)
            camera.targetTexture.Release();

        yield break;
    }

}