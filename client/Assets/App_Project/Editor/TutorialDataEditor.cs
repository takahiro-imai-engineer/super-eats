using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TutorialData))]
public class TutorialDataEditor : Editor
{
    // public override void OnInspectorGUI()
    // {
    //     base.OnInspectorGUI();

    //     TutorialData instance = target as TutorialData;

    //     if (instance.TutorialType == InGameConstant.TutorialType.OperationInstruction)
    //     {
    //         instance.TutorialOperationType = (InGameConstant.TutorialOperationType)EditorGUILayout.EnumPopup("TutorialOperationType", instance.TutorialOperationType);
    //     }
    // }
}
