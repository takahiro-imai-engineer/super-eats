using System.IO;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using UnityEditor;
using UnityEngine;

/// <summary>
/// タグを識別子と紐付ける
/// </summary>
public class TagsGenerator : AssetPostprocessor {

    static string OutputPath = "App_System/Tags/Scripts/Tags.cs";
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if(!System.Array.Exists(importedAssets, asset => asset.EndsWith("TagManager.asset"))) {
            return;
        }
        var tags = UnityEditorInternal.InternalEditorUtility.tags;
        var code = GenerateTagsCode(tags);
        var path = Path.Combine(Application.dataPath, OutputPath);
        if(!Directory.Exists(path)) {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        File.WriteAllText(path, code, Encoding.UTF8);
        AssetDatabase.ImportAsset($"Assets/{OutputPath}");
    }

    /// <summary>
    /// Tagsからソースコードのテキストを生成する
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    static string GenerateTagsCode(string[] tags) {
        var dom = GenerateTagsDom(tags);
        var compilerOptions = new CodeGeneratorOptions { IndentString = "    " };
        var codeText = new StringBuilder();
        using (var codeWriter = new StringWriter(codeText))
        {
            CodeDomProvider.CreateProvider("C#").GenerateCodeFromNamespace(dom, codeWriter, compilerOptions);
        }
        return codeText.ToString();
    }

    /// <summary>
    /// Tagsクラスを生成する
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    static CodeNamespace GenerateTagsDom(string[] tags)
    {
        var nameSpace = new CodeNamespace("app_system");
        var tagsClass = new CodeTypeDeclaration("Tags");
        nameSpace.Types.Add(tagsClass);
        // タグ名を返すプロパティを追加
        foreach (string tag in tags)
        {
            var tagProperty = new CodeMemberProperty()
            {
                Name = TagToPropertyName(tag),
                Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final,
                Type = new CodeTypeReference(typeof(string))
            };
            tagProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(tag)));
            tagsClass.Members.Add(tagProperty);
        }
        return nameSpace;
    }

    /// <summary>
    /// タグをプロパティ名に変換する
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    static string TagToPropertyName(string tag) {
        if(tag == string.Empty) {
            return tag;
        }
        string name = tag;
        if(char.IsNumber(tag[0])) {
            name = "_" + name;
        }
        if(name.IndexOf(' ') >= 0) {
            name = name.Replace(" ", "");
        }
        return name;
    }
}