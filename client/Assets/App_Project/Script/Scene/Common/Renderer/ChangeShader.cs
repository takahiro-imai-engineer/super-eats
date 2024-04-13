using UnityEngine;

public class ChangeShader : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    [SerializeField] string shaderName;
    void Awake()
    {
        var shader = Shader.Find(shaderName);
        if (shader == null)
        {
            Debug.LogError($"Shader Not Found: {shaderName}");
            return;
        }
        renderer.material.shader = shader;
    }
}
